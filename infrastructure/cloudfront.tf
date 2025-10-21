# Create CloudFront Origin Access Identity
resource "aws_cloudfront_origin_access_identity" "origin_access_identity" {
  comment = "Origin Access Identity for static website - ${terraform.workspace}"
}

# Helper locals for domain aliases
locals {
  # Remove leading "*." if present (e.g. "*.microapps.info" -> "microapps.info")
  base_domain = replace(var.domain_name, "*.", "")
  app_name    = "askly"
  # Choose subdomain prefix (adjust logic as needed)
  # Map workspace "prod" to "app", others keep workspace name
  domain_prefix = terraform.workspace == "prod" ? local.app_name : "${terraform.workspace}-${local.app_name}"

  # Final alias list (only if custom domain enabled)
  cloudfront_aliases = var.enable_custom_domain ? ["${local.domain_prefix}.${local.base_domain}"] : []
}

# Create CloudFront Distribution
resource "aws_cloudfront_distribution" "cloudfront_distribution" {
  origin {
    domain_name = aws_s3_bucket.website_bucket.bucket_regional_domain_name
    origin_id   = local.current_config.bucket_name

    s3_origin_config {
      origin_access_identity = aws_cloudfront_origin_access_identity.origin_access_identity.cloudfront_access_identity_path
    }
  }

  aliases = local.cloudfront_aliases

  enabled             = true
  is_ipv6_enabled     = true
  default_root_object = var.website_index_document

  default_cache_behavior {
    allowed_methods  = ["GET", "HEAD"]
    cached_methods   = ["GET", "HEAD"]
    target_origin_id = local.current_config.bucket_name

    forwarded_values {
      query_string = false

      cookies {
        forward = "none"
      }
    }

    viewer_protocol_policy = "redirect-to-https"
    min_ttl                = 0
    default_ttl            = 300
    max_ttl                = 3600
  }

  custom_error_response {
    error_code            = 403
    response_page_path    = "/index.html"
    response_code         = 200
    error_caching_min_ttl = 0
  }

  custom_error_response {
    error_code            = 404
    response_page_path    = "/index.html"
    response_code         = 200
    error_caching_min_ttl = 0
  }

  dynamic "viewer_certificate" {
    for_each = var.enable_custom_domain ? [1] : []
    content {
      acm_certificate_arn      = local.is_dev ? aws_acm_certificate.cert[0].arn : data.aws_acm_certificate.existing[0].arn
      ssl_support_method       = "sni-only"
      minimum_protocol_version = "TLSv1.2_2019"
    }
  }

  dynamic "viewer_certificate" {
    for_each = var.enable_custom_domain ? [] : [1]
    content {
      cloudfront_default_certificate = true
      minimum_protocol_version       = "TLSv1.2_2019"
    }
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  tags = {
    Name        = "CloudFront Distribution - ${terraform.workspace}"
    Environment = terraform.workspace
  }
}

# Get the ID of the AWS managed CachingDisabled policy
data "aws_cloudfront_cache_policy" "caching_disabled" {
  name = "Managed-CachingDisabled"
}

resource "aws_cloudfront_origin_request_policy" "cors_allow_all" {
  name    = "CORS-AllowAll-${terraform.workspace}"
  comment = "CORS policy to allow all origins"

  headers_config {
    header_behavior = "whitelist"
    headers {
      items = [
        "Origin",
        "Access-Control-Request-Method",
        "Access-Control-Request-Headers",
        "Content-Type",
        "User-Agent",
        "Referer",
        "Accept"
      ]
    }
  }

  cookies_config {
    cookie_behavior = "all"
  }

  query_strings_config {
    query_string_behavior = "all"
  }
}

# Create a response headers policy to allow Set-Cookie
resource "aws_cloudfront_response_headers_policy" "allow_set_cookie" {
  name    = "AllowSetCookie-${terraform.workspace}"
  comment = "Allow Set-Cookie header from origin"

  cors_config {
    access_control_allow_credentials = true
    access_control_allow_headers {
      items = ["Origin", "Content-Type", "Accept", "Authorization", "Cookie"]
    }
    access_control_allow_methods {
      items = ["GET", "POST", "PUT", "DELETE", "HEAD", "OPTIONS"]
    }
    access_control_allow_origins {
      items = ["https://${local.domain_prefix}.${local.base_domain}"]
    }
    origin_override = true
  }
}

# Create CloudFront Distribution for API
resource "aws_cloudfront_distribution" "api_distribution" {
  origin {
    domain_name = trimsuffix(replace(module.api.lambda_function_url, "https://", ""), "/")
    origin_id   = module.api.lambda_function_url

    custom_origin_config {
      http_port              = 80
      https_port             = 443
      origin_protocol_policy = "https-only"
      origin_ssl_protocols   = ["TLSv1.2"]
    }
  }

  enabled         = true
  is_ipv6_enabled = true

  aliases = var.enable_custom_domain ? ["api-${local.domain_prefix}.${local.base_domain}"] : []

  # Cache behavior for authentication endpoints (no caching)
  default_cache_behavior {

    allowed_methods  = ["DELETE", "GET", "HEAD", "OPTIONS", "PATCH", "POST", "PUT"]
    cached_methods   = ["GET", "HEAD"]
    target_origin_id = module.api.lambda_function_url

    viewer_protocol_policy = "redirect-to-https"

    cache_policy_id            = data.aws_cloudfront_cache_policy.caching_disabled.id
    origin_request_policy_id   = aws_cloudfront_origin_request_policy.cors_allow_all.id
    response_headers_policy_id = aws_cloudfront_response_headers_policy.allow_set_cookie.id
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  dynamic "viewer_certificate" {
    for_each = var.enable_custom_domain ? [1] : []
    content {
      acm_certificate_arn      = local.is_dev ? aws_acm_certificate.cert[0].arn : data.aws_acm_certificate.existing[0].arn
      ssl_support_method       = "sni-only"
      minimum_protocol_version = "TLSv1.2_2019"
    }
  }

  dynamic "viewer_certificate" {
    for_each = var.enable_custom_domain ? [] : [1]
    content {
      cloudfront_default_certificate = true
      minimum_protocol_version       = "TLSv1.2_2019"
    }
  }

  tags = {
    Name        = "CloudFront API Distribution - ${terraform.workspace}"
    Environment = terraform.workspace
  }
}

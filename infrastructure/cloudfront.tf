# Create CloudFront Origin Access Identity
resource "aws_cloudfront_origin_access_identity" "origin_access_identity" {
  comment = "Origin Access Identity for static website - ${terraform.workspace}"
}

# Helper locals for domain aliases
locals {
  # Remove leading "*." if present (e.g. "*.microapps.info" -> "microapps.info")
  base_domain = replace(var.domain_name, "*.", "")
  app_name    = "chatbot"
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

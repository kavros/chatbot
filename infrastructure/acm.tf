###############################################
# ACM Certificate (only created in dev workspace; reused elsewhere)
###############################################


locals {
  is_dev = terraform.workspace == "dev"
}

# Create the certificate only in the dev (e.g., dev) workspace
resource "aws_acm_certificate" "cert" {
  count             = local.is_dev ? 1 : 0
  provider          = aws.us_east_1
  domain_name       = var.domain_name
  validation_method = "DNS"

  lifecycle {
    create_before_destroy = true
  }

  tags = {
    Name          = "ACM Certificate"
    Environment   = terraform.workspace
    ManagedBy     = "terraform"
    PrimarySource = local.is_dev ? "true" : "false"
  }
}

# In non-dev workspaces, look up the existing issued certificate by domain
data "aws_acm_certificate" "existing" {
  count       = local.is_dev ? 0 : 1
  provider    = aws.us_east_1
  domain      = var.domain_name
  types       = ["AMAZON_ISSUED"]
  statuses    = ["ISSUED"]
  most_recent = true
}

locals {
  certificate_arn = local.is_dev ? aws_acm_certificate.cert[0].arn : data.aws_acm_certificate.existing[0].arn
}

# Output the DNS records only when we are creating/validating (dev workspace)
output "acm_certificate_validation_records" {
  description = "CNAME records needed to validate the ACM certificate (only in dev workspace)."
  value = local.is_dev ? {
    for dvo in aws_acm_certificate.cert[0].domain_validation_options : dvo.domain_name => {
      name  = dvo.resource_record_name
      type  = dvo.resource_record_type
      value = dvo.resource_record_value
    }
  } : {}
}

# Unified output for consumers in all workspaces
output "acm_certificate_arn" {
  description = "ARN of the ACM certificate (created in dev, reused elsewhere)."
  value       = local.certificate_arn
}


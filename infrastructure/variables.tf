variable "aws_region" {
  default = "eu-west-2"
}

variable "environment" {
  description = "Environment name (dev/prod/default)"
  type        = string
  default     = "default"
}

variable "website_index_document" {
  default = "index.html"
}

variable "domain_name" {
  description = "The domain name for the website"
  type        = string
  default     = "*.microapps.info"
}

variable "enable_custom_domain" {
  description = "Set true after ACM cert is validated to attach custom domain to CloudFront; false uses default CloudFront domain."
  type        = bool
  default     = true
}

# Environment-specific configurations
locals {
  environment_config = {
    default = {
      bucket_name         = "the-static-chatbot-bucket-113"
      lambda_timeout      = 90
      log_retention_days  = 7
      deletion_protection = false
    }
    dev = {
      bucket_name         = "chatbot-bucket-dev-113"
      lambda_timeout      = 90
      log_retention_days  = 7
      deletion_protection = false
    }
    prod = {
      bucket_name         = "chatbot-bucket-prod-113"
      lambda_timeout      = 90
      log_retention_days  = 7
      deletion_protection = true
    }
  }

  current_config = local.environment_config[terraform.workspace]
}


locals {
  environment_name = terraform.workspace == "dev" ? "Development" : "Production"
}
resource "aws_ssm_parameter" "lambda_secrets" {
  name        = "lambda-secrets-${local.environment_name}"
  type        = "String"
  value       = "your-secret-value" # Replace with your actual secret or use a variable
  description = "Secrets for Lambda in the ${local.environment_name} environment"

  lifecycle {
    ignore_changes = [value]
  }
}

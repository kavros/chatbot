variable "aws_region" {
  type        = string
  description = "value of the AWS region to deploy resources in"
}

variable "aurora_dsql_cluster_identifier" {
  type        = string
  description = "Identifier of the Aurora DSQL cluster"
}

variable "workspace" {
  type        = string
  description = "Workspace name"
}

variable "lambda_timeout" {
  type        = number
  description = "Lambda function timeout in seconds"
  default     = 90
}

variable "log_retention_days" {
  type        = number
  description = "CloudWatch log retention in days"
  default     = 7
}

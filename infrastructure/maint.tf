// filepath: terraform/main.tf
module "api" {
  source                         = "./api"
  aws_region                     = var.aws_region
  aurora_dsql_cluster_identifier = aws_dsql_cluster.database_cluster.identifier
  workspace                      = terraform.workspace
  lambda_timeout                 = local.current_config.lambda_timeout
  log_retention_days             = local.current_config.log_retention_days
}

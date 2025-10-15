resource "aws_dsql_cluster" "database_cluster" {
  deletion_protection_enabled = local.current_config.deletion_protection

  tags = {
    Name        = "TestCluster-${terraform.workspace}"
    Environment = terraform.workspace
  }
}

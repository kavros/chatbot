# Outputs
output "s3_bucket_name" {
  value = aws_s3_bucket.website_bucket.id
}

output "cloudfront_distribution_id" {
  value = aws_cloudfront_distribution.cloudfront_distribution.id
}

output "cloudfront_distribution_domain_name" {
  value = aws_cloudfront_distribution.cloudfront_distribution.domain_name
}

output "lambda_function_url" {
  value = module.api.lambda_function_url
}

output "aurora_dsql_cluster_identifier" {
  value = aws_dsql_cluster.database_cluster.identifier
}


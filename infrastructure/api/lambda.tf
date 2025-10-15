## IAM Permissions and Roles related to Lambda
data "aws_iam_policy_document" "assume_role" {
  statement {
    effect = "Allow"
    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
    actions = ["sts:AssumeRole"]
  }
}

resource "aws_iam_role" "lambda_api_role" {
  name               = "lambda_api_role_${var.workspace}"
  assume_role_policy = data.aws_iam_policy_document.assume_role.json
}

resource "aws_iam_policy" "lambda_dsql_policy" {
  name        = "lambda_dsql_policy_${var.workspace}"
  description = "Allow Lambda to connect to DSQL cluster"
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "dsql:DbConnectAdmin"
        ]
        Resource = "arn:aws:dsql:${var.aws_region}:${data.aws_caller_identity.current.account_id}:cluster/${var.aurora_dsql_cluster_identifier}"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_dsql_attach" {
  role       = aws_iam_role.lambda_api_role.name
  policy_arn = aws_iam_policy.lambda_dsql_policy.arn
}

resource "aws_iam_policy" "lambda_ssm_policy" {
  name        = "lambda_ssm_policy_${var.workspace}"
  description = "Allow Lambda to get parameters from SSM Parameter Store"
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "ssm:GetParameter",
          "ssm:GetParameters",
          "ssm:GetParametersByPath"
        ]
        Resource = "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/lambda-secrets-${var.workspace == "prod" ? "Production" : "Development"}*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "lambda_ssm_attach" {
  role       = aws_iam_role.lambda_api_role.name
  policy_arn = aws_iam_policy.lambda_ssm_policy.arn
}
data "aws_caller_identity" "current" {}

## AWS Lambda Resources

resource "aws_lambda_function" "lambda-api" {
  filename      = "${path.module}/placeholder.zip"
  function_name = "lambda_api_${var.workspace}"
  role          = aws_iam_role.lambda_api_role.arn
  handler       = "lambda-api"
  runtime       = "dotnet8"
  timeout       = var.lambda_timeout

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = var.workspace == "prod" ? "Production" : "Development"
      CLUSTER_USER           = "admin"
      CLUSTER_ENDPOINT       = "${var.aurora_dsql_cluster_identifier}.dsql.${var.aws_region}.on.aws"
      REGION                 = var.aws_region
    }
  }

  tags = {
    Environment = var.workspace
  }

  lifecycle {
    ignore_changes = [filename, source_code_hash]
  }
}

resource "aws_lambda_function_url" "lambda-api_url" {
  function_name      = aws_lambda_function.lambda-api.function_name
  authorization_type = "NONE"
}



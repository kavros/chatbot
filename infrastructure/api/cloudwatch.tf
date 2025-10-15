resource "aws_cloudwatch_log_group" "lambda_api_logs" {
  name              = "/aws/lambda/lambda_api_${var.workspace}"
  retention_in_days = var.log_retention_days

  tags = {
    Environment = var.workspace
  }
}

data "aws_iam_policy_document" "log_policy_document" {
  statement {
    effect = "Allow"
    actions = [
      "logs:CreateLogGroup",
      "logs:CreateLogStream",
      "logs:PutLogEvents",
    ]
    resources = ["arn:aws:logs:*:*:*"]
  }
}

resource "aws_iam_policy" "log_policy" {
  name   = "log_policy_${var.workspace}"
  path   = "/"
  policy = data.aws_iam_policy_document.log_policy_document.json

  tags = {
    Environment = var.workspace
  }
}

resource "aws_iam_role_policy_attachment" "log_policy_attachment" {
  role       = aws_iam_role.lambda_api_role.name
  policy_arn = aws_iam_policy.log_policy.arn
}

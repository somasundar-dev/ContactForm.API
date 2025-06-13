output "api_gateway_url" {
  description = "The URL of the API Gateway"
  value       = aws_api_gateway_deployment.example.invoke_url
}
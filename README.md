# Scans Data API

 Agility Scans Data API is equipped with the following features:

* HTTPS enforced (HTTP requests are ignored)
* Accept header versioning (e.g. "Accept: application/vnd.ScansData.Api.v1+json" requests version 1)
* Autofac for dependency injection
* FluentValidation for model validation
* IdentityServer for authentication
* Swagger for documentation
* CQS to enable/enforce the single responsibility principle
* Serilog/Seq logging
* Exception handlers
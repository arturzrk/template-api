# Template API

This template API is equipped with the following features out of the box:

* HTTPS enforced (HTTP requests are ignored)
* Accept header versioning (e.g. "Accept: application/vnd.Template.Api.v1+json" requests version 1)
* Autofac for dependency injection
* FluentValidation for model validation
* IdentityServer for authentication
* Swagger for documentation
* CQS to enable/enforce the single responsibility principle
* Serilog/Seq logging
* Exception handlers
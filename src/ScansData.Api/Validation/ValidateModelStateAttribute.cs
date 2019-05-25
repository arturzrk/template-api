namespace ScansData.Api.Validation
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using Models.Shared.Validation.Version1;

    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var validationErrors = context.ModelState.Where(ms => ms.Value.Errors.Any(e => !string.IsNullOrWhiteSpace(e.ErrorMessage)))
                                          .SelectMany(kvp => kvp.Value.Errors.Select(e => new ValidationError(kvp.Key, e.ErrorMessage)))
                                          .OrderBy(ve => ve.PropertyName)
                                          .ToList();

            if (!validationErrors.Any())
            {
                return;
            }

            var logger = (ILogger<ValidateModelStateAttribute>)context.HttpContext.RequestServices.GetService(typeof(ILogger<ValidateModelStateAttribute>));
            logger.LogInformation
                (
                    "ModelState is invalid. {@ValidationErrors}",
                    validationErrors
                );

            var response = validationErrors.Any() ? new ValidationFailedResponse(validationErrors) : null;
            var result = new BadRequestObjectResult(response);

            context.Result = result;
        }
    }
}
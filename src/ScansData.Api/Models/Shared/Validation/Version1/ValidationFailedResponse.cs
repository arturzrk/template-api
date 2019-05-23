namespace Template.Api.Models.Shared.Validation.Version1
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Error.Version1;

    public class ValidationFailedResponse: ErrorResponse
    {
        public ValidationFailedResponse( IEnumerable<ValidationError> validationErrors )
            : base( "ValidationFailed", "The request contains one or more validation errors." )
        {
            ValidationErrors = validationErrors.ToImmutableArray();
        }

        public IReadOnlyCollection<ValidationError> ValidationErrors { get; }
    }
}
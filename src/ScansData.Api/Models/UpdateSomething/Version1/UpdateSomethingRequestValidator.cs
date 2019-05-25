namespace ScansData.Api.Models.UpdateSomething.Version1
{
    using FluentValidation;

    public class UpdateSomethingRequestValidator: AbstractValidator<UpdateSomethingRequest>
    {
        public UpdateSomethingRequestValidator()
        {
            RuleFor( m => m.Name )
                .NotEmpty()
                .WithMessage( "'{PropertyName}' is required." )
                .Matches( @"^[\w\d]*$" )
                .WithMessage( @"'{PropertyName}' must match '^[\w\d]*$'." );
        }
    }
}
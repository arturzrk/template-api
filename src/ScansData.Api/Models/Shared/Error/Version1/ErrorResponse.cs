namespace ScansData.Api.Models.Shared.Error.Version1
{
    public class ErrorResponse
    {
        public ErrorResponse(): this( "UnexpectedError", "An unexpected error has occurred." ) { }

        public ErrorResponse( string errorCode, string errorMessage )
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public string ErrorCode { get; }
        public string ErrorMessage { get; }
    }
}
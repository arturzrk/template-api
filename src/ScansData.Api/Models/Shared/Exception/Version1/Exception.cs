namespace Template.Api.Models.Shared.Exception.Version1
{
    using System;

    public class Exception
    {
        public Exception( System.Exception exception )
        {
            Message = exception.Message;
            StackTrace = exception.StackTrace;

            if( exception.InnerException != null )
            {
                InnerException = new Exception( exception.InnerException );
            }
        }

        public string Message { get; }
        public string StackTrace { get; }
        public Exception InnerException { get; }
    }
}
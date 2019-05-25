namespace ScansData.Api.Common.Exceptions
{
    using System;
    using System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Http;

    public interface IExceptionHandler<in TException> where TException: Exception
    {
        Task HandleAsync( HttpContext context, TException exception );
    }
}
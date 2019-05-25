namespace ScansData.Api.Common.Exceptions
{
    using System;
    using System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Http;

    public interface IDefaultExceptionHandler
    {
        Task HandleAsync( HttpContext context, Exception exception );
    }
}
namespace ScansData.Api.ExceptionHandlers.Default.Version1
{
    using System.Net;
    using System.Threading.Tasks;
    using Common.Exceptions;
    using Common.Microsoft.AspNetCore.Http;
    using Common.Newtonsoft.Json;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Models.Shared.Error.Version1;
    using Models.Shared.Exception.Version1;

    public class DefaultExceptionHandler: IDefaultExceptionHandler
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public DefaultExceptionHandler( IHostingEnvironment hostingEnvironment )
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task HandleAsync( HttpContext context, System.Exception exception )
        {
            ErrorResponse response;

            if( _hostingEnvironment.IsDevelopment() )
            {
                response = new ExceptionResponse
                    (
                        new Models.Shared.Exception.Version1.Exception( exception )
                    );
            }
            else
            {
                response = new ErrorResponse();
            }

            await context.Response.WriteJsonAsync( HttpStatusCode.InternalServerError, response,
                                                   JsonConstants.JsonSerializerSettings );
        }
    }
}
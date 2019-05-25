namespace ScansData.Api
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Autofac;
    using Common.Autofac;
    using Common.Exceptions;
    using Common.Microsoft.AspNetCore.Http;
    using Common.Newtonsoft.Json;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.Shared.Exception.Version1;
    using Versioning;
    using Exception = Models.Shared.Exception.Version1.Exception;

    public class JsonExceptionsMiddleware
    {
        private readonly IComponentContext componentContext;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger<JsonExceptionsMiddleware> logger;
        private readonly RequestDelegate next;
        private static readonly Type SExceptionHandlerInterfaceType;

        static JsonExceptionsMiddleware()
        {
            SExceptionHandlerInterfaceType = typeof( IExceptionHandler<> );
        }

        public JsonExceptionsMiddleware(
            RequestDelegate next,
            IComponentContext componentContext,
            IHostingEnvironment hostingEnvironment,
            ILogger<JsonExceptionsMiddleware> logger )
        {
            this.next = next;
            this.componentContext = componentContext;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        public async Task Invoke( HttpContext context )
        {
            try
            {
                logger.LogInformation( "Starting API request" );
                await next( context );
                logger.LogInformation( "Finishing API request" );
            }
            catch( AggregateException exception )
            {
                var innerExceptions = exception.InnerExceptions.GroupBy( e => e.GetType() )
                                               .Select( g => g.Last() )
                                               .ToList();

                foreach( var innerException in innerExceptions.Take( innerExceptions.Count - 1 ) )
                {
                    var innerExceptionName = innerException.GetType().FullName;

                    logger.LogWarning
                        (
                            exception,
                            "Exception {ExceptionName} thrown with message {ExceptionMessage}",
                            innerExceptionName,
                            innerException.Message
                        );
                }

                await HandleException( context, innerExceptions.Last() );
            }
            catch( System.Exception exception )
            {
                await HandleException( context, exception );
            }
        }

        private async Task HandleException( HttpContext context, System.Exception originalException )
        {
            if( context.Response.HasStarted )
            {
                var exceptionName = originalException.GetType().FullName;

                logger.LogError
                    (
                        originalException,
                        "The response has already started so exception {ExceptionName} with message {ExceptionMessage} cannot be handled",
                        exceptionName,
                        originalException.Message
                    );

                return;
            }

            context.Response.Clear();

            var exceptionHandlerType = SExceptionHandlerInterfaceType.MakeGenericType( originalException.GetType() );
            dynamic exceptionHandler;

            try
            {
                exceptionHandler = componentContext.Resolve( exceptionHandlerType );
            }
            catch( System.Exception exception )
            {
                var exceptionName = exception.GetType().FullName;
                var originalExceptionName = originalException.GetType().FullName;

                logger.LogWarning
                    (
                        exception,
                        "Exception {ExceptionName} thrown with message {ExceptionMessage} when resolving exception handler for exception {OriginalExceptionName} with message {OriginalExceptionMessage}",
                        exceptionName,
                        exception.Message,
                        originalExceptionName,
                        originalException.Message
                    );

                await HandleUsingDefaultExceptionHandler( context, originalException );

                return;
            }

            var handleMethod = exceptionHandlerType.GetTypeInfo().GetMethod( "HandleAsync" );

            try
            {
                await handleMethod.Invoke( exceptionHandler, new object[] { context, originalException } );
            }
            catch( System.Exception exception )
            {
                var exceptionName = exception.GetType().FullName;
                var originalExceptionName = originalException.GetType().FullName;

                logger.LogError
                    (
                        exception,
                        "Exception {ExceptionName} thrown with message {ExceptionMessage} when handling exception {OriginalExceptionName} with message {OriginalExceptionMessage}",
                        exceptionName,
                        exception.Message,
                        originalExceptionName,
                        originalException.Message
                    );

                if( hostingEnvironment.IsDevelopment() )
                {
                    var response = new ExceptionResponse
                        (
                            "ExceptionHandlerThrewException",
                            $"Exception {exceptionName} thrown with message {exception.Message} when handling exception {originalExceptionName} with message {originalException.Message}",
                            new Exception( exception )
                        );

                    await context.Response.WriteJsonAsync( HttpStatusCode.InternalServerError, response, JsonConstants.JsonSerializerSettings );
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
        }

        private async Task HandleUsingDefaultExceptionHandler( HttpContext context, System.Exception originalException )
        {
            var originalExceptionName = originalException.GetType().FullName;

            if( originalException.GetType() == typeof( TimeoutException ) )
            {
                logger.LogCritical
                    (
                        originalExceptionName,
                        "Unhandled exception {ExceptionName} thrown with message {ExceptionMessage}",
                        originalExceptionName,
                        originalException.Message
                    );
            }
            else
            {
                logger.LogError
                    (
                        originalException,
                        "Unhandled exception {ExceptionName} thrown with message {ExceptionMessage}",
                        originalExceptionName,
                        originalException.Message
                    );
            }

            var version = context.GetRequestedApiVersion()?.MajorVersion ?? Versions.Latest;

            IDefaultExceptionHandler defaultExceptionHandler;

            try
            {
                defaultExceptionHandler = componentContext.ResolveVersioned<IDefaultExceptionHandler>( version );
            }
            catch( System.Exception exception )
            {
                var exceptionName = exception.GetType().FullName;

                logger.LogError
                    (
                        exception,
                        "Exception {ExceptionName} thrown with message {ExceptionMessage} when resolving default exception handler for exception {OriginalExceptionName} with message {OriginalExceptionMessage} for version {Version}",
                        exceptionName,
                        exception.Message,
                        originalExceptionName,
                        originalException.Message,
                        version
                    );

                if( hostingEnvironment.IsDevelopment() )
                {
                    var response = new ExceptionResponse
                        (
                            "DefaultExceptionHandlerNotRegistered",
                            $"Exception {exceptionName} thrown with message {exception.Message} when resolving default exception handler for exception {originalExceptionName} with message {originalException.Message} for version {version}",
                            new Exception( exception )
                        );

                    await context.Response.WriteJsonAsync( HttpStatusCode.InternalServerError, response, JsonConstants.JsonSerializerSettings );
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

                return;
            }

            try
            {
                await defaultExceptionHandler.HandleAsync( context, originalException );
            }
            catch( System.Exception exception )
            {
                var exceptionName = exception.GetType().FullName;

                logger.LogError
                    (
                        exception,
                        "Exception {ExceptionName} thrown with message {ExceptionMessage} when handling exception {OriginalExceptionName} with message {OriginalExceptionMessage}",
                        exceptionName,
                        exception.Message,
                        originalExceptionName,
                        originalException.Message
                    );

                if( hostingEnvironment.IsDevelopment() )
                {
                    var response = new ExceptionResponse
                        (
                            "DefaultExceptionHandlerThrewException",
                            $"Exception {exceptionName} thrown with message {exception.Message} when handling exception {originalExceptionName} with message {originalException.Message}",
                            new Exception( exception )
                        );

                    await context.Response.WriteJsonAsync( HttpStatusCode.InternalServerError, response, JsonConstants.JsonSerializerSettings );
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
        }
    }
}
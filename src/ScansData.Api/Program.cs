namespace ScansData.Api
{
    using System;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Core.Enrichers;
    using Serilog.Events;
    using Serilog.Exceptions;

    public class Program
    {
        public static void Main( string[] args )
        {
            Console.Title = "ScansData.Api";

            BuildWebHost( args ).Run();
        }

        public static IWebHost BuildWebHost( string[] args ) =>
            WebHost.CreateDefaultBuilder( args )
                   .UseSerilog( ( hostingContext, loggerConfiguration ) =>
                       {
                           loggerConfiguration.MinimumLevel
                                              .Is( hostingContext.Configuration.GetValue<LogEventLevel>( "Serilog:LogEventLevel" ) )
                                              .Enrich.FromLogContext()
                                              .Enrich.With( new PropertyEnricher( "Component", "ScansData.Api" ) )
                                              .Enrich.With( new PropertyEnricher( "Environment", hostingContext.HostingEnvironment.EnvironmentName ) )
                                              .Enrich.WithExceptionDetails()
                                              .WriteTo.Console( hostingContext.Configuration.GetValue<LogEventLevel>( "Serilog:LogEventLevel" ) );

                           var seqUrl = hostingContext.Configuration.GetValue<string>( "Seq:Url" );

                           if( !string.IsNullOrWhiteSpace( seqUrl ) )
                           {
                               loggerConfiguration.WriteTo.Seq
                                   (
                                       seqUrl,
                                       apiKey: hostingContext.Configuration.GetValue<string>( "Seq:ApiKey" ),
                                       restrictedToMinimumLevel: hostingContext.Configuration.GetValue<LogEventLevel>( "Serilog:LogEventLevel" )
                                   );
                           }
                       }
                   )
                   .UseStartup<Startup>()
                   .Build();
    }
}

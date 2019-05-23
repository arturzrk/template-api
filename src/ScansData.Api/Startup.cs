namespace Template.Api
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Authentication;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Common.Newtonsoft.Json;
    using Common.Swashbuckle.AspNetCore.SwaggerGen;
    using FluentValidation;
    using FluentValidation.AspNetCore;
    using IdentityServer4.AccessTokenValidation;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    using Swashbuckle.AspNetCore.Swagger;
    using Versioning;

    public class Startup
    {
        private static readonly Assembly assembly;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        static Startup()
        {
            assembly = typeof(Startup).GetTypeInfo().Assembly;
        }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;

            ValidatorOptions.DisplayNameResolver = ValidatorOptions.PropertyNameResolver;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddApiVersioning( o =>
            {
                o.ApiVersionReader = new AcceptHeaderApiVersionReader();
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion( Versions.Latest, 0 );
            } );

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.ApiName = "Template.Api";
                        options.Authority = _configuration.GetValue<string>("IdentityServer:Authority");
                        options.RequireHttpsMetadata = _environment.IsProduction();
                    });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            services.AddMvc( o =>
                    {
                        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                        o.Filters.Add( new AuthorizeFilter( policy ) );
                    } )
                    .AddFluentValidation( o =>
                    {
                        o.ValidatorFactoryType = typeof( ServiceProviderValidatorFactory );
                        o.RegisterValidatorsFromAssembly( assembly );
                    } )
                    .AddJsonOptions( o =>
                    {
                        o.SerializerSettings.Converters = JsonConstants.JsonSerializerSettings.Converters;
                        o.SerializerSettings.Formatting = JsonConstants.JsonSerializerSettings.Formatting;
                    } );

            services.AddMvcCore()
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .AddVersionedApiExplorer( o => o.GroupNameFormat = "'v'VVV" );

            services.AddRouting(o =>
            {
                o.AppendTrailingSlash = false;
                o.LowercaseUrls = true;
            });

            AddSwagger(services);

            var autofacServiceProvider = BuildAutofacServiceProvider( services );
            return autofacServiceProvider;
        }

        private void AddSwagger( IServiceCollection services )
        {
            var identityServerAuthority = _configuration.GetValue<string>( "IdentityServer:Authority" );

            services.AddSwaggerGen(o =>
            {
                o.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    AuthorizationUrl = $"{identityServerAuthority}connect/authorize",
                    Flow = "implicit",
                    TokenUrl = $"{identityServerAuthority}connect/token",
                    Scopes = new Dictionary<string, string>
                    {
                        {"Template.Api", "Template API"}
                    }
                });
                o.CustomSchemaIds(x => x.FullName);
                o.DescribeAllEnumsAsStrings();
                o.OperationFilter<AuthorizeOperationFilter>( "Template.Api" );
                o.OperationFilter<CancellationTokenOperationFilter>();

                if (_environment.IsDevelopment())
                {
                    o.OperationFilter<SubHeaderOperationFilter>();
                }

                o.OperationFilter<SwaggerDefaultValuesOperationFilter>();

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = assembly.GetName().Name + ".xml";
                var xmlCommentsFilePath = Path.Combine( basePath, fileName );
                if (File.Exists(xmlCommentsFilePath))
                {
                    o.IncludeXmlComments(xmlCommentsFilePath);
                }

                var buildServiceProvider = services.BuildServiceProvider()
                                                   .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var apiVersionDescription in buildServiceProvider.ApiVersionDescriptions)
                {
                    o.SwaggerDoc(apiVersionDescription.GroupName, CreateInfoForApiVersion(apiVersionDescription));
                }
            });
        }

        private static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info
            {
                Title = $"Template Api {description.ApiVersion}",
                Version = description.ApiVersion.ToString()
            };

            if (description.IsDeprecated)
            {
                info.Description += " (deprecated)";
            }

            return info;
        }

        private static IServiceProvider BuildAutofacServiceProvider( IServiceCollection services )
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterAssemblyModules(assembly);
            return new AutofacServiceProvider(containerBuilder.Build());
        }

        public void Configure(IApplicationBuilder app,IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            app.UseMiddleware<JsonExceptionsMiddleware>();

            if( _environment.IsDevelopment() )
            {
                app.UseMiddleware<SubHeaderMiddleware>();
            }

            app.UseSwagger();
            app.UseSwaggerUI( o =>
            {
                o.OAuthClientId( "Template.Api.Swagger" );
                o.OAuthClientSecret( "f4da10f1-33db-453e-8334-86182f68644b" );
                o.OAuthAppName( "Template Api Swagger" );

                foreach( var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions )
                {
                    o.SwaggerEndpoint( $"/swagger/{apiVersionDescription.GroupName}/swagger.json",
                                       apiVersionDescription.GroupName.ToUpperInvariant() );
                }
            } );

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
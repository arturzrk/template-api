namespace ScansData.Api.ExceptionHandlers
{
    using System;
    using System.Linq;
    using Autofac;
    using Common.Exceptions;
    using Versioning;

    public class ExceptionHandlersModule: Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
            {
                builder.RegisterAssemblyTypes( assembly )
                       .As( t => t.GetInterfaces()
                                  .Where( i => i.IsClosedTypeOf( typeof( IExceptionHandler<> ) ) ) )
                       .SingleInstance();

                builder.RegisterAssemblyTypes( assembly )
                       .Where( t => typeof( IDefaultExceptionHandler ).IsAssignableFrom( t ) &&
                                    Versions.IsInVersionedNamespace( t ) )
                       .Keyed<IDefaultExceptionHandler>( t => Versions.GetVersionFromNamespace( t ) )
                       .SingleInstance();
            }
        }
    }
}
namespace ScansData.Api.Cqs.Commands
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using Autofac.Core;

    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddCommandDispatcher(this ContainerBuilder extended)
        {
            extended.RegisterType<AutofacCommandDispatcher>()
                .As<ICommandDispatcher>()
                .InstancePerDependency();

            return extended;
        }

        public static ContainerBuilder AddCommandHandlerDecorators(this ContainerBuilder extended)
        {
            extended.RegisterGenericDecorator
                (
                    typeof(LoggingCommandHandlerDecorator<,>),
                    typeof(ICommandHandler<,>),
                    "CommandHandler"
                )
                .InstancePerDependency();

            return extended;
        }

        public static ContainerBuilder AddCommandHandlers(this ContainerBuilder extended)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AddCommandHandlers(assembly);
            }

            void AddCommandHandlers(Assembly assembly)
            {
                extended.RegisterAssemblyTypes(assembly)
                    .As(t => t.GetInterfaces()
                        .Where(i => i.IsClosedTypeOf(typeof(ICommandHandler<,>)))
                        .Select(i => new KeyedService("CommandHandler", i)))
                    .InstancePerDependency();
            }

            return extended;
        }
    }
}
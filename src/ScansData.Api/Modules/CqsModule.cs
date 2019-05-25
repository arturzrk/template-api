namespace ScansData.Api.Modules
{
    using Autofac;
    using Cqs.Commands;
    using Cqs.Events;
    using Cqs.Queries;

    public class CqsModule: Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.AddCommandDispatcher()
                   .AddCommandHandlerDecorators()
                   .AddCommandHandlers();

            builder.AddQueryDispatcher()
                   .AddQueryHandlerDecorators()
                   .AddQueryHandlers();

            builder.AddEventDispatcher()
                   .AddEventHandlerDecorators()
                   .AddEventHandlers();
        }
    }
}
namespace ScansData.Api.Common.Autofac
{
    using global::Autofac;

    public static class ComponentContextExtensions
    {
        public static TService ResolveVersioned<TService>( this IComponentContext componentContext, int version )
        {
            var service = default(TService);

            while( version > 0 && service == null )
            {
                service = componentContext.ResolveKeyed<TService>( version );
            }

            return service;
        }
    }
}
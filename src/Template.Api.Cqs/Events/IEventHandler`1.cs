﻿namespace Template.Api.Cqs.Events
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEventHandler<in TEvent>
        where TEvent: IEvent
    {
        Task HandleAsync( TEvent @event, CancellationToken cancellationToken );
    }
}
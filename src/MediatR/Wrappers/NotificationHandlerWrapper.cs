using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Wrappers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class NotificationHandlerWrapper
{
    public abstract Task Handle(INotification notification, IServiceProvider serviceFactory,
        Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task> publish,
        CancellationToken cancellationToken);
}

public class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
    where TNotification : INotification
{
    public override Task Handle(INotification notification, IServiceProvider serviceFactory,
        Func<IEnumerable<Func<INotification, CancellationToken, Task>>, INotification, CancellationToken, Task> publish,
        CancellationToken cancellationToken)
    {
        var handlers = serviceFactory
            .GetServices<INotificationHandler<TNotification>>()
            .Select(static x => new Func<INotification, CancellationToken, Task>((theNotification, theToken) => x.Handle((TNotification)theNotification, theToken)));

        return publish(handlers, notification, cancellationToken);
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.NotificationHandling.Extensions
{
    internal class NotificationHandlerDecorator<H, N> : INotificationHandler<N>, IDisposable
        where H : class, INotificationHandler<N>
        where N : INotification
    {
        private H innerHandler;
        private IServiceProvider serviceProvider;

        public NotificationHandlerDecorator(H innerHandler, IServiceProvider serviceProvider)
        {
            this.innerHandler = innerHandler;
            this.serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            innerHandler = null;
            serviceProvider = null;
        }

        public Task Handle(N notification, CancellationToken cancellationToken)
        {
            Task resultTask;

            try
            {
                resultTask = innerHandler.Handle(notification, cancellationToken);

                resultTask.Wait();
            }
            finally
            {

                if (serviceProvider is IServiceScope scope)
                    scope.Dispose();
            }

            return resultTask;
        }
    }
}

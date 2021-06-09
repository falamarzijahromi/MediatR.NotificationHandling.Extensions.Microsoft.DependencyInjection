using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System;
using MediatR;
using MediatR.NotificationHandling.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions to configure MediatR cotification handling in separate dependency scopes.
    /// After calling AddMediatR you can call extension methods
    /// There are three pre conditions for using extenion methods:
    /// - IMediatR must be registered with scoped lifestyle
    /// - Microsoft runtime implemetation of IServiceProvider must be IServiceScope too.
    /// - Extension methods must be called after all notification handlers registered.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure MediatR cotification handling in separate dependency scopes.
        /// This extension methods is better to be called after AddMediatR method
        /// </summary>
        /// <example>
        /// <code>
        /// services.AddMediatR(...);
        /// ...
        /// service.IsolateMediatRNotificationHandlingScopes();
        /// </code>
        /// </example>
        /// <param name="services">Service collection</param>       
        /// <returns>Service collection</returns>
        public static IServiceCollection IsolateMediatRNotificationHandlingScopes(this IServiceCollection services)
        {
            return IsolateNotificationHandlingScopes(services);
        }

        /// <summary>
        /// Configure MediatR cotification handling in separate dependency scopes.
        /// This extension methods is better to be called exactly with AddMediatR method
        /// </summary>
        /// <example>
        /// <code>
        /// services.AddMediatR(...).IsolateNotificationHandlingScopes()
        /// </code>
        /// </example>
        /// <param name="services">Service collection</param>       
        /// <returns>Service collection</returns>

        public static IServiceCollection IsolateNotificationHandlingScopes(this IServiceCollection services)
        {
            CheckPreConditions(services);

            Manipuldate_NotificationHandler_Registrations(services);

            RegistrarInterceptionDependencies(services);

            return services;
        }


        private static void CheckPreConditions(IServiceCollection services)
        {
            Check_IMediatR_Registered_As_Scoped_Service(services);

            Check_IServiceProvider_Is_IServiceScope(services);
        }

        private static void Check_IMediatR_Registered_As_Scoped_Service(IServiceCollection services)
        {
            var mediatrDescriptor = services.SingleOrDefault(desc => desc.ServiceType.Equals(typeof(IMediator)));

            if (mediatrDescriptor is null)
                throw new NotSupportedException("AddMediatR must be called befor using this extension method");

            if (mediatrDescriptor.Lifetime != ServiceLifetime.Scoped)
                throw new NotSupportedException("IMediatR must be registered with scoped lifstyle");
        }

        private static void Check_IServiceProvider_Is_IServiceScope(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var scope = serviceProvider.CreateScope();

            var provider = scope.ServiceProvider.GetRequiredService<IServiceProvider>();

            if (!provider.Equals(scope))
            {
                throw new NotSupportedException("IService Provider Must be IServiceScope");
            }

            scope.Dispose();

            serviceProvider.Dispose();
        }


        private static void Manipuldate_NotificationHandler_Registrations(IServiceCollection services)
        {
            var notificationHandlerDescs = services.Where(desc => IsInotificationHandler(desc.ServiceType)).ToList();

            foreach (var serviceDescriptor in notificationHandlerDescs)
            {
                var notificationType = serviceDescriptor.ServiceType.GenericTypeArguments[0];

                var handlerDecoratorType = typeof(NotificationHandlerDecorator<,>).MakeGenericType(serviceDescriptor.ImplementationType, notificationType);

                services.Remove(serviceDescriptor);

                services.AddScoped(serviceDescriptor.ServiceType, sp => CreateDecorator(sp, handlerDecoratorType));

                services.TryAddScoped(handlerDecoratorType);

                services.TryAddScoped(serviceDescriptor.ImplementationType);
            }
        }

        private static bool IsInotificationHandler(Type serviceType)
        {
            var handlerType = typeof(INotificationHandler<>);

            return serviceType.IsGenericType && serviceType.Name == handlerType.Name && serviceType.Namespace == handlerType.Namespace;
        }

        private static object CreateDecorator(IServiceProvider sp, Type decoratorType)
        {
            var scope = sp.CreateScope();

            var proxy = scope.ServiceProvider.GetRequiredService(decoratorType);

            return proxy;
        }


        private static void RegistrarInterceptionDependencies(IServiceCollection services)
        {
            services.AddScoped(typeof(NotificationHandlerDecorator<,>));
        }
    }
}

﻿using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using System.Linq;
    using Shouldly;
    using Xunit;

    public class DuplicateAssemblyResolutionTests
    {
        private readonly IServiceProvider _provider;

        public DuplicateAssemblyResolutionTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(new Logger());
            services
                .AddMediatR(config => config.AsScoped(), typeof(Ping), typeof(Ping))
                .IsolateNotificationHandlingScopes();
            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ShouldResolveNotificationHandlersOnlyOnce()
        {
            _provider.GetServices<INotificationHandler<Pinged>>().Count().ShouldBe(3);
        }
    }
}
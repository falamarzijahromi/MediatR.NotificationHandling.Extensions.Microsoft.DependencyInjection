﻿using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Extensions.Microsoft.DependencyInjection.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Shouldly;
    using Xunit;

    public class TypeResolutionTests
    {
        private readonly IServiceProvider _provider;

        public TypeResolutionTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(new Logger());
            services
                .AddMediatR(config => config.AsScoped(), typeof(Ping))
                .IsolateNotificationHandlingScopes();
            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ShouldResolveMediator()
        {
            _provider.GetService<IMediator>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveSender()
        {
            _provider.GetService<ISender>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolvePublisher()
        {
            _provider.GetService<IPublisher>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveRequestHandler()
        {
            _provider.GetService<IRequestHandler<Ping, Pong>>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveVoidRequestHandler()
        {
            _provider.GetService<IRequestHandler<Ding, Unit>>().ShouldNotBeNull();
        }

        [Fact]
        public void ShouldResolveNotificationHandlers()
        {
            _provider.GetServices<INotificationHandler<Pinged>>().Count().ShouldBe(3);
        }

        [Fact]
        public void ShouldResolveFirstDuplicateHandler()
        {
            _provider.GetService<IRequestHandler<DuplicateTest, string>>().ShouldNotBeNull();
            _provider.GetService<IRequestHandler<DuplicateTest, string>>()
                .ShouldBeAssignableTo<DuplicateHandler1>();
        }

        [Fact]
        public void ShouldResolveIgnoreSecondDuplicateHandler()
        {
            _provider.GetServices<IRequestHandler<DuplicateTest, string>>().Count().ShouldBe(1);
        }
    }
}
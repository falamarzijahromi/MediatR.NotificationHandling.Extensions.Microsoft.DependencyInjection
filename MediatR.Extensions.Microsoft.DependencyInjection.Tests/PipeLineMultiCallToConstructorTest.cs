﻿using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Extensions.Microsoft.DependencyInjection.Tests
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;

    public class PipelineMultiCallToConstructorTests
    {
        public class ConstructorTestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        {
            private readonly Logger _output;

            public ConstructorTestBehavior(Logger output) => _output = output;

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
            {
                _output.Messages.Add("ConstructorTestBehavior before");
                var response = await next();
                _output.Messages.Add("ConstructorTestBehavior after");

                return response;
            }
        }

        public class ConstructorTestRequest : IRequest<ConstructorTestResponse>
        {
            public string Message { get; set; }
        }

        public class ConstructorTestResponse
        {
            public string Message { get; set; }
        }

        public class ConstructorTestHandler : IRequestHandler<ConstructorTestRequest, ConstructorTestResponse>
        {

            private static volatile object _lockObject = new object();
            private readonly Logger _logger;
            private static int _constructorCallCount;

            public static int ConstructorCallCount => _constructorCallCount;

            public static void ResetCallCount()
            {
                lock (_lockObject)
                {
                    _constructorCallCount = 0;
                }
            }

            public ConstructorTestHandler(Logger logger)
            {
                _logger = logger;
                lock (_lockObject)
                {
                    _constructorCallCount++;
                }
            }

            public Task<ConstructorTestResponse> Handle(ConstructorTestRequest request, CancellationToken cancellationToken)
            {
                _logger.Messages.Add("Handler");
                return Task.FromResult(new ConstructorTestResponse { Message = request.Message + " ConstructorPong" });
            }
        }

        [Fact]
        public async Task Should_not_call_constructor_multiple_times_when_using_a_pipeline()
        {
            ConstructorTestHandler.ResetCallCount();
            ConstructorTestHandler.ConstructorCallCount.ShouldBe(0);

            var output = new Logger();
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(output);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ConstructorTestBehavior<,>));
            services.AddMediatR(typeof(Ping).GetTypeInfo().Assembly);
            var provider = services.BuildServiceProvider();

            var mediator = provider.GetService<IMediator>();

            var response = await mediator.Send(new ConstructorTestRequest { Message = "ConstructorPing" });

            response.Message.ShouldBe("ConstructorPing ConstructorPong");

            output.Messages.ShouldBe(new[]
            {
                "ConstructorTestBehavior before",
                "First pre processor",
                "Next pre processor",
                "Handler",
                "First post processor",
                "Next post processor",
                "ConstructorTestBehavior after"
            });
            ConstructorTestHandler.ConstructorCallCount.ShouldBe(1);
        }
    }
}
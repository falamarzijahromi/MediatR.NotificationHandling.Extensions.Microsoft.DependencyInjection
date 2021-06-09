using System.Threading;
using MediatR;

namespace TestApp
{
    using System.IO;
    using System.Threading.Tasks;

    public class PingedHandler1 : INotificationHandler<Pinged>
    {
        private readonly TextWriter _writer;
        private readonly SomeService someService;

        public PingedHandler1(TextWriter writer, SomeService someService)
        {
            _writer = writer;
            this.someService = someService;
        }

        public async Task Handle(Pinged notification, CancellationToken cancellationToken)
        {
            await _writer.WriteLineAsync("Got pinged1 async.");
            await _writer.WriteLineAsync($"SomeService Hash Code: {someService.GetHashCode()}");
        }
    }

    public class PingedHandler2 : INotificationHandler<Pinged>
    {
        private readonly TextWriter _writer;
        private readonly SomeService someService;

        public PingedHandler2(TextWriter writer, SomeService someService)
        {
            _writer = writer;
            this.someService = someService;
        }

        public async Task Handle(Pinged notification, CancellationToken cancellationToken)
        {
            await _writer.WriteLineAsync("Got pinged2 async.");
            await _writer.WriteLineAsync($"SomeService Hash Code: {someService.GetHashCode()}");
        }
    }

    public class PongedHandler1 : INotificationHandler<Ponged>
    {
        private readonly TextWriter _writer;

        public PongedHandler1(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Handle(Ponged notification, CancellationToken cancellationToken)
        {
            return _writer.WriteLineAsync("Got ponged1 async.");
        }
    }

    public class PongedHandler2 : INotificationHandler<Ponged>
    {
        private readonly TextWriter _writer;

        public PongedHandler2(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Handle(Ponged notification, CancellationToken cancellationToken)
        {
            return _writer.WriteLineAsync("Got ponged2 async.");
        }
    }

    //public class ConstrainedPingedHandler<TNotification> : INotificationHandler<TNotification>
    //    where TNotification : Pinged
    //{
    //    private readonly TextWriter _writer;

    //    public ConstrainedPingedHandler(TextWriter writer)
    //    {
    //        _writer = writer;
    //    }

    //    public Task Handle(TNotification notification, CancellationToken cancellationToken)
    //    {
    //        return _writer.WriteLineAsync("Got pinged constrained async.");
    //    }
    //}

    public class PingedAlsoHandler : INotificationHandler<Pinged>
    {
        private readonly TextWriter _writer;

        public PingedAlsoHandler(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Handle(Pinged notification, CancellationToken cancellationToken)
        {
            return _writer.WriteLineAsync("Got pinged also async.");
        }
    }
}
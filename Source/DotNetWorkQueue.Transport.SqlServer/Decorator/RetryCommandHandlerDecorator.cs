﻿using DotNetWorkQueue.Transport.RelationalDatabase;
using DotNetWorkQueue.Transport.SqlServer.Basic;
using DotNetWorkQueue.Validation;
using Polly;

namespace DotNetWorkQueue.Transport.SqlServer.Decorator
{
    /// <inheritdoc />
    internal class RetryCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IPolicies _policies;
        private Policy _policy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryCommandHandlerDecorator{TCommand}" /> class.
        /// </summary>
        /// <param name="decorated">The decorated handler.</param>
        /// <param name="policies">The policies.</param>
        public RetryCommandHandlerDecorator(ICommandHandler<TCommand> decorated,
            IPolicies policies)
        {
            Guard.NotNull(() => decorated, decorated);
            Guard.NotNull(() => policies, policies);

            _decorated = decorated;
            _policies = policies;
        }

        /// <inheritdoc />
        public void Handle(TCommand command)
        {
            if (_policy == null)
            {
                _policies.Registry.TryGet(TransportPolicyDefinitions.RetryCommandHandler, out _policy);
            }
            if (_policy != null)
            {
                _policy.Execute(() => _decorated.Handle(command));
            }
            else //no policy found
                _decorated.Handle(command);
        }
    }
}

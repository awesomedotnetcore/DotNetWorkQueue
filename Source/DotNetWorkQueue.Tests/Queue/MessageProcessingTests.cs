﻿// ---------------------------------------------------------------------
//This file is part of DotNetWorkQueue
//Copyright © 2017 Brian Lehnen
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// ---------------------------------------------------------------------
using DotNetWorkQueue.Queue;
using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Xunit;
namespace DotNetWorkQueue.Tests.Queue
{
    public class MessageProcessingTests
    {
        [Fact]
        public void Handle()
        {
            var wrapper = new MessageProcessingWrapper();
            var test = wrapper.Create();
            test.Handle();
            wrapper.MessageContextFactory.Received(1).Create();
        }

        [Fact]
        public void Handle_Receive_Message()
        {
            var wrapper = new MessageProcessingWrapper();
            var test = wrapper.Create();
            test.Handle();
            wrapper.ReceiveMessages.ReceivedWithAnyArgs(1).ReceiveMessage(null);
        }

        public class MessageProcessingWrapper
        {
            private readonly IFixture _fixture;
            public IReceiveMessages ReceiveMessages;
            public IMessageContextFactory MessageContextFactory;
            public MessageProcessingWrapper()
            {
                _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
                ReceiveMessages = _fixture.Create<IReceiveMessages>();
                var factory = _fixture.Create<IReceiveMessagesFactory>();
                factory.Create().ReturnsForAnyArgs(ReceiveMessages);
                _fixture.Inject(factory);
                MessageContextFactory = _fixture.Create<IMessageContextFactory>();
                _fixture.Inject(ReceiveMessages);
                _fixture.Inject(MessageContextFactory);
            }
            public MessageProcessing Create()
            {
                return _fixture.Create<MessageProcessing>();
            }
        }
    }
}

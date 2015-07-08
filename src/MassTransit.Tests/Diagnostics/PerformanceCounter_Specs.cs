﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.Diagnostics
{
    using System.Threading;
    using System.Threading.Tasks;
    using Monitoring.Performance;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Registering_a_performance_counter_observer :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_update_performance_counters()
        {
            _completed = GetTask<bool>();
            var receiveObserver = new PerformanceCounterReceiveObserver();

            var observerHandle = Bus.ConnectReceiveObserver(receiveObserver);

            for (int i = 0; i < 1000; i++)
            {
                await InputQueueSendEndpoint.Send(new PingMessage());
            }

            await _completed.Task;

            observerHandle.Disconnect();
        }

        TaskCompletionSource<bool> _completed;

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            long value = 0;
            configurator.Handler<PingMessage>(async context =>
            {
                if (Interlocked.Increment(ref value) == 1000)
                    _completed.TrySetResult(true);

            });
        }
    }
}
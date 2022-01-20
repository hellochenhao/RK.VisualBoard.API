using Microsoft.Extensions.Hosting;
using Rokin.Common.RabbitMQ.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VisualBoard_Interface
{
    public class MQCustomer : IHostedService
    {
        private readonly IRabbitMQHelper publish;

        public MQCustomer(IRabbitMQHelper publish)
        {
            this.publish = publish;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

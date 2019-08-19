using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace MvcControllerWithRabbitMQ
{
    public class RabbitConsumer : IHostedService
    {

        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly ILogger _logger;

        public RabbitConsumer(ILogger<RabbitConsumer> logger)
        {
            _logger = logger;
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest",
                    Port = 5672,
                };
                this.connection = factory.CreateConnection();
                this.channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitConsumer init error,ex:{ex.Message}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }

        public virtual bool Process(string message)
        {
            //throw new NotImplementedException();
            _logger.LogInformation($"RabbitConsumer:Process(),{message}");
            return true;
        }

        public void Register()
        {
            var queue = "hello";
            var exchange = "message";
            var routingKey = "florian";
            Console.WriteLine($"RabbitConsumer register,routeKey:{routingKey}");
            channel.ExchangeDeclare(exchange: exchange, type: "topic");
            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: queue, exchange: exchange, routingKey: routingKey);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var result = Process(message);
                if (result)
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: queue, consumer: consumer);
        }

        public void DeRegister()
        {
            this.connection.Close();
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.connection.Close();
            return Task.CompletedTask;
        }
    }

}



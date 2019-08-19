using System;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

namespace MvcControllerWithRabbitMQ
{
    public class RabbitProducer
    {

        private readonly IModel _channel;

        private readonly ILogger _logger;

        public RabbitProducer(ILogger<RabbitProducer> logger)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest",
                    Port = 5672,
                };
                var connection = factory.CreateConnection();
                _channel = connection.CreateModel();
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitProducer init fail");
            }
            _logger = logger;
        }

        public virtual void PushMessage(object message)
        {
            var queue = "hello";
            var exchange = "message";
            var routingKey = "florian";
            _logger.LogInformation($"PushMessage,{message}");
            _channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            _channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: null, body: body);
        }
    }
}
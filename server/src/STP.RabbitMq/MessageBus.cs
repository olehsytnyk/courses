using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Logging;
using STP.Interfaces.Messages;
using STP.Interfaces.Enums;
using System.Threading.Tasks;

namespace STP.RabbitMq
{
    public class MessageBus : IMessageBus
    {
        private readonly ConcurrentDictionary<string, Subscription> subscriptionInfoDictionary = new ConcurrentDictionary<string, Subscription>();
        private readonly HashSet<string> existingExchanges = new HashSet<string>();
        private readonly IConnectionService _persistentConnection;
        private readonly ILogger<MessageBus> _logger;
        private IModel _consumerChannel;
        public MessageBus(IConnectionService persistentConnection, ILogger<MessageBus> logger)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _consumerChannel = SetUpConsumerChannel();
        }
        private string ConvertToRabbitMqExchangeType(RabbitExchangeType customExchangeType)
        {
            switch (customExchangeType)
            {
                case RabbitExchangeType.DirectExchange:
                    return ExchangeType.Direct;
                case RabbitExchangeType.FanoutExchange:
                    return ExchangeType.Fanout;
                default:
                    throw new ArgumentException($"No corresponding RabbitMq exchange type exists for {customExchangeType}", nameof(RabbitExchangeType));
            }

        }
        private IModel SetUpConsumerChannel()
        {
            _logger.LogInformation("Starting setup of RabbitMQ consumer channel");
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            var consumerChannel = _persistentConnection.CreateModel();
            consumerChannel.BasicQos(prefetchSize: 0, prefetchCount: 5, global: false);
            consumerChannel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning("Recreating RabbitMQ consumer channel");
                _consumerChannel.Dispose();
                _consumerChannel = SetUpConsumerChannel();
            };
            return consumerChannel ?? SetUpConsumerChannel();
        }

        private void StartConsumingMessages(string queuename)
        {
            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += async (model, ea) =>
            {
                var dictionaryKey = ea.Exchange + ea.RoutingKey;
                _logger.LogInformation("Asking to handle the message ");
                // await HandleMessage(dictionaryKey, ea.Body);
                //private async Task HandleMessage(string dictionaryKey, byte[] message)
               // {
                    if (subscriptionInfoDictionary.TryGetValue(dictionaryKey, out Subscription subscription))
                    {
                        var messageHandler = subscription.MessageHandlerInstance;
                        if (!subscription.MessageResultInBytes)
                        {
                            var messageType = subscription.MessageType;
                            var json = Encoding.UTF8.GetString(ea.Body);
                            var messageToSend = (IMessage)JsonConvert.DeserializeObject(json, messageType);
                            await messageHandler.HandleAsync(messageToSend);
                        }
                        else await messageHandler.HandleAsync(ea.Body);
                    }
                //}
                _consumerChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                _logger.LogInformation("Message handled");
            };
            _consumerChannel.BasicConsume(queue: queuename, autoAck: false, consumer: consumer);
            _logger.LogInformation("Consumer channel is ready to handle messages ");
        }
        //private async Task HandleMessage(string dictionaryKey, byte[] message)
        //{
        //    if (subscriptionInfoDictionary.TryGetValue(dictionaryKey, out Subscription subscription))
        //    {
        //        var messageHandler = subscription.MessageHandlerInstance;
        //        if (!subscription.MessageResultInBytes)
        //        {
        //            var messageType = subscription.MessageType;
        //            var json = Encoding.UTF8.GetString(message);
        //            var messageToSend = (IMessage)JsonConvert.DeserializeObject(json, messageType);
        //            await messageHandler.HandleAsync(messageToSend);
        //        }
        //        else await messageHandler.HandleAsync(message);
        //    }
        //}

        public void Publish(IMessage messageToSend, string exchangeName, RabbitExchangeType exchangeType, string routingKey)
        {
            if (!existingExchanges.Contains(exchangeName))
            {
                CreateExchange(exchangeName, exchangeType);
                existingExchanges.Add(exchangeName);
            }
            var message = JsonConvert.SerializeObject(messageToSend);
            var body = Encoding.UTF8.GetBytes(message);
            var properties = _consumerChannel.CreateBasicProperties();
            properties.Persistent = true;
            _logger.LogInformation("Publishing message to RabbitMQ");
            _consumerChannel.BasicPublish(exchange: exchangeName,
                                  routingKey: routingKey,
                                  basicProperties: properties,
                                  body: body);

        }

        public void Subscribe<TMessage, TMessageHandler>(TMessageHandler handlerInstance, string exchangeName, RabbitExchangeType exchangeType, string routingKey)
            where TMessage : IMessage
            where TMessageHandler : IMessageHandler
        {
            if (!existingExchanges.Contains(exchangeName))
            {
                CreateExchange(exchangeName, exchangeType);
                existingExchanges.Add(exchangeName);
            }
            var dictionaryKey = exchangeName + routingKey;

            if (!subscriptionInfoDictionary.ContainsKey(dictionaryKey))
            {
                _logger.LogInformation("Subscribing to message");
                var queueName = _consumerChannel.QueueDeclare(durable: true, exclusive: true, autoDelete: true).QueueName;
                subscriptionInfoDictionary.TryAdd(dictionaryKey, new Subscription(queueName, handlerInstance));
                StartConsumingMessages(queueName);
                _consumerChannel.QueueBind(queue: queueName,
                                         exchange: exchangeName,
                                         routingKey: routingKey);
            }
        }
        public void Subscribe<TMessage, TMessageHandler>(TMessageHandler handlerInstance, string exchangeName,
            RabbitExchangeType exchangeType, string routingKey, bool messageResultInBytes)
           where TMessage : IMessage, new()
           where TMessageHandler : IMessageHandler
        {
            if (!existingExchanges.Contains(exchangeName))
            {
                CreateExchange(exchangeName, exchangeType);
                existingExchanges.Add(exchangeName);
            }
            var dictionaryKey = exchangeName + routingKey;
            var messageType = typeof(TMessage);
            if (!subscriptionInfoDictionary.ContainsKey(dictionaryKey))
            {
                var queueName = _consumerChannel.QueueDeclare(durable: true, exclusive: true, autoDelete: false).QueueName;
                subscriptionInfoDictionary.TryAdd(dictionaryKey, new Subscription(queueName, handlerInstance, messageType, messageResultInBytes));
                _logger.LogInformation("Subscribing to message {MessageName}", messageType.Name);
                StartConsumingMessages(queueName);
                _consumerChannel.QueueBind(queue: queueName,
                                          exchange: exchangeName,
                                          routingKey: routingKey);
            }
        }

        public void Unsubscribe<TMessage, TMessageHandler>(string exchangeName, string routingKey)
             where TMessage : IMessage
             where TMessageHandler : IMessageHandler
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            var dictionaryKey = exchangeName + routingKey;
            if (subscriptionInfoDictionary.TryGetValue(dictionaryKey, out Subscription subscription))
            {
                var queueName = subscription.QueueName;
                subscriptionInfoDictionary.TryRemove(dictionaryKey, out var noUse);
                _consumerChannel.QueueUnbind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);
            }
        }

        private void CreateExchange(string exchangeName, RabbitExchangeType localExchangeType)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            var exchangeType = ConvertToRabbitMqExchangeType(localExchangeType);
            _logger.LogInformation($"Creating exchange {exchangeName} of type {exchangeType} ");
            _consumerChannel.ExchangeDeclare(exchange: exchangeName,
                                        type: exchangeType,
                                        durable: true);

        }
        public void Dispose()
        {
            _logger.LogWarning("Disposing consumer channel ");
            _consumerChannel?.Dispose();
        }
    }
}


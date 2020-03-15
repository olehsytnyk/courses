using STP.Interfaces.Messages;
using System;
using System.Collections.Generic;

namespace STP.RabbitMq
{
    public sealed class Subscription
    {
        public string QueueName { get; }
        public IMessageHandler MessageHandlerInstance { get; }
        public Type MessageType { get; }
        public bool MessageResultInBytes { get; private set; } = true;

        public Subscription(string queueName, IMessageHandler messageHandlerInstance)
        {
            QueueName = queueName;
            MessageHandlerInstance = messageHandlerInstance;
        }
        public Subscription(string queueName, IMessageHandler messageHandlerInstance, Type messageType, bool messageResultInBytes)
        {
            QueueName = queueName;
            MessageHandlerInstance = messageHandlerInstance;
            MessageType = messageType;
            MessageResultInBytes = messageResultInBytes;
        }
    }
}

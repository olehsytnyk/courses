using STP.Interfaces.Enums;

namespace STP.Interfaces.Messages
{
    public interface IMessageBus
    {
        void Publish(IMessage message, string exchangeName, RabbitExchangeType exchangeType, string routingKey);

        void Subscribe<TMessage, TMessageHandler>(TMessageHandler handerInstance, string exchangeName, RabbitExchangeType exchangeType, string routingKey)
            where TMessage : IMessage
            where TMessageHandler : IMessageHandler;
        void Subscribe<TMessage, TMessageHandler>(TMessageHandler handerInstance, string exchangeName, RabbitExchangeType exchangeType, string routingKey, bool messageResultInBytes)
            where TMessage : IMessage, new()
            where TMessageHandler : IMessageHandler;
        void Unsubscribe<TMessage, TMessageHandler>(string exchangeName, string routingKey)
            where TMessage : IMessage
            where TMessageHandler : IMessageHandler;
    }
}


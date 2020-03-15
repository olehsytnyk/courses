using System.Threading.Tasks;

namespace STP.Interfaces.Messages
{
    public interface IMessageHandler
    {
        Task HandleAsync(byte[] message);
        Task HandleAsync(IMessage message);
    }
}
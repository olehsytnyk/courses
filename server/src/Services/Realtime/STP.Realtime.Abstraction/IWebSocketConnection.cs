using System;
using System.Threading;
using System.Threading.Tasks;

namespace STP.Realtime.Abstraction
{
    public interface IWebSocketConnection 
    {
        Guid Id { get; set; }
        string UserId { get; set; }
        string SessionId { get; set; }
        Task SendAsync(byte[] message, CancellationToken cancellationToken);
        Task SendAsync(string message, CancellationToken cancellationToken);
        Task ReceiveMessagesUntilCloseAsync();
    }
}

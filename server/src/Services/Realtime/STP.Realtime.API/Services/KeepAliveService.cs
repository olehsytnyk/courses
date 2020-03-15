using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using STP.Realtime.Abstraction;

namespace STP.Realtime.API.Services
{
    internal class KeepAliveService : IHostedService
    {
        private const string KEEPALIVE_MESSAGE = "KeepAlive";

        private readonly IWebSocketConnectionsManager _WebSocketConnectionsManager;

        private Task _heartbeatTask;

        private CancellationTokenSource _cancellationTokenSource;
        public KeepAliveService(IWebSocketConnectionsManager WebSocketConnectionsManager)
        {
            _WebSocketConnectionsManager = WebSocketConnectionsManager;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _heartbeatTask = HeartbeatAsync(_cancellationTokenSource.Token);

            return _heartbeatTask.IsCompleted ? _heartbeatTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_heartbeatTask != null)
            {
                _cancellationTokenSource.Cancel();

                await Task.WhenAny(_heartbeatTask, Task.Delay(-1, cancellationToken));

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _WebSocketConnectionsManager.SendToAllAsync(KEEPALIVE_MESSAGE, cancellationToken);
                //every 120 seconds it sends KEEP_ALIVE message
                await Task.Delay(TimeSpan.FromSeconds(120), cancellationToken);
            }
        }
    }
}

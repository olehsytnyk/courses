namespace STP.Realtime.Common.Options
{
    public class WebSocketConnectionsOptions
    {
        //
        // Summary:
        //     Sets or gets length of buffer of sendMessage
        public int? SendSegmentSize { get; set; }

        //
        // Summary:
        //     Sets or gets length of buffer of receivedMessage
        public int ReceivePayloadBufferSize { get; set; }

        public WebSocketConnectionsOptions()
        {
            ReceivePayloadBufferSize = 4 * 1024;
        }
    }
}

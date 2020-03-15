namespace STP.Realtime.Common.WebSocketMessages
{
    public enum SubsribeAction { Join, Leave };
    public enum Exchange { User, Market, Profile, Datafeed, Position, Order }
    public enum SubjectAction { Update, Add, Delete, UPL }
    public class SocketMessage
    {
        public string RequestId { get; set; }
        public SubsribeAction Action { get; set; }
        public Exchange Subject { get; set; }
        public string SubjectId { get; set; }
        public SubjectAction SubjectAction { get; set; }

        public string GetRoutingKey()
        {
            return SubjectAction.ToString() + ":" + SubjectId;
        }
        public string GetRoomKey()
        {
            return Subject.ToString()+SubjectAction.ToString() + SubjectId;
        }
    }
}

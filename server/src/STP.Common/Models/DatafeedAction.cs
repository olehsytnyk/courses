using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Common.Models
{
    public class DatafeedAction
    {
        public long MarketId;
        public string SessionId;
        public DatafeedAction (long _marketId,string _sessionId)
        {
            MarketId = _marketId;
            SessionId = _sessionId;
        }
    }
}

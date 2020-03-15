using STP.Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace STP.Messages
{
    public class UPLMessage:IMessage
    {
        public long PositionId { get; set; }
        public double UnrealizedProfitLoss { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace OpcExporter.Model
{
    public class OpcConversation
    {
        public OpcPacket IncomingPacket { get; set; }
        public OpcPacket OutgoingPacket { get; set; }

        public OpcConversation(OpcPacket incomingPacket, OpcPacket outgoingPacket)
        {
            this.IncomingPacket = incomingPacket;
            this.OutgoingPacket = outgoingPacket;
        }

        public OpcMessageType GetMessageType()
        {
            return OutgoingPacket.OpcMessage;
        }

        public ulong CalculateTimeDifference()
        {
            return OutgoingPacket.Time - IncomingPacket.Time;
        }
    }
}

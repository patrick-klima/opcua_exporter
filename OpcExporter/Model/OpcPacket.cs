using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using SharpPcap;

namespace OpcExporter.Model
{
    public enum Direction
    {
        Incoming,
        Outgoing
    }
    public enum OpcMessageType
    {
        ReadRequest = 631,
        ReadResponse = 634,
        BrowseRequest = 527,
        BrowseResponse = 530,
        CallRequest = 712,
        CallResponse = 715
    }

    public class OpcPacket
    {
        public OpcMessageType OpcMessage;
        public Direction Direction;
        public int SecuritySequenceNumber;
        public int Length;
        public ulong Time;

        private void parseSecuritySequenceNumberLength(byte[] data)
        {
            byte[] securitySequenceNumberLength = new byte[4];
            securitySequenceNumberLength[0] = data[70];
            securitySequenceNumberLength[1] = data[71];
            securitySequenceNumberLength[2] = data[72];
            securitySequenceNumberLength[3] = data[73];

            SecuritySequenceNumber = BitConverter.ToInt32(securitySequenceNumberLength);
        }

        private bool isOpcPacket(byte[] data)
        {
            bool retVal = false;

            try
            {
                byte[] identifier = new byte[3];
                identifier[0] = data[54];
                identifier[1] = data[55];
                identifier[2] = data[56];

                retVal = identifier.SequenceEqual(new byte[3] {0x4d, 0x53, 0x47});
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        private void parseDirection()
        {
            switch (OpcMessage)
            {
                case OpcMessageType.ReadRequest:
                    Direction = Direction.Incoming;
                    break;
                case OpcMessageType.ReadResponse:
                    Direction = Direction.Outgoing;
                    break;
                case OpcMessageType.BrowseRequest:
                    Direction = Direction.Incoming;
                    break;
                case OpcMessageType.BrowseResponse:
                    Direction = Direction.Outgoing;
                    break;
                case OpcMessageType.CallRequest:
                    Direction = Direction.Incoming;
                    break;
                case OpcMessageType.CallResponse:
                    Direction = Direction.Outgoing;
                    break;
            }
        }

        private bool parseMessageType(byte[] data)
        {
            byte[] message = new byte[2];
            message[0] = data[80];
            message[1] = data[81];

            short messageInt = BitConverter.ToInt16(message);
            Enum.TryParse<OpcMessageType>(messageInt.ToString(), out OpcMessage);

            return Enum.IsDefined(typeof(OpcMessageType), OpcMessage);
            
        }

        public OpcPacket Parse(RawCapture rawPacket)
        {
            Length = rawPacket.Data.Length;
            Time = rawPacket.Timeval.MicroSeconds;

            // Check if magic string 'MSG' is found in the payload
            if (!isOpcPacket(rawPacket.Data)) return null;
            if (!parseMessageType(rawPacket.Data)) return null;

            parseSecuritySequenceNumberLength(rawPacket.Data);
            parseDirection();

            return this;
        }
    }
}

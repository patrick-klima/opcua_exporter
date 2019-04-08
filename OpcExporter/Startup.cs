using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpcExporter.Model;
using PacketDotNet;
using RestSharp.Authenticators;
using Serilog;
using Serilog.Events;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;

namespace OpcExporter
{
    public class Startup
    {
        public Options ArgOptions { get; set; }

        private Prometheus Prom;

        public Startup(Options options)
        {
            ArgOptions = options;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Is(ArgOptions.Verbose ? LogEventLevel.Verbose : LogEventLevel.Error)
                .CreateLogger();

        }

        public void Run()
        {
            Prom = new Prometheus(ArgOptions.MetricPort);
            
            var devices = CaptureDeviceList.Instance;
            var device = devices[ArgOptions.InterfaceId];
            Log.Information("Selected device: {0}", device.Description);

            if (device is WinPcapDevice winPcap)
            {
                winPcap.Open(OpenFlags.DataTransferUdp, 1000);
                device = (WinPcapDevice)winPcap;
                Log.Information("Using winPcap driver (windows)");
            } else if (device is LibPcapLiveDevice libPcap)
            {
                libPcap.Open(DeviceMode.Normal, 1000);
                device = (LibPcapLiveDevice)libPcap;
                Log.Information("Using libPcap driver (linux)");
            }
            else
            {
                Log.Error("Unsupported device");
            }

            string filter = $"tcp and port {ArgOptions.Port}";
            device.Filter = filter;
            Log.Information("Using filter {0}", filter);

            device.OnPacketArrival += Device_OnPacketArrival;
            device.OnCaptureStopped += Device_OnCaptureStopped;

            device.StartCapture();
            Log.Information("Start Capturing");

            // Force program to stay open while waiting for events.
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private void Device_OnCaptureStopped(object sender, CaptureStoppedEventStatus status)
        {
            Log.Information("Stop Capturing");
        }

        private OpcPacket lastPacket;
        private void Device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;

            OpcPacket packet = new OpcPacket().Parse(e.Packet);
            if (packet != null)
            {
                Log.Information("OPC Packet Transfer. ID: {0} Type: {1} Direction: {2}", packet.SecuritySequenceNumber, packet.OpcMessage.ToString(), packet.Direction);

                if (ResponseToRequest(packet))
                {
                    var diff = packet.Time - lastPacket.Time;
                    if (diff > 100000) return;

                    OpcConversation conv = new OpcConversation(lastPacket, packet);
                    Prom.UpdateMetrics(conv);

                }

                lastPacket = packet;
                
            }
        }

        private bool ResponseToRequest(OpcPacket newPacket)
        {
            if (lastPacket != null)
            {
                // Was the previous packet a request and the new packet a response?
                if (lastPacket.Direction == Direction.Incoming && newPacket.Direction == Direction.Outgoing)
                {
                    // Are we sure they have the same ID?
                    if (lastPacket.SecuritySequenceNumber == newPacket.SecuritySequenceNumber)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

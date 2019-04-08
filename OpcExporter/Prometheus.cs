using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using OpcExporter.Model;
using Prometheus.Client;
using Prometheus.Client.MetricServer;
using RestSharp;
using Serilog;

namespace OpcExporter
{
    public class Prometheus
    {
        public MetricServer MetricServer { get; set; }

        public Gauge ReadRequest { get; set; }
        public Gauge BrowseRequest { get; set; }
        public Gauge CallRequest { get; set; }

        public Prometheus(int port)
        {
            this.MetricServer = new MetricServer(port);
            ReadRequest = Metrics.CreateGauge("opc_readrequest_latency", "Readrequest latency in microseconds");
            BrowseRequest = Metrics.CreateGauge("opc_browserequest_latency", "Browserequest latency in microseconds");
            CallRequest = Metrics.CreateGauge("opc_callrequest_latency", "Callrequest latency in microseconds");
            Log.Information("Start Metric server on port {0}", port);
            this.MetricServer.Start();
        }

        public void UpdateMetrics(OpcConversation conv)
        {
            string measurement = conv.GetMessageType().ToString();
            ulong timedifference = conv.CalculateTimeDifference();
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            switch (conv.GetMessageType())
            {
                case OpcMessageType.ReadResponse:
                    ReadRequest.Set(timedifference);
                    break;
                case OpcMessageType.BrowseResponse:
                    BrowseRequest.Set(timedifference);
                    break;
                case OpcMessageType.CallResponse:
                    CallRequest.Set(timedifference);
                    break;
            }
            ReadRequest.Set(timedifference);

        }

        ~Prometheus()
        {
            MetricServer.Stop();
        }
    }
}

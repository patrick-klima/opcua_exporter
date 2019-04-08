using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace OpcExporter
{
    public class Options
    {
        [Option("opc-port", Required = true, HelpText = "Set OPC Server Port")]
        public int Port { get; set; }

        [Option("interface", Required = true, HelpText = "Set pcap interface id")]
        public int InterfaceId { get; set; }

        [Option("verbose", Required = false, HelpText = "Set output to verbose")]
        public bool Verbose { get; set; }

        [Option("listen-port", Required = true, HelpText = "Port that will be used to expose metrics")]
        public int MetricPort { get; set; }
    }
}

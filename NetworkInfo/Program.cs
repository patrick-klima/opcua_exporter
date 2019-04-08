using System;
using SharpPcap;

namespace NetworkInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}c", ver);

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // Print out the devices
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.ReadKey();
        }
    }
}
2
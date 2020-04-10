using Buttplug.Client;
using Buttplug.Core;
using Buttplug.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vibrating
{
    public class Server
    {
        public static Process Hostprocess;

        public static void Main(string[] args)
        {
            Run(args).Wait();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine(args.Name);
            return null;
        }

        private static Buttplug.Server.Managers.XInputGamepadManager.XInputGamepadManager controller(IButtplugLogManager logger)
        {
            return new Buttplug.Server.Managers.XInputGamepadManager.XInputGamepadManager(logger);
        }

        public static async Task Run(string[] args)
        {
            //if (args.Length != 1)
            //{ 
            //    return;
            //}

            //Hostprocess = Process.GetProcessById(Convert.ToInt32(args[0]));
            var server = new ButtplugEmbeddedConnector("Example Server");
            server.Server.AddDeviceSubtypeManager(controller);
            var client = new ButtplugClient("Example Client", server);

            await client.ConnectAsync();
            void HandleDeviceAdded(object aObj, DeviceAddedEventArgs aArgs)
            {
                Console.Out.WriteLine($"Device connected: {aArgs.Device.Name}");
            }

            client.DeviceAdded += HandleDeviceAdded;

            void HandleDeviceRemoved(object aObj, DeviceRemovedEventArgs aArgs)
            {
                Console.Out.WriteLine($"Device disconnected: {aArgs.Device.Name}");
            }

            client.DeviceRemoved += HandleDeviceRemoved;

            async Task ScanForDevices()
            {
                Console.Out.WriteLine("Found devices will be printed to console.");
                await client.StartScanningAsync();
            }

            try
            {
                await ScanForDevices();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                Console.Out.WriteLine(errorMessage);
            }

            List<Task> DeviceSending = new List<Task>();
            while (true)
            {
                string line = await Console.In.ReadLineAsync();
                double strength = Convert.ToDouble(line);
                if (strength > 1f)
                {
                    //Console.WriteLine("too high strength: " + strength);
                    strength = 1f;
                }

                if (strength < 0f)
                {
                    //Console.WriteLine("too low strength: " + strength);
                    strength = 0f;
                }

                foreach (var device in client.Devices)
                {
                    var task = device.SendVibrateCmd(strength);
                    DeviceSending.Add(task);
                }

                await Console.Out.WriteLineAsync($"vibrating for {strength}");

                foreach (var item in DeviceSending)
                {
                    await item;
                }

                DeviceSending.Clear();
            }
        }
    }
}

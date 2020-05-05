using Buttplug.Client;
using Buttplug.Core;
using Buttplug.Core.Logging;
using Buttplug.Core.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Threading.Tasks;

namespace Vibrating
{
    public class Server
    {
        public static Process Hostprocess;

        public static async Task Main(string[] args)
        {
            await Run(args);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.Error.WriteLine(args.Name);
            return null;
        }

        private static Buttplug.Server.Managers.XInputGamepadManager.XInputGamepadManager XController(IButtplugLogManager logger)
        {
            return new Buttplug.Server.Managers.XInputGamepadManager.XInputGamepadManager(logger);
        }

        private static Buttplug.Server.Managers.HidSharpManager.HidSharpManager HidController(IButtplugLogManager logger)
        {
            return new Buttplug.Server.Managers.HidSharpManager.HidSharpManager(logger);
        }

        private static Buttplug.Server.Managers.UWPBluetoothManager.UWPBluetoothManager UwpController(IButtplugLogManager logger)
        {
            return new Buttplug.Server.Managers.UWPBluetoothManager.UWPBluetoothManager(logger);
        }

        private static Buttplug.Server.Managers.WinUSBManager.WinUSBManager UsbController(IButtplugLogManager logger)
        {
            return new Buttplug.Server.Managers.WinUSBManager.WinUSBManager(logger);
        }

        static void HandleDeviceAdded(object aObj, DeviceAddedEventArgs aArgs)
        {
            Console.Out.WriteLine($"Device connected: {aArgs.Device.Name}");
        }

        static void HandleDeviceRemoved(object aObj, DeviceRemovedEventArgs aArgs)
        {
            Console.Out.WriteLine($"Device disconnected: {aArgs.Device.Name}");
        }

        public static async Task Run(string[] args)
        {
            if (args.Length != 1)
            {
                return;
            }

            Hostprocess = Process.GetProcessById(Convert.ToInt32(args[0]));
            ButtplugClient client = await Setup();

            if (client == null)
            {
                Console.Error.WriteLine("setup error for RiskOfRumble");
                return;
            }

            Task[] tasks = { Reading(), Task.CompletedTask };
            double cachedValue = 0.0f;
            while (true)
            {
                if (Hostprocess.HasExited)
                {
                    return;
                }
                Task.WaitAny(tasks);
                if (tasks[0].IsCompleted)
                {
                    cachedValue = ((Task<double>)tasks[0]).Result;
                    tasks[0] = Task.Run(() => Reading());
                }
                if (tasks[1].IsCompleted)
                {
                    tasks[1] = Task.Run(() => Sending(client, cachedValue));
                }

                await Task.Delay(10);
            }
        }


        static async Task ScanForDevices(ButtplugClient client)
        {
            Console.Out.WriteLine("Found devices will be printed to console.");
            await client.StartScanningAsync();
        }

        private static async Task<double> Reading()
        {
            string line = await Console.In.ReadLineAsync();
            double strength = double.Parse(line, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo);
            if (strength > 1.0)
            {
#if DEBUG
                Console.WriteLine("too high strength: " + strength);
#endif
                strength = 1.0;
            }

            if (strength < 0.0)
            {
#if DEBUG
                Console.WriteLine("too low strength: " + strength);
#endif
                strength = 0.0;
            }
#if DEBUG
            Console.WriteLine($"vibrating for {strength}");
#endif
            return strength;
        }

        static List<Task> sendTasks = new List<Task>();
        private static async Task Sending(ButtplugClient client, double cachedValue)
        {
            Task.WaitAll(sendTasks.ToArray());
            foreach (var device in client.Devices)
            {
                var count = device.GetMessageAttributes<VibrateCmd>().FeatureCount;
                if (!count.HasValue)
                {
                    continue;
                }

                sendTasks.Add(device.SendVibrateCmd(cachedValue));
            }
        }

        private static async Task<ButtplugClient> Setup()
        {
            var server = new ButtplugEmbeddedConnector("Example Server");
            server.Server.AddDeviceSubtypeManager(XController);
            server.Server.AddDeviceSubtypeManager(UsbController);
            server.Server.AddDeviceSubtypeManager(HidController);

            SelectQuery sq = new SelectQuery("SELECT DeviceId FROM Win32_PnPEntity WHERE service='BthLEEnum'");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);
            if (searcher.Get().Count > 0)
            {
                Console.Out.WriteLine("bluetooth is enabled");
                server.Server.AddDeviceSubtypeManager(UwpController);
            }

            var client = new ButtplugClient("Example Client", server);

            try
            {
                await client.ConnectAsync();
            }
            catch (ButtplugClientConnectorException ex)
            {
                Console.Error.WriteLine($"Can't connect, exiting! Message: {ex.InnerException.Message}");
                return null;
            }
            catch(ButtplugHandshakeException ex)
            {
                Console.Error.WriteLine($"Handshake issue, exiting! Message: {ex.InnerException.Message}");
                return null;
            }

            client.DeviceAdded += HandleDeviceAdded;
            client.DeviceRemoved += HandleDeviceRemoved;

            try
            {
                await ScanForDevices(client);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Can't scan, exiting! Message: {ex.Message}");
            }

            return client;
        }
    }
}

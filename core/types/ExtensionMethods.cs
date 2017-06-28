using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AxisDeviceDiscoveryLib.core.types
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Used to provide cancellation possibility to any Async Methods returning a Task<T>
        /// </summary>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                        s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }
        
        public static List<deviceNetworkInfo> OrderByIPAscending(this List<deviceNetworkInfo> DevicesInfoList)
        {
            return DevicesInfoList.OrderBy(x => x.IPaddress.ToString().Replace(".", "")).ToList();
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);

        public static string getMACAddress(IPAddress IP)
        {
            IPAddress dst = IP; // the destination IP address

            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;

            if (SendARP(BitConverter.ToInt32(dst.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen) != 0)
                throw new InvalidOperationException("SendARP failed.");

            string[] str = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
                str[i] = macAddr[i].ToString("x2");

            return string.Join("", str);
        }

        #region network ports management
        //Maintains network ports used by UDPSockets centrally to avoid dubbel usage
        private static List<int> _networkPort = new List<int>();
        private static int _extraRandomSeed = 100;
        public static int getNewNetworkPort()
        {
            Random rnd = new Random((int)(System.DateTime.Now.Ticks + ++_extraRandomSeed));
            int port = rnd.Next(45000, 65000);

            while (_networkPort.Contains(port))
                port = rnd.Next(45000, 65000);

            _networkPort.Add(port);
            return port;
        }
        public static void releaseNetworkPort(int Port)
        {
            if (_networkPort.Contains(Port))
                _networkPort.Remove(Port);
        }
        #endregion

        /// <summary>
        /// Extension to get the device model name by sending a request to the UPNP service address
        /// </summary>
        /// <param name="UPNPServiceAddress">Usually Ex: "http://<ip>:49157/rootdesc1.xml", this is contained in the UPNP probe response message</param>
        /// <returns></returns>
        public static async Task get_device_infoAsync(this deviceNetworkInfo DeviceInfo)
        {
            if (!string.IsNullOrEmpty(DeviceInfo.UPNPServiceAddress))
            {
                string model = string.Empty;
                using (HttpClient request = new HttpClient())
                {
                    try
                    {
                        XDocument doc = XDocument.Parse(await request.GetStringAsync(DeviceInfo.UPNPServiceAddress));
                        model = doc.Root.Element("{urn:schemas-upnp-org:device-1-0}device").Element("{urn:schemas-upnp-org:device-1-0}modelName").Value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("UPNP_GET_DEVICE_INFO " + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                    }
                }
            
                DeviceInfo.Model = model;
            }
        }
    }


}

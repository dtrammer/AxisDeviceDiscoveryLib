using AxisDeviceDiscoveryLib.core.services;
using AxisDeviceDiscoveryLib.core.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace AxisDeviceDiscoveryLib.core.services
{
    //BUG : include a cancellationToken in the socket incoming reading process
    public class Discovery_Upnp : UDPMutlicast
    {
        public string ProbeMessage = "M-SEARCH * HTTP/1.1\r\n"
                                    + "HOST: 239.255.255.250:1900\r\n"
                                    + "MAN: \"ssdp:discover\"\r\n"
                                    + "MX: 3\r\n"
                                    + "ST: ssdp:all\r\n\r\n";

        private string _MACVendorFilterPrefix = "00408C|ACCC8E"; //Axis default
        public string MACVendorFilterPrefix { get { return _MACVendorFilterPrefix; } set { _MACVendorFilterPrefix = value; } }

        #region methods
        public Discovery_Upnp(List<networkInterface> Interfaces, eOnDiscoveryCompleted OnDiscoveryDoneCallback) : base(Interfaces , OnDiscoveryDoneCallback)
        {
            base._probeMessage = ProbeMessage;
            base.MulticastAddress = "239.255.255.250";
            base.MulticastPort = 1900;
        }

        protected async override void OnSearchCompleted(networkInterface Interface, IList<string> Responses)
        {
            string IPAddress;
            string serviceAddress;
            string MACAddress = "";
            foreach (string s in Responses)
            {
                //Check for MAC address
                if (Regex.IsMatch(s, MACVendorFilterPrefix, RegexOptions.Singleline))
                {
                    //extract IP example
                    serviceAddress = Regex.Match(s, @"(?<=LOCATION:).*?(?=\r\n)", RegexOptions.Singleline).Value;
                    IPAddress = Regex.Match(serviceAddress, @"(?<=http://).*?(?=:)").Value;
                    if(!string.IsNullOrEmpty(MACVendorFilterPrefix))
                        MACAddress = Regex.Match(s, "(?<=USN: uuid:Upnp-BasicDevice-1_0-).{12,12}", RegexOptions.Singleline).Value;
                    Interface.add_DeviceInfo(new deviceNetworkInfo() { IPAddress = IPAddress, MACAddress = MACAddress , Model = await get_device_info(serviceAddress) });
                }
            }

            base.NotifyResultProcessingDone();
        }

        /// <summary>
        /// Method used to extract the model name, by using a http call to the UPNP description page
        /// </summary>
        /// <param name="UPNPServiceAddress">Usually Ex: "http://<ip>:49157/rootdesc1.xml", this is contained in the UPNP probe response message</param>
        /// <returns></returns>
        private async Task<string> get_device_info(string UPNPServiceAddress)
        {
            string model = string.Empty;
            using (HttpClient request = new HttpClient())
            {
                try
                {
                    XDocument doc = XDocument.Parse(await request.GetStringAsync(UPNPServiceAddress));
                    model = doc.Root.Element("{urn:schemas-upnp-org:device-1-0}device").Element("{urn:schemas-upnp-org:device-1-0}modelName").Value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                }
            }
            return model;
        }
        #endregion
    }
}

using AxisDeviceDiscoveryLib.core.types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

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

        protected override void OnSearchCompleted(networkInterface Interface, IList<Tuple<IPAddress,string>> Responses)
        {
            string serviceAddress;
            string MACAddress = "";

            foreach (Tuple<IPAddress,string> s in Responses)
            {
                serviceAddress = Regex.Match(s.Item2, @"(?<=LOCATION:).*?(?=\r\n)", RegexOptions.Singleline).Value;
                Interface.add_DeviceInfo(new deviceNetworkInfo() { IPaddress = s.Item1, MACAddress = MACAddress, UPNPServiceAddress = serviceAddress });
            }

            base.NotifyResultProcessingDone();
        }

        #endregion
    }
}

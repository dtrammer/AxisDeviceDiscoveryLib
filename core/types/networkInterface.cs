using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AxisDeviceDiscoveryLib.core.types
{
    /// <summary>
    /// Class representing an active Network Interface on the current system
    /// </summary>
    public class networkInterface
    {
        public int Lanid;
        public NetworkInterfaceType type;
        public string IPAddress;
        public List<deviceNetworkInfo> DiscoveredDevices = new List<deviceNetworkInfo>();

        public void add_DeviceInfo(deviceNetworkInfo Info)
        {
            var existingInfo = DiscoveredDevices.Where(x => x.IPAddress == Info.IPAddress).FirstOrDefault();
            if (existingInfo == null)
                DiscoveredDevices.Add(Info);
            else
            {
                //See if other info could be populated based on results of different discovery services
                if (string.IsNullOrEmpty(existingInfo.MACAddress) && !string.IsNullOrEmpty(Info.MACAddress))
                    existingInfo.MACAddress = Info.MACAddress;
                if (string.IsNullOrEmpty(existingInfo.XAddress) && !string.IsNullOrEmpty(Info.XAddress))
                    existingInfo.XAddress = Info.XAddress;
            }
        }
    }
}

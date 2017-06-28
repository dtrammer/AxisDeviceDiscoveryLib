using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AxisDeviceDiscoveryLib.core.types
{
    /// <summary>
    /// Class representing an active Network Interface on the current system
    /// </summary>
    public class networkInterface
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private string _vendorMACFilter = "00408C|ACCC8E";
        public string VendorMACfilter { get { return _vendorMACFilter; } set { _vendorMACFilter = value; } }

        public int Lanid { get; set; }
        public NetworkInterfaceType type { get; set; }
        public string IPAddress { get; set; }
        public List<deviceNetworkInfo> DiscoveredDevices = new List<deviceNetworkInfo>();

        public void add_DeviceInfo(deviceNetworkInfo NewInfo)
        {
            this._lock.EnterWriteLock();

            var existingInfo = DiscoveredDevices.Where(x => x.IPaddress.ToString() == NewInfo.IPaddress.ToString()).FirstOrDefault();

            if (existingInfo == null)
            {
                //Resolve the mac address
                try
                {
                    NewInfo.MACAddress = ExtensionMethods.getMACAddress(NewInfo.IPaddress);
                }catch(Exception ex)
                {
                    NewInfo.MACAddress = "Could not resolve address";
                }
                if (!string.IsNullOrEmpty(VendorMACfilter) && Regex.IsMatch(NewInfo.MACAddress.ToUpper(), VendorMACfilter))
                    DiscoveredDevices.Add(NewInfo);

            }
            else
            {
                //Check if other info could be added
                if (string.IsNullOrEmpty(existingInfo.ONVIFXAddress) && !string.IsNullOrEmpty(NewInfo.ONVIFXAddress))
                    existingInfo.ONVIFXAddress = NewInfo.ONVIFXAddress;
                if (string.IsNullOrEmpty(existingInfo.UPNPServiceAddress) && !string.IsNullOrEmpty(NewInfo.UPNPServiceAddress))
                    existingInfo.UPNPServiceAddress = NewInfo.UPNPServiceAddress;
            }

            this._lock.ExitWriteLock();
        }

        public override string ToString()
        {
            return "Lanid : " + Lanid + " IPAddress : " + IPAddress + " DataLink type : " + type.ToString();
        }
    }
}

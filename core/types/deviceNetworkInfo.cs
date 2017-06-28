using System.Net;
using System.Text;

namespace AxisDeviceDiscoveryLib.core.types
{
    /// <summary>
    /// Class represting a discovered device returned by the Discovery service protocols
    /// </summary>
    public class deviceNetworkInfo
    {
        public IPAddress IPaddress { get; set; }
        public string MACAddress { get; set; }
        public string ONVIFXAddress { get; set; }
        public string UPNPServiceAddress { get; set; }
        public string Model { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(IPaddress);

            if (!string.IsNullOrEmpty(MACAddress))
                sb.Append(" - " + MACAddress);

            if (!string.IsNullOrEmpty(ONVIFXAddress))
                sb.Append(" - Onvif : " + ONVIFXAddress);

            if (!string.IsNullOrEmpty(UPNPServiceAddress))
                sb.Append(" - UPNP : " + UPNPServiceAddress);

            return sb.ToString();
        }
    }
}

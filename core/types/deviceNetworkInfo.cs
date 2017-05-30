using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxisDeviceDiscoveryLib.core.types
{
    /// <summary>
    /// Class represting a discovered device returned by the Discovery service protocols
    /// </summary>
    public class deviceNetworkInfo
    {
        public string IPAddress;
        public string MACAddress;
        public string XAddress;
        public string Model;
    }
}

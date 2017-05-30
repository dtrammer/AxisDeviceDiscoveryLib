using AxisDeviceDiscoveryLib.core.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxisDeviceDiscoveryLib.core.types
{
    public interface IDiscoveryService
    {
        eOnDiscoveryCompleted OnDiscoveryCompleted { get; set; }
        bool IsRunning { get; set; }
        void search(int TimeOut);
    }
}

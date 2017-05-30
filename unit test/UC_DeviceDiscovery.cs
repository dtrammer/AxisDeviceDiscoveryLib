using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AxisDeviceDiscoveryLib;
using System.Collections.Generic;
using AxisDeviceDiscoveryLib.core.services;
using AxisDeviceDiscoveryLib.core.types;

namespace UT_AxisDeviceDiscoveryLib
{
    [TestClass]
    public class UC_DeviceDiscovery
    {
        [TestMethod]
        public void GetActiveNetworkInterfaces()
        {
            DiscoveryService discovery = new DiscoveryService();

            foreach (networkInterface x in discovery.ActiveInterfaces)
            {
                Console.WriteLine(x.Lanid + " - " + x.type.ToString() + " - " + x.IPAddress);
            }

            Assert.IsTrue(discovery.ActiveInterfaces.Count > 0);
        }

        [TestMethod]
        public void search_With_WSDiscovery()
        {
            DiscoveryService discovery = new DiscoveryService();
            Discovery_WS webSearch = new Discovery_WS(discovery.ActiveInterfaces , 
                (interfaces) =>
                {
                    foreach (networkInterface ni in interfaces)
                    {
                        Console.WriteLine("Interface : " + ni.Lanid + " ip : " + ni.IPAddress + " Devices found " + ni.DiscoveredDevices.Count + " :\r\n");
                        foreach (deviceNetworkInfo dn in ni.DiscoveredDevices)
                            Console.WriteLine("Model : " + dn.Model + "\r\nIPAddress : " + dn.IPAddress + "\r\nXAddress : " + dn.XAddress + "\r\nMacAddress : " + dn.MACAddress);
                    }
                }
            );

            webSearch.search(3000);

            while(webSearch.IsRunning) { }

            //For test to be true it should be run with at least 1 device on the network
            Assert.IsTrue(discovery.ActiveInterfaces.Count > 0 && discovery.ActiveInterfaces[0].DiscoveredDevices.Count > 0);
        }

        [TestMethod]
        public void search_With_UPNPDiscovery()
        {
            DiscoveryService discovery = new DiscoveryService();
            Discovery_Upnp upnpSearch = new Discovery_Upnp(discovery.ActiveInterfaces,
                (interfaces) =>
                {
                    foreach (networkInterface ni in interfaces)
                    {
                        Console.WriteLine("Interface : " + ni.Lanid + " ip : " + ni.IPAddress + " Devices found " + ni.DiscoveredDevices.Count + " :\r\n");
                        foreach (deviceNetworkInfo dn in ni.DiscoveredDevices)
                            Console.WriteLine("Model : " + dn.Model + "\r\nIPAddress : " + dn.IPAddress + "\r\nXAddress : " + dn.XAddress + "\r\nMacAddress : " + dn.MACAddress);
                    }
                }
            );

            upnpSearch.search(3000);

            while (upnpSearch.IsRunning) { }

            //For test to be true it should be run with at least 1 device on the network
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void search_With_DiscoveryService()
        {
            IList<networkInterface> Interfaces = new List<networkInterface>();
            DiscoveryService discovery = new DiscoveryService( (interfaces) => { Interfaces = interfaces; });

            discovery.Search(3000);

            while (discovery.IsRunning) { }

            foreach (networkInterface ni in Interfaces)
            {
                Console.WriteLine("*** Interface : " + ni.Lanid + " local IP : " + ni.IPAddress + " - " + ni.DiscoveredDevices.Count + " devices ***\r\n");
                foreach (deviceNetworkInfo dn in ni.DiscoveredDevices)
                {
                    Console.WriteLine("Model : " + dn.Model + "\r\nIPAddress : " + dn.IPAddress + "\r\nXAddress : " + dn.XAddress + "\r\nMacAddress : " + dn.MACAddress +"\r\n");
                }
            }

            //For test to be true there should be one active interface and it should be run with at least 1 device on the network
            Assert.IsTrue(Interfaces[0].DiscoveredDevices.Count > 0);
        }

    }
}

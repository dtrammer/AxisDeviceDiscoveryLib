# AxisDeviceDiscoveryLib
C# Library to discover Axis devices on a TCP/IP network, it uses the SOAP based WS-Discovery protocol and UPNP Protocol

<h3>Prerequisite & install</h3>

- .net 4.5.2
- Download the AxisDeviceDiscoveryLib.dll and add it as a reference to your VS project
  Use the object browser to explore the ActionEventLib namespace
  
<H3>Comments</H3>
  
  - The library supports and scans on multiple Network Interfaces at once
  - WS-Discovery is a Microsoft standard protocol that allows the discovery of web-services over a TCP/IP network, all devices that       support ONVIF will reply to this protocol, this is enabled by default on Axis devices. Devices from other vendors with support for ONVIF can also be detected
  - UPNP is also enabled by default on Axis devices, by setting the MACVendorFilterPrefix (by default : "00408C|ACCC8E" for Axis devices) property to an empty string on the Discovery_Upnp service object, one can discover all UPNP enabled devices on a TCP/IP network (for ex: Other vendor cameras, Smart TV's, Smart phones etc ...)
  
<H3>Usage sample</H3>

DiscoveryService discovery = new DiscoveryService(OnDiscoveryCompleted);
discovery.Search(3000);

//Callback method
public void OnDiscoveryCompleted(IList<networkInterface> Interfaces)
{
  foreach (networkInterface ni in Interfaces)
            {
                Console.WriteLine("*** Interface : " + ni.Lanid + " local IP : " + ni.IPAddress + " - " + ni.DiscoveredDevices.Count + " devices ***\r\n");
                foreach (deviceNetworkInfo dn in ni.DiscoveredDevices)
                {
                    Console.WriteLine("Model : " + dn.Model + "\r\nIPAddress : " + dn.IPAddress + "\r\nXAddress : " + dn.XAddress + "\r\nMacAddress : " + dn.MACAddress +"\r\n");
                }
            }
}

<H3>Main object</H3>

<h4>DiscoveryService</h4>
Main service used for device network discovery
<table>
<th>Property type</th><th>Name</th><th>Description</th>
  <tr>
    <td>List&lt;networkInterface&gt;</td>
    <td>ActiveInterfaces</td>
    <td>List of networkInterface objects representing active network interfaces of the system</td>
  </tr>
  <tr>
    <td>Bool</td>
    <td>IsRunning</td>
    <td>Indicates if the discovery service is currently running</td>
  </tr>
</table>

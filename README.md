# AxisDeviceDiscoveryLib
C# Library to discover Axis devices on a TCP/IP network, it uses the SOAP based WS-Discovery protocol and UPNP Protocol

<h3>Prerequisite & install</h3>

- .net 4.5.2
- Download the AxisDeviceDiscoveryLib.dll and add it as a reference to your VS project
  Use the object browser to explore the ActionEventLib namespace
  
<H3>Comments</H3>
  
  - The library supports and scans on multiple Network Interfaces at once
  - WS-Discovery is a Microsoft standard protocol that allows the discovery of web-services over a TCP/IP network, all devices that       support ONVIF will reply to this protocol and is enabled by default on Axis devices. As any device that supports the ONVIF protocol will reply this also means that devices from other vendors can also be detected
  - UPNP is also enabled by default on Axis devices, by setting the MACVendorFilterPrefix (by default : "00408C|ACCC8E" for Axis devices) property to an empty string on the Discovery_Upnp service object, one can discover all UPNP enabled devices on a TCP/IP network (for ex: Other vendor cameras, Smart TV's, Smart phones etc ...)
  
<H3>Samples</H3>

See the unit test UC_DeviceDiscovery class of the UT_AxisDeviceDiscoveryLib project for usage samples 

<H3>Objects</H3>

<h4>DiscoveryService</h4>
<table>
  <th>Property</th><th>Description</th>
  <tr>
    <td>List<networkInterface> ActiveInterfaces</td>
    <td>List of active network interfaces of the system</td>
  </tr>
  <tr>
    <td>Bool IsRunning</td>
    <td>Indicates if the discovery service is currently running</td>
  </tr>
</table>

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
  
<H3>Samples</H3>

See the unit test UC_DeviceDiscovery class of the UT_AxisDeviceDiscoveryLib project for usage samples 

<H3>Objects</H3>

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

<h4>Discovery_WS</h4>
Represents an active network interfaces installed on the system
<table>
<th>Property type</th><th>Name</th><th>Description</th>
  <tr>
    <td>String</td>
    <td>ProbeMessage</td>
    <td>Message that is broadcasted on the network for WS-Discovery based devices</td>
  </tr>
  <tr>
    <td>Event&lt;networkInterface,IList&lt;String&gt;&gt;</td>
    <td>OnSearchCompleted</td>
    <td>Callback that is invoked when service has finished searching</td>
  </tr>
</table>

<h4>networkInterface</h4>
Represents an active network interfaces installed on the system
<table>
<th>Property type</th><th>Name</th><th>Description</th>
  <tr>
    <td>List&lt;deviceNetworkInfo&gt;</td>
    <td>ActiveInterfaces</td>
    <td>List of deviceNetworkInfo references, gets populated by the DiscoveryService </td>
  </tr>
  <tr>
    <td>String</td>
    <td>IPAddress</td>
    <td>Currently used IPv4 address of the interface</td>
  </tr>
  <tr>
    <td>int</td>
    <td>Lanid</td>
    <td>ID automatically assigned by library to the current interface</td>
  </tr>
    <tr>
    <td>NetworkInterfaceType</td>
    <td>type</td>
    <td>Enum that specifies the type of network interface, either NetworkInterfaceType.Ethernet or NetworkInterfaceType.Wireless80211</td>
  </tr>
</table>

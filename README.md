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

<p>DiscoveryService discovery = new DiscoveryService(OnCompletedCallback);</p>
<p>discovery.Search(3000);</p>

Declare a DiscoveryService object and pass it a Callback method that will be invoked on completion.
  - Callback is of type eOnDiscoveryCompleted(IList<networkInterface> Interfaces)
Then call the Search(int TimeoutInMillisec) method on the DiscoveryService reference
<br>
The callback will be invoked on completion that occurs after the specified timeout in the Search(int) method
The callback will have a List of &lt;networkInterface&gt; instances representing the active network interfaces of the system, each networkInterface has a property of type List&lt;deviceNetworkInfo&gt; that contains the discovered device information.
<br>
<h4>Callback example :</h4>

private void OnDiscoveryCompleted(List&lt;networkInterface&gt;)
{
  //Do something with the results in the different Interfaces
}

The networkInterface instance has members :
- Lanid (in case you have multiple interfaces installed on the system) 
- IPAddress, the current IPv4 address used by the interface
- type, representing the Data-link protocol either NetworkInterfaceType.Ethernet or NetworkInterfaceType.Wireless80211
- DiscoveredDevices, a List of &lt;deviceNetworkInfo&gt; instances representing a discovered device on the network

The deviceNetworkInfo instance has members :
- IPAddress, IPv4 address of the device
- MACAddress
- XAddress, onvif service address
- Model, device model name

For more usage sample, have a look at the UnitTest project

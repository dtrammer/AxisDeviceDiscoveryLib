<h2>AxisDeviceDiscoveryLib</h2>
C# Library to discover Axis devices on a TCP/IP network, it uses the SOAP based WS-Discovery protocol and UPNP Protocol

<h3>Prerequisite & install</h3>

- .net 4.5.2
- Download the AxisDeviceDiscoveryLib.dll and add it as a reference to your VS project
  Use the object browser to explore the ActionEventLib namespace
  
<H3>Comments</H3>
  
  - The library supports and scans on multiple Network Interfaces at once
  - Searches are executed on different threads
  - WS-Discovery is a Microsoft standard protocol that allows the discovery of web-services over a TCP/IP network, all devices that       support ONVIF will reply to this protocol, this is enabled by default on Axis devices. Devices from other vendors with support for ONVIF can also be detected
  - UPNP is also enabled by default on Axis devices, by setting the MACVendorFilterPrefix (by default : "00408C|ACCC8E" for Axis devices) property to an empty string on the Discovery_Upnp service object, one can discover all UPNP enabled devices on a TCP/IP network (for ex: Other vendor cameras, Smart TV's, Smart phones etc ...)
  
<H3>Usage sample</H3>
<br><code>
<p>DiscoveryService discovery = new DiscoveryService(OnCompletedCallback);</p>
<p>discovery.Search(3000);</p></code>

 1. Declare a <b>DiscoveryService</b> object and pass it a Callback method that will be invoked on completion.
    -  Callback is of type <b>eOnDiscoveryCompleted(IList&lt;networkInterface&gt; Interfaces)</b>
 2. Then call the <b>Search(int TimeoutInMillisec)</b> method on the DiscoveryService reference

<p>The callback will be invoked on completion that occurs after the specified timeout in the <b>Search(...)</b> method it will be passed a List of <b>&lt;networkInterface&gt;</b> instances as parameter (representing the active network interfaces of the system), each <b>networkInterface</b> instance has a property of type <b>List&lt;deviceNetworkInfo&gt;</b> that contains the discovered device information.</p>

<h4>Callback example :</h4>
<code>
<p>private void OnDiscoveryCompleted(List&lt;networkInterface&gt;)
{</p>
<p>//Do something with the results in the different Interfaces</p>
<p>}</p>
</code>

The <b>networkInterface</b> instance has members :
- Lanid (in case you have multiple interfaces installed on the system) 
- IPAddress, the current IPv4 address used by the interface
- type, representing the Data-link protocol either NetworkInterfaceType.Ethernet or NetworkInterfaceType.Wireless80211
- DiscoveredDevices, a List of &lt;deviceNetworkInfo&gt; instances representing a discovered device on the network

The <b>deviceNetworkInfo</b> instance has members :
- IPAddress, IPv4 address of the device
- MACAddress
- XAddress, onvif service address
- Model, device model name

For more usage samples, have a look at the UnitTest project

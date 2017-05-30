# AxisDeviceDiscoveryLib
C# Library to discover Axis devices on TCP/IP networks, it uses the SOAP based WS-Discovery protocol and UPNP Protocol

<h3>Prerequisite & install</h3>

- .net 4.5.2
- Download the AxisDeviceDiscoveryLib.dll and add it as a reference to your VS project
  Use the object browser to explore the ActionEventLib namespace
  
  <H3>Comments</H3>
  
  - WS-Discovery is a Microsoft standard protocol that allows the discovery of web-services over a TCP/IP network, all devices that support ONVIF will reply to this protocol and is enabled by default on Axis devices. As any device that supports the ONVIF protocol will reply this also means that devices from other vendors can also be detected
  - UPNP is also enabled by default on Axis devices, by setting the MACVendorFilterPrefix property to an empty string on the Discovery_Upnp service object, one can discover all UPNP enabled devices on a TCP/IP network (for ex: Other vendor cameras, Smart TV's, Smart phones etc ...)
  
  <H3>Samples</H3>

using AxisDeviceDiscoveryLib.core.types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace AxisDeviceDiscoveryLib.core.services
{
    //3702   
    public class Discovery_WS : UDPMutlicast
    {
        public bool ExcludeLocalLinkAddesses;

        private string ProbeMessage = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\">"
                            + "<s:Header>"
                            + "<a:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/04/discovery/Probe</a:Action>"
                            + "<a:MessageID>urn:uuid:{0}</a:MessageID>"
                            + "<a:ReplyTo><a:Address>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>"
                            + "</a:ReplyTo><a:To s:mustUnderstand=\"1\">urn:schemas-xmlsoap-org:ws:2005:04:discovery</a:To>"
                            + "</s:Header><s:Body><Probe xmlns=\"http://schemas.xmlsoap.org/ws/2005/04/discovery\">"
                            + "< Durationxmlns=\"http://schemas.microsoft.com/ws/2008/06/discovery\">PT5S</Duration>"
                            + "</Probe></s:Body></s:Envelope>";

        public Discovery_WS(List<networkInterface> Interfaces, eOnDiscoveryCompleted OnDiscoveryDoneCallback, bool ExcludeLocalLinkAddresses = false) : base(Interfaces, OnDiscoveryDoneCallback)
        {
            base._probeMessage = ProbeMessage;
            base.MulticastAddress = "239.255.255.250";
            base.MulticastPort = 3702;

            this.ExcludeLocalLinkAddesses = ExcludeLocalLinkAddresses;
        }

        //Search is overrided as the WS-Discovery protocol will automatically broadcast on the different active network interfaces
        //So we just take the first NIC in the list and create only one socket
        public override void search(int TimeOut)
        {
            if (string.IsNullOrEmpty(base.MulticastAddress))
                throw new Exception("MULTICAST_ADDRESS_NOTSET");

            if (base.MulticastPort == 0)
                throw new Exception("MULTICAST_PORT_NOTSET");

            if (base._interfaces.Count == 0)
                throw new Exception("NO_VALID_NETWORK_INTERFACE");

            base._sockets.Clear();

            IsRunning = true;

            _sockets.Add(
                    new UDPSocket(_interfaces[0], base.MulticastAddress, base.MulticastPort, new Action<networkInterface, IList<Tuple<IPAddress,string>>>(OnSearchCompleted))
                );
            base._sockets[_sockets.Count - 1].start(TimeOut, _probeMessage);
        }

        protected override void OnSearchCompleted(networkInterface Interface, IList<Tuple<IPAddress, string>> Responses)
        {
            //Process the result from socket for this Interface
            string[] Xaddresses;
            try
            {
                foreach (Tuple<IPAddress, string> s in Responses)
                {
                    Xaddresses = Regex.Match(s.Item2, @"(?<=XAddrs>).*?(?=</)", RegexOptions.Singleline).Value.Split(char.Parse(" "));
                    for (int idx = 0; idx < Xaddresses.Length; idx++)
                    {
                        if(ExcludeLocalLinkAddesses)
                        {
                            string ipaddress = Regex.Match(Xaddresses[idx], "(?<=http://).*?(?=/)").Value;
                            if(ipaddress.Substring(0,7) != "169.254")
                                Interface.add_DeviceInfo(new deviceNetworkInfo() { IPaddress = s.Item1, ONVIFXAddress = Xaddresses[idx],  Model = parse_model_from_service_response(s.Item2) });
                        }
                        else
                            Interface.add_DeviceInfo(new deviceNetworkInfo() { IPaddress = s.Item1, ONVIFXAddress = Xaddresses[idx], Model = parse_model_from_service_response(s.Item2) });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            base.NotifyResultProcessingDone();
        }

        private string parse_model_from_service_response(string Response)
        {
            string model = string.Empty;
            try
            {
                string scopes = Regex.Match(Response, @"(?<=Scopes>).*?(?=</)",RegexOptions.Singleline).Value;
                model = Regex.Match(scopes, "(?<=hardware/).*?(?= )").Value;
            }
            catch (Exception)
            {
            }
            return model;
        }
    }
}

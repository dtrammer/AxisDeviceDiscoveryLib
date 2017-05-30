using AxisDeviceDiscoveryLib.core.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AxisDeviceDiscoveryLib.core.services
{
    //3702   
    public class Discovery_WS : UDPMutlicast
    {
        private string ProbeMessage = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2004/08/addressing\">"
                            + "<s:Header>"
                            + "<a:Action s:mustUnderstand=\"1\">http://schemas.xmlsoap.org/ws/2005/04/discovery/Probe</a:Action>"
                            + "<a:MessageID>urn:uuid:{0}</a:MessageID>"
                            + "<a:ReplyTo><a:Address>http://schemas.xmlsoap.org/ws/2004/08/addressing/role/anonymous</a:Address>"
                            + "</a:ReplyTo><a:To s:mustUnderstand=\"1\">urn:schemas-xmlsoap-org:ws:2005:04:discovery</a:To>"
                            + "</s:Header><s:Body><Probe xmlns=\"http://schemas.xmlsoap.org/ws/2005/04/discovery\">"
                            + "< Durationxmlns=\"http://schemas.microsoft.com/ws/2008/06/discovery\">PT5S</Duration>"
                            + "</Probe></s:Body></s:Envelope>";

        public Discovery_WS(List<networkInterface> Interfaces, eOnDiscoveryCompleted OnDiscoveryDoneCallback) : base(Interfaces, OnDiscoveryDoneCallback)
        {
            base._probeMessage = ProbeMessage;
            base.MulticastAddress = "239.255.255.250";
            base.MulticastPort = 3702;
        }

        protected override void OnSearchCompleted(networkInterface Interface, IList<string> Responses)
        {
            //Process the result from socket for this Interface
            string[] Xaddresses;
            foreach (string s in Responses)
            {
                Xaddresses = Regex.Match(s, @"(?<=XAddrs>).*?(?=</)", RegexOptions.Singleline).Value.Split(char.Parse(" "));
                for (int idx = 0; idx < Xaddresses.Length; idx++)
                    Interface.add_DeviceInfo(new deviceNetworkInfo() { XAddress = Xaddresses[idx], IPAddress = Regex.Match(Xaddresses[idx], "(?<=http://).*?(?=/)").Value, Model = parse_model_from_service_response(s) });
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

using AxisDeviceDiscoveryLib.core.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AxisDeviceDiscoveryLib.core.types
{
    public abstract class UDPMutlicast : IDiscoveryService
    {
        private List<networkInterface> _interfaces;
        private List<UDPSocket> _sockets = new List<UDPSocket>();
        private Timer _discoveryTimeOut;

        protected string _probeMessage;
        private int _multicastPort;
        public int MulticastPort { get { return _multicastPort; } set { _multicastPort = value; } }
        private string _multicastAddress = "239.255.255.250";
        public string MulticastAddress { get { return _multicastAddress; } set { _multicastAddress = value; } }
        public bool IsRunning { get; set; }
        public eOnDiscoveryCompleted OnDiscoveryCompleted { get; set; }

        public UDPMutlicast(List<networkInterface> Interfaces, eOnDiscoveryCompleted OnDiscoveryCompleted)
        {
            _interfaces = Interfaces;
            this.OnDiscoveryCompleted += OnDiscoveryCompleted;
        }

        public void search(int TimeOut)
        {
            if (string.IsNullOrEmpty(_multicastAddress))
                throw new Exception("MULTICAST_ADDRESS_NOTSET");

            if (_multicastPort == 0)
                throw new Exception("MULTICAST_PORT_NOTSET");

            if (_interfaces.Count == 0)
                throw new Exception("NO_VALID_NETWORK_INTERFACE");

            _sockets.Clear();

            IsRunning = true;
            foreach (networkInterface i in _interfaces)
            {
                _sockets.Add(
                    new UDPSocket(i, _multicastAddress , _multicastPort, new Action<networkInterface, IList<string>>(OnSearchCompleted))
                );
                _sockets[_sockets.Count - 1].start(TimeOut, _probeMessage);
            }
        }

        protected abstract void OnSearchCompleted(networkInterface Interface, IList<string> Responses);

        protected void NotifyResultProcessingDone()
        {
            if(_sockets.Count(z=> z._isRunning) == 0)
            {
                IsRunning = false;
                //Raise final event when all responses are received or when timeout reached
                if (OnDiscoveryCompleted != null)
                    OnDiscoveryCompleted.Invoke(_interfaces);
            }
        }
    }
}

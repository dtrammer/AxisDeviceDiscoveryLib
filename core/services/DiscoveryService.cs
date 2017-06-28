using AxisDeviceDiscoveryLib.core.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;



namespace AxisDeviceDiscoveryLib.core.services
{
    #region delegates declaration
    public delegate void eOnDiscoveryCompleted(IList<networkInterface> Interfaces); 
    #endregion

    /// <summary>
    /// Main class used to discover devices on the network, it will search with all available protocols (WS-discovery & UPNP)
    /// </summary>
    public class DiscoveryService
    {
        #region private members
        private IList<IDiscoveryService> _discoveryServices = new List<IDiscoveryService>();
        private eOnDiscoveryCompleted _onDiscoveryCompleted;
        #endregion

        #region public members
        /// <summary>
        /// Property indicating if the Discovery service is currently running
        /// </summary>
        public bool IsRunning=false;
        /// <summary>
        /// property containing a list with the active network interfaces of the system
        /// </summary>
        public List<networkInterface> ActiveInterfaces = new List<networkInterface>();
        #endregion

        #region constructors
        public DiscoveryService()
        {
            //Initialise
            this.Initialize();
        }
        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="OnDiscoveryCompletedCallback">eOnDiscoveryCompleted(IList<networkInterface> Interfaces) callback that will be invoked once discovery is done</param>
        public DiscoveryService(eOnDiscoveryCompleted OnDiscoveryCompletedCallback)
        {
            //Initialise
            this.Initialize();
            _onDiscoveryCompleted += OnDiscoveryCompletedCallback;
        }
        #endregion

        #region initialization methods
        private void Initialize()
        {
            //Get local active network interfaces
            set_NetworkInterfaces();
            //Add discovery services
            _discoveryServices.Add(new Discovery_WS(ActiveInterfaces, OnDiscoveryServiceCompleted));
            _discoveryServices.Add(new Discovery_Upnp(ActiveInterfaces, OnDiscoveryServiceCompleted));
        }
        private void set_NetworkInterfaces()
        {
            int NICcounter = 1;
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((item.NetworkInterfaceType == NetworkInterfaceType.Ethernet || item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && ip.IsDnsEligible == true)
                            ActiveInterfaces.Add(new networkInterface()
                            {
                                Lanid = NICcounter++,
                                type = item.NetworkInterfaceType,
                                IPAddress = ip.Address.ToString()
                            });
                    }
                }
            }

            if (ActiveInterfaces.Count == 0)
                throw new Exception("NO_ACTIVE_IPADDRESS_FOUND");
        }
        #endregion

        #region public Methods
        /// <summary>
        /// Method that will star the search on the network with all available protocols (WS-Discovery & UPNP)
        /// </summary>
        public void Search(int TimeOut)
        {
            IsRunning = true;
            foreach (IDiscoveryService dm in this._discoveryServices)
                Task.Factory.StartNew(()=> { dm.search(TimeOut);});
        }
        #endregion

        #region events
        //Callback invoked by the Discovery Services method on completion
        private void OnDiscoveryServiceCompleted(IList<networkInterface> Interfaces)
        {
            if (!_discoveryServices.Any(x=> x.IsRunning))
            {
                foreach (networkInterface ni in Interfaces)
                    ni.DiscoveredDevices = ni.DiscoveredDevices.OrderByIPAscending();

                _onDiscoveryCompleted.Invoke(ActiveInterfaces);
                IsRunning = false;
            }
        }
        #endregion
    }
}

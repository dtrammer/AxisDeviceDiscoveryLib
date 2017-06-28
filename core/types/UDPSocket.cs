using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AxisDeviceDiscoveryLib.core.types
{
    public class UDPSocket
    {
        #region private members
        private UdpClient _socket;
        private int _listeningPort;
        private Timer _timeOutTimer;
        private networkInterface _interface;
        private int _multicastPort;
        private List<UdpReceiveResult> _discoveryResponses = new List<UdpReceiveResult>();
        private Action<networkInterface,IList<Tuple<IPAddress,string>>> _onDiscoveryCompleted;
        private CancellationTokenSource _cts;
        private string _multicastAddress;
        #endregion
        public bool _isRunning;

        #region constructor
        /// <summary>
        /// Constructor to setup an UDP socket
        /// </summary>
        /// <param name="Interface">The local NIC</param>
        /// <param name="Port">Port to listen on for incoming UDP packets</param>
        /// <param name="OnCompleted">Action<networkInterface> event that is raised when listening is Completed</param>
        public UDPSocket(networkInterface Interface , string MulticastAddress, int MultiCastPort, Action<networkInterface, IList<Tuple<IPAddress, string>>> OnCompleted)
        {
            _interface = Interface;
            _multicastAddress = MulticastAddress;
            _multicastPort = MultiCastPort;
            _onDiscoveryCompleted += OnCompleted;
            _listeningPort = ExtensionMethods.getNewNetworkPort();
        }
        #endregion       

        #region methods
        /// <summary>
        /// Method to start the UDPsocket and send the probe message ,
        /// When timeout is reached the socket will be closed and raise the OnCompleted event
        /// </summary>
        /// <param name="TimeOut">Socket listeneing timeout in milliseconds</param>
        public async void start(int TimeOut , string ProbeMessage)
        {
            //ini socket
            _socket = new UdpClient(new IPEndPoint(IPAddress.Parse(_interface.IPAddress), _listeningPort)) { EnableBroadcast = true };
            //set timeout timer
            _timeOutTimer = new System.Threading.Timer(OnSocketTimeout, null, TimeOut, Timeout.Infinite);

            _isRunning = true;

            byte[] probeMessage = Encoding.ASCII.GetBytes(string.Format(ProbeMessage, Guid.NewGuid().ToString()));

            int result = await _socket.SendAsync(probeMessage, probeMessage.Length,new IPEndPoint(IPAddress.Parse(_multicastAddress), _multicastPort));
            _cts = new CancellationTokenSource();
            try
            {
                UdpReceiveResult response;
                while (_isRunning)
                {
                    response = await this._socket.ReceiveAsync().WithCancellation<UdpReceiveResult>(_cts.Token);
                    _discoveryResponses.Add(response);
                }
            }
            catch (System.OperationCanceledException) {
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _socket.Close();
                ExtensionMethods.releaseNetworkPort(_listeningPort);
            }
        }

        private void OnSocketTimeout(object state)
        {
            _timeOutTimer.Dispose();
            _cts.Cancel();
            _isRunning = false;
            ////process the responses
            List<Tuple<IPAddress,string>> responses = new List<Tuple<IPAddress, string>>();

            foreach (UdpReceiveResult s in this._discoveryResponses)
            {
                if (s.Buffer != null && s.Buffer.Length > 0)
                {
                    var response = Encoding.UTF8.GetString(s.Buffer);
                    responses.Add(new Tuple<IPAddress, string>(s.RemoteEndPoint.Address, response));
                }
            }

            if (_onDiscoveryCompleted != null)
                _onDiscoveryCompleted.Invoke(_interface,responses);
        }
        #endregion
    }
}

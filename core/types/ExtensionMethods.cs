using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AxisDeviceDiscoveryLib.core.types
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Used to provide cancellation possibility to any Async Methods returning a Task<T>
        /// </summary>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                        s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }

        #region network ports management
        //Maintains network ports used by UDPSockets centrally to avoid dubbel usage
        private static List<int> _networkPort = new List<int>();
        public static int getNewNetworkPort()
        {
            int port = new Random((int)(System.DateTime.Now.Ticks)).Next(12400, 14000);
            while(_networkPort.Contains(port))
                new Random((int)(System.DateTime.Now.Ticks)).Next(12400, 14000);

            _networkPort.Add(port);
            return port;
        }
        public static void releaseNetworkPort(int Port)
        {
            if (_networkPort.Contains(Port))
                _networkPort.Remove(Port);
        }
        #endregion
    }

}

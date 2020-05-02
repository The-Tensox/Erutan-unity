using System;
using Erutan;
using Utils;
using Component = UnityEngine.Component;

namespace Sessions
{
    /// <summary>
    /// Should act as a high-level layer to networking
    /// </summary>
    public class SessionManager : Singleton<SessionManager>
    {
        #region Variables
        private string _deviceId;
        private Client _client;

        #endregion
        

        #region Properties

        public Client Client => _client;

        #endregion
        
        #region events

        /// <summary>
        /// Received when the client is disconnected.
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// Received when the client is connected.
        /// </summary>
        public event Action Connected;

        #endregion

        public void Init(string ip, int port)
        {
            _client = new Client(ip, port);
            _client.StreamConnected += ClientOnStreamConnected;
            _client.StreamClosed += ClientOnStreamClosed;
        }

        
        private void ClientOnStreamConnected()
        {
            Connected?.Invoke();
        }

        private void ClientOnStreamClosed()
        {
            Disconnected?.Invoke();
        }

        public void Connect()
        {
            _client.Init();
        }

        public void UpdateParameters(double timeScale = 0, bool debug = false, Protometry.Box cullingArea = null)
        {
            var p = new Packet();
            var t = new Packet.Types.UpdateParametersPacket.Types.Parameter();
            if (timeScale != 0) t.TimeScale = timeScale;
            if (debug) t.Debug = debug;
            if (cullingArea != null) t.CullingArea = cullingArea;
            p.UpdateParameters = new Packet.Types.UpdateParametersPacket();
            p.UpdateParameters.Parameters.Add(t);
            Instance.Client.Send(p);
        }
    }
}
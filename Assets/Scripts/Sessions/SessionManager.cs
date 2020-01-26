using UnityEngine;
using System;
using Scripts.Utils;
using System.Threading.Tasks;

namespace Erutan.Scripts.Sessions
{
    public class SessionManager : Singleton<SessionManager>
    {
        #region Variables
        [SerializeField] private string _ipAddress = "localhost";
        [SerializeField] private int _port = 50051;
        private string _deviceId;
        private Client _client;

        #endregion

        #region Properties

        public Client Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new Client(_ipAddress, _port);
                }
                return _client;
            }
        }

        #endregion

        #region Events
        public event Action OnConnectionSuccess = delegate { Record.Log(">> Connection Success"); };

        public event Action OnNewAccountCreated = delegate { Record.Log(">> New Account Created"); };

        public event Action OnConnectionFailure = delegate { Record.Log(">> Connection Error"); };

        public event Action OnDisconnected = delegate { Record.Log(">> Disconnected"); };

        #endregion

        

/*
        public async Task<bool> ConnectAsync()
        {
            //HelloReply response = await Client.SayHelloAsync($"Log me plz");
            
            switch (response)
            {
                case AuthenticationResponse.Authenticated:
                    OnConnectionSuccess?.Invoke();
                    break;
                case AuthenticationResponse.NewAccountCreated:
                    OnNewAccountCreated?.Invoke();
                    OnConnectionSuccess?.Invoke();
                    break;
                case AuthenticationResponse.Error:
                    OnConnectionFailure?.Invoke();
                    break;
                default:
                    InConsole.Instance.Log("Unhandled response received: " + response, LogLevel.Error);
                    break;
            }

            if (response != null) {
                OnConnectionSuccess?.Invoke();
            } else {
                OnConnectionFailure?.Invoke();
            }
            return response != null;
            
            return true;
        }

        public async Task<bool> SendPosition(Vector3 position) {
            return await Client.SendPosition(position);
        }

        public async Task<bool> DisconnectAsync()
        {
            
            ByeReply response = await Client.SayByeAsync($"Disconnect me plz");
            if (response != null) OnDisconnected.Invoke();
            return response != null;
            
            return true;
        }
        */
    }
}
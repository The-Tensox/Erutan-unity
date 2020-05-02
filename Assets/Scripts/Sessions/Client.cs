using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Erutan;
using Grpc.Core;
using UnityEngine;
using Utils;
using static Erutan.Erutan;
using Metadata = Erutan.Metadata;

namespace Sessions
{
    /// <summary>
    /// Should act as the lowest layer of networking, should be used rarely
    /// </summary>
    public class Client
    {

        #region properties

        public string Host { get; }
        public int Port { get; }
        
        public bool IsConnected;
        public string Token => _token;

        #endregion


        #region events

        /// <summary>
        /// Received when a stream is closed.
        /// </summary>
        public event Action StreamClosed;

        /// <summary>
        /// Received when a stream is connected.
        /// </summary>
        public event Action StreamConnected;

        /// <summary>
        /// Received a stream packet.
        /// </summary>
        public event Action<Packet> ReceivedPacket;

        #endregion


        private ErutanClient _networkClient;
        private Channel _channel;

        private string _token;



        private Thread _listenerThread;
        private Thread _senderThread;

        private AsyncDuplexStreamingCall<Packet, Packet> _stream;
        private IAsyncStreamReader<Packet> _inStream;
        private IAsyncStreamWriter<Packet> _outStream;
        private Queue<Packet> _packetsToBeSent = new Queue<Packet>();
        public Client(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public async void Init() {
            // Add client cert to the handler
            Task AsyncAuthInterceptor(AuthInterceptorContext context, Grpc.Core.Metadata metadata)
            {
                return Task.Run(() => { metadata.Add("Authorization", $"Bearer {_token}"); });
            }

            var channelCredentials = new SslCredentials(File.ReadAllText($"{Application.streamingAssetsPath}/server1.crt"));//, //new KeyCertificatePair(File.ReadAllText($"{Application.dataPath}/server1.crt"), ""));
            
            var callCredentials = CallCredentials.FromInterceptor(AsyncAuthInterceptor);
            Record.Log($"Trying to connect ...");

            _channel = new Channel(
                "127.0.0.1",
                50051,
                ChannelCredentials.Create(
                    channelCredentials,
                    callCredentials)/*,
                new []{ // https://grpc.github.io/grpc/node/grpc.html#~ChannelOptions
                    new ChannelOption("grpc.keepalive_permit_without_calls", 1),
                    new ChannelOption("grpc.keepalive_time_ms", 5 * 1000), // 5 seconds
                }*/
            );

            await _channel.ConnectAsync(deadline: System.DateTime.UtcNow.AddSeconds(20));
            Record.Log($"gRPC channel status: {_channel.State}");
            _networkClient = new ErutanClient(_channel);
            //var headers = new Grpc.Core.Metadata(); // https://github.com/grpc/grpc/blob/master/doc/PROTOCOL-HTTP2.md
            //headers.Add(new Grpc.Core.Metadata.Entry("grpc-timeout", "2S"));
            _stream = _networkClient.Stream();
            
            _inStream = _stream.ResponseStream;
            _outStream = _stream.RequestStream;
            
            // Listen to server packets on different thread
            _listenerThread = new Thread(Listen) {IsBackground = true};
            _listenerThread.Start();
            
            // Send packets to server on different thread
            _senderThread = new Thread(DequeuePacketsToBeSent) {IsBackground = true};
            _senderThread.Start();
            
            StreamConnected?.Invoke();
            IsConnected = true;
        }

        public void Logout() {
            // Close stream !
            _stream?.Dispose();
            StreamClosed?.Invoke();
        }

        private async void Listen() {
            while (await _inStream.MoveNext())
            {
                var packet = _inStream.Current;
                // Invoke the event on the main thread
                UnityMainThreadDispatcher.Instance()
                    .Enqueue(() =>
                    {
                        switch (packet.TypeCase)
                        {
                            case Packet.TypeOneofCase.Authentication:
                                _token = packet.Authentication.ClientToken;
                                // Record.Log($"Received auth {_token}");

                                break;
                            case Packet.TypeOneofCase.CreatePlayer:
                                Record.Log($"Received create player {packet.CreatePlayer}");

                                break;
                        }
                        // Record.Log($"Received {packet}");

                        ReceivedPacket?.Invoke(packet);
                    });
            }
        }

        private async void DequeuePacketsToBeSent()
        {
            while (true)
            {
                while (_packetsToBeSent.Count > 0)
                {
                    // Record.Log($"Send");
                    await _outStream.WriteAsync(_packetsToBeSent.Dequeue());
                }
                await Task.Delay(100);
            }
        }
        
        /// General purpose low level client to server packet sending method
        public void Send(Packet packet)
        {
            if (!IsConnected) {
                Record.Log($"Not connected !", LogLevel.Error);
                return;
            }
            packet.Metadata = new Metadata();
            _packetsToBeSent.Enqueue(packet);
        }
    }
}

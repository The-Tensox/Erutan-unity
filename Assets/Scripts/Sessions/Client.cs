using UnityEngine;
using Grpc.Core;
using Erutan.Scripts.Utils;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.IO;
using static Erutan.Scripts.Protos.Erutan;
using Erutan.Scripts.Protos;

namespace Erutan.Scripts.Sessions
{
    public class Client
    {
        public string Host { get; }
        public int Port { get; }
        public bool IsConnected;


        /// <summary>
        /// Received when a stream is closed.
        /// </summary>
        public event Action Closed;

        /// <summary>
        /// Received when a stream is connected.
        /// </summary>
        public event Action Connected;

        /// <summary>
        /// Received a stream packet.
        /// </summary>
        public event Action<Packet> ReceivedPacket;

        /// <summary>
        /// Received a presence change for joins and leaves with users in a stream.
        /// </summary>
        //event Action<PresenceEvent> ReceivedPresence;

        /// <summary>
        /// Received when an error occurs on the stream.
        /// </summary>
        public event Action<Exception> ReceivedError;

        private ErutanClient _networkClient;
        public Channel _channel;

        private string _token;

        private Thread _listenerThread;

        private AsyncDuplexStreamingCall<Packet, Packet> _stream;
        private IAsyncStreamReader<Packet> _inStream;
        private IAsyncStreamWriter<Packet> _outStream;
        public Client(string host, int port)
        {
            Host = host;
            Port = port;
            // _channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);
        }

        public async void Init() {
            // Add client cert to the handler
            AsyncAuthInterceptor asyncAuthInterceptor = (context, metadata) =>
                  {
                      return Task.Run(() => {
                          metadata.Add("Authorization", $"Bearer {_token}");
                      });
                  };
                  
            var channelCredentials = new SslCredentials(File.ReadAllText($"{Application.dataPath}/server1.crt"));//, //new KeyCertificatePair(File.ReadAllText($"{Application.dataPath}/server1.crt"), ""));
            
            var callCredentials = CallCredentials.FromInterceptor(asyncAuthInterceptor);
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
            Record.Log($"Status: {_channel.State}");
            _networkClient = new ErutanClient(_channel);
            //var headers = new Grpc.Core.Metadata(); // https://github.com/grpc/grpc/blob/master/doc/PROTOCOL-HTTP2.md
            //headers.Add(new Grpc.Core.Metadata.Entry("grpc-timeout", "2S"));
            _stream = _networkClient.Stream();
            
            _inStream = _stream.ResponseStream;
            _outStream = _stream.RequestStream;
            
            // Listen to server packets on different thread
            _listenerThread = new Thread(new ThreadStart(Listen));
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
            
            //SpamServer();

            Connected?.Invoke();
            IsConnected = true;
        }

        public void Logout() {
            // Close stream !
            _stream.Dispose();
            Closed?.Invoke();
        }

        private async void Listen() {
            while (await _inStream.MoveNext())
            {
                var packet = _inStream.Current;
                // Invoke the event on the main thread
                UnityMainThreadDispatcher.Instance()
                    .Enqueue(() => ReceivedPacket?.Invoke(packet));
            }
        }
        
        /// General purpose low level client to server packet sending method
        public async Task Send(Packet packet)
        {
            packet.Metadata = new Protos.Metadata();
            Record.Log($"Sending {packet}");
            await _outStream.WriteAsync(packet);
        }

        private async void SpamServer() {
            var updatePositionPacket = new Packet.Types.UpdatePositionPacket();
            var randomPosition = UnityEngine.Random.insideUnitSphere * 10;
            updatePositionPacket.ObjectId = "zz";
            updatePositionPacket.Position = new NetVector3();
            updatePositionPacket.Position.X = randomPosition.x;
            updatePositionPacket.Position.Y = randomPosition.y;
            updatePositionPacket.Position.Z = randomPosition.z;
            var packet = new Packet();
            packet.UpdatePosition = updatePositionPacket;
            while (true) {
                await Send(packet);
                await Task.Delay(1000);
            }
        }
    }
}
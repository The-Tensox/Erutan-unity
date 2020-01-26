using UnityEngine;
using Grpc.Core;
using Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

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
        event Action Closed;

        /// <summary>
        /// Received when a stream is connected.
        /// </summary>
        event Action Connected;

        /// <summary>
        /// Received a stream message.
        /// </summary>
        event Action<StreamResponse> ReceivedMessage;

        /// <summary>
        /// Received a presence change for joins and leaves with users in a stream.
        /// </summary>
        //event Action<PresenceEvent> ReceivedPresence;

        /// <summary>
        /// Received when an error occurs on the stream.
        /// </summary>
        event Action<Exception> ReceivedError;

        private Erutan.ErutanClient _networkClient;
        private Channel _channel;

        private string _token;

        private Thread _listenerThread;

        private AsyncDuplexStreamingCall<StreamRequest, StreamResponse> _stream;
        private IAsyncStreamReader<StreamResponse> _inStream;
        private IAsyncStreamWriter<StreamRequest> _outStream;
        public Client(string host, int port)
        {
            Host = host;
            Port = port;
            // _channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);
        }

        public async Task Init() {
            /*

            , new []{
                new ChannelOption("grpc.keepalive_permit_without_calls", 1),
                new ChannelOption("grpc.keepalive_time_ms", 5 * 1000), // 5 seconds
            }

            */
            _channel = new Channel(Host, Port, ChannelCredentials.Insecure);
            await _channel.ConnectAsync(deadline: System.DateTime.UtcNow.AddSeconds(20));
            _networkClient = new Erutan.ErutanClient(_channel);
            LoginResponse response =_networkClient.Login(new LoginRequest() { Name = "Louis", Password = ""});
            _token = response.Token;
            var m = new Metadata();
            m.Insert(0, new Metadata.Entry("x-token", _token));
            _stream = _networkClient.Stream(m);
            _inStream = _stream.ResponseStream;
            _outStream = _stream.RequestStream;
            
            // Listen to server packets on different thread
            _listenerThread = new Thread(new ThreadStart(Listen));
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
            
            //SpamServer();
            IsConnected = true;
        }

        public void Logout() {
            _networkClient.Logout(new LogoutRequest() { Token = _token });

            // Close stream !
            _stream.Dispose();
        }

        private async void Listen() {
            while (await _inStream.MoveNext())
            {
                var streamResponse = _inStream.Current;
                switch (streamResponse.EventCase) {
                    case StreamResponse.EventOneofCase.ClientPacket:
                        Packet p = streamResponse.ClientPacket;
                        switch (p.TypeCase) {
                            case Packet.TypeOneofCase.EntityPosition:
                                continue;
                            default:
                                Record.Log($"Unimplemented packet handler ! {p.TypeCase}");
                                break;
                        }
                        break;
                    case StreamResponse.EventOneofCase.ClientLogin:
                    case StreamResponse.EventOneofCase.ClientLogout:
                    case StreamResponse.EventOneofCase.ServerMessage:
                    case StreamResponse.EventOneofCase.ServerShutdown:
                        Record.Log($"Received {streamResponse.EventCase}: {streamResponse}");
                        break;
                    default:
                        // TODO: https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions
                        Record.Log($"Unimplemented packet handler ! {streamResponse.EventCase}");
                        break;
                }
            }
        }

        /// General purpose low level client to server packet sending method
        public async void Send(Packet packet) => await _outStream.WriteAsync(new StreamRequest()
        {
            Message = packet
        });

        private async void SpamServer() {
            var v = new Packet.Types.Position();
            var randomPosition = Random.insideUnitSphere * 10;
            v.Id = "zz";
            v.X = randomPosition.x;
            v.Y = randomPosition.y;
            v.Z = randomPosition.z;
            while (true) {
                await _outStream.WriteAsync(new StreamRequest(){ 
                    Message = new Packet(){ 
                        EntityPosition = v
                    } 
                });
                await Task.Delay(1000);
                return;
            }
        }
    }
}
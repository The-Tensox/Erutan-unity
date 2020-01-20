using UnityEngine;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace Scripts.Sessions
{
    public class Client
    {
        public string Host { get; }
        public int Port { get; }

        private Greeter.GreeterClient _client;
        private Channel _channel;

        public Client(string host, int port)
        {
            Host = host;
            Port = port;
            _channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);
            _client = new Greeter.GreeterClient(_channel);
        }

        public async Task<HelloReply> SayHelloAsync(string message) {
            return await _client.SayHelloAsync(new HelloRequest() { Name = message });
        }

        public async Task<bool> SendPosition(Vector3 p) {
            var protoPosition = new Position();
            protoPosition.X = p.x;
            protoPosition.Y = p.y;
            protoPosition.Z = p.z;
            var response =_client.SendPosition().RequestStream.WriteAsync(protoPosition);
            return true; // TODO
        }

        /*
        public async Task<HelloReply> SendPacketAsync(PacketMessage packet) {
            _client.Send()
        }
        */

        public async Task<ByeReply> SayByeAsync(string message) {
            return await _client.SayByeAsync(new ByeRequest() { Name = message });
        }
    }
}
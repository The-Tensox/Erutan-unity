using Erutan.Scripts.Sessions;
using UnityEngine;


namespace Erutan.Scripts.Gameplay.Network {
    public class TransformNetwork : MonoBehaviour {
        
        private float _lastSent;
        private void Update() {
            // TODO: create a "gameplaymanager" that has also the client
            // so we can split gameplay from session
            /*
            if (SessionManager.Instance.Client.IsConnected && Time.time - _lastSent > 0.5f) {
                var positionPacket = new Packet.Types.Position();
                var myPosition = transform.position;
                positionPacket.Id = GetInstanceID().ToString();
                positionPacket.X = myPosition.x;
                positionPacket.Y = myPosition.y;
                positionPacket.Z = myPosition.z;
                //Debug.Log($"Sending {positionPacket.X};{positionPacket.Y};{positionPacket.Z}");
                SessionManager.Instance.Client.Send(new Packet() { EntityPosition = positionPacket });
                _lastSent = Time.time;
            }
            */
        }
    }
}
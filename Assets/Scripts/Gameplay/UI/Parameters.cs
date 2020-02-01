using Erutan.Scripts.Sessions;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.UI
{
    public class Parameters : MonoBehaviour
    {
        public void UpdateTickrate(float tickrate) {
            // TODO: build higher-level functions instead :)
            var p =new Protos.Packet();
            p.Metadata = new Protos.Metadata();
            var t = new Protos.Packet.Types.UpdateParametersPacket.Types.Parameter();
            t.Tickrate = tickrate;
            p.UpdateParameters = new Protos.Packet.Types.UpdateParametersPacket();
            p.UpdateParameters.Parameters.Add(t);
            SessionManager.Instance.Client.Send(p);
        }
    }
}
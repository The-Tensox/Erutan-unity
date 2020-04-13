using Erutan.Scripts.Sessions;
using Erutan.Scripts.Utils;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.UI
{
    public class Parameters : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI Label;
        public void UpdateTickrate(float timeScale) {
            // TODO: build higher-level functions instead :)
            var p = new Packet {Metadata = new Metadata()};
            var t = new Packet.Types.UpdateParametersPacket.Types.Parameter {TimeScale = timeScale};
            Label.text = $"Timescale {timeScale}";
            p.UpdateParameters = new Packet.Types.UpdateParametersPacket();
            p.UpdateParameters.Parameters.Add(t);
            SessionManager.Instance.Client.Send(p);
        }
        
        public void UpdateDebug(bool debug) {
            var p = new Packet {Metadata = new Metadata()};
            var t = new Packet.Types.UpdateParametersPacket.Types.Parameter {Debug = debug};
            p.UpdateParameters = new Packet.Types.UpdateParametersPacket();
            p.UpdateParameters.Parameters.Add(t);
            SessionManager.Instance.Client.Send(p);
        }
    }
}
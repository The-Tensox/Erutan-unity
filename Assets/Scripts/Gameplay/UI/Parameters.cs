using Erutan.Scripts.Sessions;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.UI
{
    public class Parameters : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI Label;
        public void UpdateTickrate(float timeScale) {
            // TODO: build higher-level functions instead :)
            var p =new Packet();
            p.Metadata = new Metadata();
            var t = new Packet.Types.UpdateParametersPacket.Types.Parameter();
            t.TimeScale = timeScale;
            Label.text = $"Timescale {timeScale}";
            p.UpdateParameters = new Packet.Types.UpdateParametersPacket();
            p.UpdateParameters.Parameters.Add(t);
            SessionManager.Instance.Client.Send(p);
        }
    }
}
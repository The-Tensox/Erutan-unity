using Erutan;
using Sessions;
using UnityEngine;

namespace Gameplay.UI
{
    public class Parameters : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI Label;
        public void UpdateTickrate(float timeScale) {
            Label.text = $"Timescale {timeScale}";
            SessionManager.Instance.UpdateParameters(timeScale);
        }
        
        public void UpdateDebug(bool debug) {
            SessionManager.Instance.UpdateParameters(debug: debug);
        }
    }
}
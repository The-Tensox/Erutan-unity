using Sessions;
using UnityEngine;
using Utils;

namespace Gameplay.Player
{
    /// <summary>
    /// Cull the area around to improve performance
    /// </summary>
    public class Cull : MonoBehaviour
    {
        [HideInInspector] public UnityEngine.Vector3 size;
        private UnityEngine.Vector3 _lastPosition;
        private void Update()
        {
            // Limit the culling requests: delay threshold or moved
            if (Time.frameCount % 5 == 0 || Vector3.Distance(_lastPosition, transform.position) > 1)
            {
                var t = transform;
                var p = t.position;
                SessionManager.Instance.UpdateParameters(cullingArea: new Protometry.Box
                {
                    Min = (p - size).ToVector3(),
                    Max = (p + size).ToVector3(),
                });
                // Record.Log($"Culling now {p}");
                _lastPosition = p;
            }
        }
    }
}
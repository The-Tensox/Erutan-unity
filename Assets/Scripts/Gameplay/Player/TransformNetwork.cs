using System;
using Erutan;
using GamePlay.Object;
using Protometry;
using Sessions;
using UnityEngine;
using Utils;

namespace Gameplay.Player {
    public class TransformNetwork : MonoBehaviour {
        
        private float _lastSent;
        private Protometry.Vector3 _lastPosition;
        [HideInInspector] public ulong id;

        private void Start()
        {
            _lastPosition = transform.position.ToVector3();
        }

        private void Update()
        {
            // return;
            if (SessionManager.Instance.Client.IsConnected && Time.time - _lastSent > 0.1f && ObjectManager.Instance.Objects.ContainsKey(id))
            {
                // Record.Log($"yay {id}");
                var position = transform.position;
                ObjectManager.Instance.UpdateObject(id, position.ToVector3(), position.ToVector3());
                // ObjectManager.Instance.UpdateObject(id, new Protometry.Vector3{X = 0, Y = 50, Z = 0});
                _lastSent = Time.time;
                // _lastPosition = position.ToVector3();
            }
        }
    }
}
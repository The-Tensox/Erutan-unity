using Erutan.Scripts.Protos;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.Nature
{
    public abstract class NatureObject : MonoBehaviour
    {
        [HideInInspector] public string OwnerId;
        [HideInInspector] public string Id;

        public void Move(NetVector3 position) {
            transform.position = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
        }

        public void Rotate(NetQuaternion rotation) {
            Debug.Log($"my rot {transform.rotation}");
            transform.Rotate(0, (float)rotation.Y, 0);// = new Quaternion((float)rotation.X, (float)rotation.Y, (float)rotation.Z, (float)rotation.W);
            Debug.Log($"my rot {transform.rotation}");
        }
    }
}
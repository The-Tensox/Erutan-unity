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
            transform.rotation = new Quaternion((float)rotation.X, (float)rotation.Y, (float)rotation.Z, (float)rotation.W);
        }
    }
}
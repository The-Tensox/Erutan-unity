using Erutan.Scripts.Protos;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.Nature
{
    public class Entity : MonoBehaviour
    {
        [HideInInspector] public ulong Id;

        public void Move(NetVector3 position) {
            transform.position = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
        }

        public void Rotate(NetQuaternion rotation) {
            transform.rotation = new Quaternion((float)rotation.X, (float)rotation.Y, (float)rotation.Z, (float)rotation.W);
        }
    }
}
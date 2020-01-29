using Erutan.Scripts.Protos;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.Nature
{
    public abstract class NatureObject : MonoBehaviour
    {
        [HideInInspector] public string OwnerId;
        [HideInInspector] public string Id;

        public void Move(NetVector3 position) {
            transform.position = new Vector3(position.X, position.Y, position.Z);
        }
    }
}
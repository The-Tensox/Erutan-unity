using System.Collections.Generic;
using Erutan;
using Erutan.Scripts.Utils;
using Protometry;
using UnityEngine;

namespace Erutan.Scripts.Gameplay.Entity
{
    public class Entity : MonoBehaviour
    {
        [HideInInspector] public ulong Id;
        [HideInInspector] public Google.Protobuf.Collections.RepeatedField<Component> Components;

        public void Move(Protometry.Vector3 position) {
            transform.position = position.ToVector3();
        }

        public void Rotate(Protometry.Quaternion rotation) {
            transform.rotation = rotation.ToQuaternion();
        }
    }
}
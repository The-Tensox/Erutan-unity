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

        public void Move(VectorN position) {
            transform.position = position.ToVector3();
        }

        public void Rotate(QuaternionN rotation) {
            transform.rotation = rotation.ToQuaternion();
        }
    }
}
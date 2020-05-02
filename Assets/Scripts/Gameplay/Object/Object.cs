using UnityEngine;
using Utils;
using Component = Erutan.Component;

namespace GamePlay.Object
{
    public class Object : MonoBehaviour
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
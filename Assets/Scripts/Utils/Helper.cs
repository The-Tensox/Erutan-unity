using Erutan.Scripts.Protos;
using UnityEngine;

namespace Erutan.Scripts.Utils{
    public static class Helper
    {
        public static Quaternion ToQuaternion(this NetQuaternion netQuaternion) 
        {
            return new Quaternion((float)netQuaternion.X, (float)netQuaternion.Y, (float)netQuaternion.Z, (float)netQuaternion.W);
        }
        public static Vector3 ToVector3(this NetVector3 netVector3) 
        {
            return new Vector3((float)netVector3.X, (float)netVector3.Y, (float)netVector3.Z);
        }

        public static Vector2 ToVector2(this NetVector2 netVector2) 
        {
            return new Vector2((float)netVector2.X, (float)netVector2.Y);
        }
    }
}
using UnityEngine;

namespace Erutan.Scripts.Utils
{
    /**
    * Simple protobuf helpers to have cleaner code
    */
    public static class Helper
    {
        public static UnityEngine.Quaternion ToQuaternion(this Protometry.Quaternion q) 
        {
            return new UnityEngine.Quaternion((float) q.X, (float) q.Y, (float) q.Z, (float) q.W);
        }
        public static UnityEngine.Vector3 ToVector3(this Protometry.Vector3 v) 
        {
            return new UnityEngine.Vector3((float) v.X, (float) v.Y, (float) v.Z);
        }

        public static Color ToColor(this Component.Types.RenderComponent.Types.Color c) 
        {
            return new Color(c.Red, c.Green, c.Blue, c.Alpha);
        }
    }
}
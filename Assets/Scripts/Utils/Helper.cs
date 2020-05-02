using Component = Erutan.Component;

namespace Utils
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
        
        public static Protometry.Quaternion ToQuaternion(this UnityEngine.Quaternion q)
        {
            return new Protometry.Quaternion {X = q.x, Y = q.y, Z = q.z, W = q.w};
        }
        public static Protometry.Vector3 ToVector3(this UnityEngine.Vector3 v) 
        {
            return new Protometry.Vector3 { X = v.x, Y = v.y,  Z = v.z};
        }

        public static UnityEngine.Color ToColor(this Component.Types.RenderComponent.Types.Color c) 
        {
            return new UnityEngine.Color(c.Red, c.Green, c.Blue, c.Alpha);
        }
    }
}
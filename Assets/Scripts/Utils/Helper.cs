using Erutan;
using Google.Protobuf.Collections;
using Protometry;
using UnityEngine;

namespace Erutan.Scripts.Utils
{
    /**
    * Simple protobuf helpers to have cleaner code
    */
    public static class Helper
    {
        public static RepeatedField<double> RepeatedField(double[] dimensions)
        {
            return new RepeatedField<double> {dimensions};
        }
        public static VectorN VectorN(double[] dimensions) 
        {
            var v = new VectorN();
            foreach (var d in dimensions)
            {
                v.Dimensions.Add(d);
            }
            return v;
        }
        public static QuaternionN QuaternionN(double[] dimensions) 
        {
            var q = new QuaternionN();
            foreach (var d in dimensions)
            {
                q.Value.Dimensions.Add(d);
            }
            return q;
        }
        // TODO: handle exceptions ?
        public static float GetF(this VectorN v, int dimension) 
        {
            return (float)v.Dimensions[dimension];
        }
        public static double GetD(this VectorN v, int dimension) 
        {
            return v.Dimensions[dimension];
        }
        public static float GetF(this QuaternionN q, int dimension) 
        {
            return q.Value.GetF(dimension);
        }
        public static double GetD(this QuaternionN q, int dimension) 
        {
            return q.Value.GetD(dimension);
        }
        public static Quaternion ToQuaternion(this QuaternionN q) 
        {
            return new Quaternion(q.GetF(0), q.GetF(1), q.GetF(2), q.GetF(3));
        }
        public static Vector3 ToVector3(this VectorN v) 
        {
            return new Vector3(v.GetF(0), v.GetF(1), v.GetF(2));
        }
        public static Vector2 ToVector2(this VectorN v) 
        {
            return new Vector2(v.GetF(0), v.GetF(1));
        }
        
        public static Color ToColor(this Component.Types.RenderComponent c) 
        {
            return new Color(c.Red, c.Green, c.Blue, c.Alpha);
        }
    }
}
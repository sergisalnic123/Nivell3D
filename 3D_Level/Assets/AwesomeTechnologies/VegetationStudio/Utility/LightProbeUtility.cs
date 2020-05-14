using UnityEngine;
using UnityEngine.Rendering;

namespace AwesomeTechnologies.Utility.LightProbes
{
    public class LightProbeUtility
    {
        public struct LightProbeData
        {
            public Vector4 unity_SHAr;
            public Vector4 unity_SHAg;
            public Vector4 unity_SHAb;
            public Vector4 unity_SHBr;
            public Vector4 unity_SHBg;
            public Vector4 unity_SHBb;
            public Vector4 unity_SHC;
        }

        public static LightProbeData GetLightProbeData(SphericalHarmonicsL2 sh)
        {
            return new LightProbeData
            {
                unity_SHAr = Get_Unity_SHAr(sh),
                unity_SHAg = Get_unity_SHAg(sh),
                unity_SHAb = Get_unity_SHAb(sh),
                unity_SHBr = Get_Unity_SHBr(sh),
                unity_SHBg = Get_Unity_SHBg(sh),
                unity_SHBb = Get_Unity_SHBb(sh),
                unity_SHC = Get_Unity_SHC(sh)
            };
        }
        public static Vector4 Get_Unity_SHAr(SphericalHarmonicsL2 sh)
        {
            return new Vector4(sh[0, 3], sh[0, 1], sh[0, 2], sh[0, 0] - sh[0, 6]);
        }

        public static Vector4 Get_unity_SHAg(SphericalHarmonicsL2 sh)
        {
            return new Vector4(sh[1, 3], sh[1, 1], sh[1, 2], sh[1, 0] - sh[1, 6]);
        }

        public static Vector4 Get_unity_SHAb(SphericalHarmonicsL2 sh)
        {
            return new Vector4(sh[2, 3], sh[2, 1], sh[2, 2], sh[2, 0] - sh[2, 6]);
        }

        public static Vector4 Get_Unity_SHBr(SphericalHarmonicsL2 sh)
        {
            return new Vector4(sh[0, 4], sh[0, 5], sh[0, 6] * 3, sh[0, 7]);
        }

        public static Vector4 Get_Unity_SHBg(SphericalHarmonicsL2 sh)
        {
            return new Vector4(sh[1, 4], sh[1, 5], sh[1, 6] * 3, sh[1, 7]);
        }

        public static Vector4 Get_Unity_SHBb(SphericalHarmonicsL2 sh)
        {
            return new Vector4(sh[2, 4], sh[2, 5], sh[2, 6] * 3, sh[2, 7]);
        }

        public static Vector4 Get_Unity_SHC(SphericalHarmonicsL2 sh)
        {
            return new Vector4(sh[0, 8], sh[2, 8], sh[1, 8], 1);
        }
    }
}

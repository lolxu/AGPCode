using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace __OasisBlitz.Utility
{
    public class Interpolate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="y3"></param>
        /// <param name="mu"></param>
        /// <returns></returns>
        public static float CubicInterpolate(float y0, float y1, float y2, float y3, float mu)
        {
            float mu2 = mu * mu;
            float a0 = -0.5f * y0 + 1.5f * y1 - 1.5f * y2 + 0.5f * y3;
            float a1 = y0 - 2.5f * y1 + 2f * y2 - 0.5f * y3;
            float a2 = -0.5f * y0 + 0.5f * y2;
            float a3 = y1;

            return a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3;
        }
    }
}
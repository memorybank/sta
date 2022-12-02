using UnityEngine;
using System;

namespace Playa.Common.Utils
{
    public class MathUtils
    {
        public static float Sigmoid(double value)
        {
            return (float)(1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }
    }
}
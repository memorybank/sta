using System;
using UnityEngine;

namespace Playa.Common.Utils
{
    public class BlenderToUnity
    {
        public static Vector3 BlenderToUnity_Position(float x, float y, float z)
        {
            return new Vector3(-x, z, -y);
        }

        public static Vector3 BlenderToUnity_Position(Vector3 vector3)
        {
            return BlenderToUnity_Position(vector3.x, vector3.y, vector3.z);
        }

        public static Transform GetAmy(Transform itemTransform)
        {
            return GameUtils.FindDeepChild(itemTransform, "Armature");
        }



    }
}
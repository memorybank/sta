using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Common
{

    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        [JsonIgnore]
        public Vector3 UnityVector
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        public SerializableVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
    }

    [System.Serializable]
    public class SerializableQuaternion
    {
        public float w;
        public float x;
        public float y;
        public float z;

        [JsonIgnore]
        public Quaternion UnityQuaternion
        {
            get
            {
                return new Quaternion(x, y, z, w);
            }
        }

        public SerializableQuaternion(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }
    }

}
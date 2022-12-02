using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Animations
{
    [System.Serializable]
    public class ReferenceInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public class MotionExtracter
    {
        public static List<ReferenceInfo> ExtractReference(Animator animator, Transform reference, int dataPoints)
        {
            Debug.Log(string.Format("{0} extract started", reference.name));
            var results = new List<ReferenceInfo>();
            var deltaTime = animator.GetCurrentAnimatorStateInfo(0).length / dataPoints;
            for (int i = 0; i < dataPoints; i++)
            {
                var info = new ReferenceInfo();
                info.position = reference.position;
                info.rotation = reference.rotation;
                results.Add(info);
                animator.Update(deltaTime);
            }
            Debug.Log(string.Format("{0} extract finished", reference.name));
            return results;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars.IK
{
    public class AvatarMeasurements
    {
        public float HeadSize;

        public static AvatarMeasurements Get(Transform avatar)
        {
            var measurements = new AvatarMeasurements();

            foreach (Transform child in avatar.transform)
            {                
                var smr = child.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    child.gameObject.AddComponent<MeshCollider>();
                    child.GetComponent<MeshCollider>().sharedMesh = smr.sharedMesh;
                }
            }

            RaycastHit hit;
            var head = ArmatureUtils.FindPartString(avatar, "Head");
            Physics.Raycast(head.position, head.forward, out hit);
            measurements.HeadSize = hit.distance;
            
            return measurements;
        }
    }

}
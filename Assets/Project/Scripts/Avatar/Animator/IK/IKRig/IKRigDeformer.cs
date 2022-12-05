using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars.IK
{
    // We choose to compute deformation at runtime since most of the time
    // only a small part of animation will be played
    public abstract class IKRigDeformer
    {
        [SerializeField] protected Transform _Avatar;

        public IKRigDeformer(Transform avatar)
        {
            _Avatar = avatar;
        }

        public abstract NormalizedIKPose Deform(NormalizedIKPose pose);
    }

}
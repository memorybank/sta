using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars.IK
{
    public class KeyFrame
    {
        public float originTime;
        public Vector3 destPosition;
        public Vector3 originVelocity;
        public Vector3 destVelocity;

        public KeyFrame(float originTime, Vector3 destPosition, Vector3 originVelocity, Vector3 destVelocity)
        {
            this.originTime = originTime;
            this.destPosition = destPosition;
            this.originVelocity = originVelocity;
            this.destVelocity = destVelocity;
        }
    }

    public class InteractionTarget
    {
        public Transform target;
        public Vector3 offset;
        public float normalizedTargetTime;


        public InteractionTarget(Transform target, Vector3 offset, float normalizedTargetTime)
        {
            this.target = target;
            this.offset = offset;
            this.normalizedTargetTime = normalizedTargetTime;
        }
    }

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
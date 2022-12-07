using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.Common;
using Animancer;

namespace Playa.Avatars.IK
{
    public class InteractionTargetDeformer : IKRigDeformer
    {
        private InteractionTarget _Target;

        public InteractionTargetDeformer(Transform avatar, InteractionTarget target) : base(avatar)
        {
            _Target = target;
        }

        public override NormalizedIKPose Deform(NormalizedIKPose pose)
        {
            var deformed = pose.Copy();

            var t = 0f;
            if (deformed.NormalizedTime - _Target.normalizedTargetTime < 1e-6)
            {
                t = deformed.NormalizedTime / _Target.normalizedTargetTime;
            }
            else
            {
                t = 1f - (deformed.NormalizedTime - _Target.normalizedTargetTime) / (1f - _Target.normalizedTargetTime);
            }

            var a = deformed.LeftHandPosition;
            var b = _Target.target.position + _Target.target.localRotation * _Target.offset - 
                ArmatureUtils.FindPartString(_Avatar, "LeftArm").position;

            deformed.LeftHandPosition = Vector3.Lerp(a, b, t);

            Debug.Log(string.Format("InteractionTargetDeformer {0},{1}=>{2}", a, b, deformed.LeftHandPosition));

            return deformed;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.Common;

namespace Playa.Avatars.IK
{
    // TODO: Extend it to full body and matrix representation
    public class NormalizedIKPose
    {
        public float NormalizedTime;
        public Vector3 LeftElbowPosition;
        public Quaternion LeftElbowRotation;
        public Vector3 LeftHandPosition;
        public Quaternion LeftHandRotation;
        public Vector3 RightElbowPosition;
        public Quaternion RightElbowRotation;
        public Vector3 RightHandPosition;
        public Quaternion RightHandRotation;

        public NormalizedIKPose Copy()
        {
            NormalizedIKPose result = new NormalizedIKPose();
            result.NormalizedTime = NormalizedTime;
            result.LeftElbowPosition = this.LeftElbowPosition;
            result.LeftElbowRotation = this.LeftElbowRotation;
            result.LeftHandPosition = this.LeftHandPosition;
            result.LeftHandRotation = this.LeftHandRotation;
            result.RightElbowPosition = this.RightElbowPosition;
            result.RightElbowRotation = this.RightElbowRotation;
            result.RightHandPosition = this.RightHandPosition;
            result.RightHandRotation = this.RightHandRotation;
            return result;
        }

        public static NormalizedIKPose GetCurrentPose(Transform avatar)
        {
            var pose = new NormalizedIKPose();
            pose.LeftElbowPosition = ArmatureUtils.FindPartString(avatar, "LeftForeArm").position
                - ArmatureUtils.FindPartString(avatar, "LeftArm").position;
            pose.LeftElbowRotation = ArmatureUtils.FindPartString(avatar, "LeftForeArm").localRotation;
            pose.LeftHandPosition = ArmatureUtils.FindPartString(avatar, "LeftHand").position
                - ArmatureUtils.FindPartString(avatar, "LeftArm").position;
            pose.LeftElbowRotation = ArmatureUtils.FindPartString(avatar, "LeftHand").localRotation;
            pose.RightElbowPosition = ArmatureUtils.FindPartString(avatar, "RightForeArm").position
                - ArmatureUtils.FindPartString(avatar, "RightArm").position;
            pose.RightElbowRotation = ArmatureUtils.FindPartString(avatar, "RightForeArm").localRotation;
            pose.RightHandPosition = ArmatureUtils.FindPartString(avatar, "RightHand").position
                - ArmatureUtils.FindPartString(avatar, "RightArm").position;
            pose.RightElbowRotation = ArmatureUtils.FindPartString(avatar, "RightHand").localRotation;
            return pose;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars
{
    [System.Serializable]
    public class EulerAngleDeformerOptions
    {
        public Vector3 LeftArmScale;
        public Vector3 LeftArmTranspose;
        public Vector3 LeftElbowScale;
        public Vector3 LeftElbowTranspose;
        public Vector3 RightArmScale;
        public Vector3 RightArmTranspose;
        public Vector3 RightElbowScale;
        public Vector3 RightElbowTranspose;
        public bool LeftArmEnabled;
        public bool LeftElbowEnabled;
        public bool RightArmEnabled;
        public bool RightElbowEnabled;
    }

    [System.Serializable]
    public class EulerAngleDeformer : PoseDeformer
    {
        private Vector3 _LeftArmBase = new Vector3(50f, 0f, 0f);
        private Vector3 _RightArmBase = new Vector3(50f, 0f, 0f);
        private Vector3 _LeftElbowBase = new Vector3(0f, 0f, -90f);
        private Vector3 _RightElbowBase = new Vector3(0f, 0f, 90f);
        
        [SerializeField] private EulerAngleDeformerOptions _Options;

        public EulerAngleDeformer()
        {
            _Options = CreateDefaultOptions();
        }

        public override void Deform(Transform armature)
        {
            if (_Options.LeftArmEnabled)
            {
                var leftArm = ArmatureUtils.FindPartString(armature, "LeftArm");
                leftArm.localEulerAngles = Vector3.Scale(leftArm.localEulerAngles - _LeftArmBase, _Options.LeftArmScale) + _LeftArmBase + _Options.LeftArmTranspose;
            }
            if (_Options.LeftElbowEnabled)
            {
                var leftElbow = ArmatureUtils.FindPartString(armature, "LeftForeArm");
                leftElbow.localEulerAngles = Vector3.Scale(leftElbow.localEulerAngles - _LeftElbowBase, _Options.LeftElbowScale) + _LeftElbowBase + _Options.LeftElbowTranspose;
            }
            if (_Options.RightArmEnabled)
            {
                var rightArm = ArmatureUtils.FindPartString(armature, "RightArm");
                rightArm.localEulerAngles = Vector3.Scale(rightArm.localEulerAngles - _RightArmBase, _Options.RightArmScale) + _RightArmBase + _Options.RightArmTranspose;
            }
            if (_Options.RightElbowEnabled)
            {
                var rightElbow = ArmatureUtils.FindPartString(armature, "RightForeArm");
                rightElbow.localEulerAngles = Vector3.Scale(rightElbow.localEulerAngles - _RightElbowBase, _Options.RightElbowScale) + _RightElbowBase + _Options.RightElbowTranspose;
            }
        }

        public void SetOptions(EulerAngleDeformerOptions options)
        {
            _Options = options;
        }

        public static EulerAngleDeformerOptions CreateDefaultOptions()
        {
            var options = new EulerAngleDeformerOptions();
            options.LeftArmScale = new Vector3(1f, 1f, 1f);
            options.LeftArmTranspose = new Vector3(0f, 0f, 0f);
            options.LeftElbowScale = new Vector3(1f, 1f, 1f);
            options.LeftElbowTranspose = new Vector3(0f, 0f, 0f);
            options.RightArmScale = new Vector3(1f, 1f, 1f);
            options.RightArmTranspose = new Vector3(0f, 0f, 0f);
            options.RightElbowScale = new Vector3(1f, 1f, 1f);
            options.RightElbowTranspose = new Vector3(0f, 0f, 0f);
            options.LeftArmEnabled = true;
            options.LeftElbowEnabled = true;
            options.RightArmEnabled = true;
            options.RightElbowEnabled = true;
            return options;
    }
    }
}
using Animancer;
using Newtonsoft.Json;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Playa.Avatars.IK
{
    public class IKRigDecoder : MonoBehaviour
    {
        public AnimationClip _Clip;        
        public IKRigDeformer _Deformer;
        public Transform _Playback;

        [SerializeField] private Transform _LeftElbowFollow;
        [SerializeField] private Transform _LeftHandFollow;
        [SerializeField] private Transform _RightElbowFollow;
        [SerializeField] private Transform _RightHandFollow;
        [SerializeField] private Transform _LeftArm;
        [SerializeField] private Transform _RightArm;

        public AnimancerState _State;

        private FullBodyBipedIK _FBIK;

        public void Play()
        {
            var animancerComponent = _Playback.GetComponent<AnimancerComponent>();
            _State = animancerComponent.Play(_Clip, 0.0f, FadeMode.FromStart);
            animancerComponent.Animator.playableGraph.SetTimeUpdateMode(UnityEngine.Playables.DirectorUpdateMode.Manual);
        }

        public void Init()
        {
            _LeftArm = ArmatureUtils.FindPartString(_Playback, "LeftArm");
            _RightArm = ArmatureUtils.FindPartString(_Playback, "RightArm");

            _LeftElbowFollow = new GameObject("LeftElbow").transform;
            _LeftHandFollow = new GameObject("LeftHand").transform;
            _RightElbowFollow = new GameObject("RightElbow").transform;
            _RightHandFollow = new GameObject("RightHand").transform;
            _LeftElbowFollow.position = ArmatureUtils.FindPartString(_Playback, "LeftForeArm").position;
            _LeftHandFollow.position = ArmatureUtils.FindPartString(_Playback, "LeftHand").position;
            _RightElbowFollow.position = ArmatureUtils.FindPartString(_Playback, "RightForeArm").position;
            _RightHandFollow.position = ArmatureUtils.FindPartString(_Playback, "RightHand").position;
            _FBIK = _Playback.GetComponent<FullBodyBipedIK>();
        }

        private void Awake()
        {
            Init();
            SetupTargets();
        }

        private void SetupTargets()
        {
            _FBIK.solver.leftHandEffector.target = _LeftHandFollow;
            _FBIK.solver.leftHandEffector.positionWeight = 1.0f;
            
            /*
            _FBIK.solver.leftHandEffector.rotationWeight = 1.0f;            
            _FBIK.solver.leftArmChain.bendConstraint.bendGoal = _LeftElbowFollow;
            _FBIK.solver.leftArmChain.bendConstraint.weight = 1.0f;
            */

            //_FBIK.solver.rightHandEffector.target = _RightHandFollow;
            //_FBIK.solver.rightHandEffector.positionWeight = 1.0f;
        }

        private void LateUpdate()
        {                        
            if (_Deformer != null)
            {
                var rigData = NormalizedIKPose.GetCurrentPose(_Playback);
                rigData.NormalizedTime = _State.NormalizedTime;
                var hand = rigData.LeftHandPosition;
                rigData = _Deformer.Deform(rigData);
                Debug.Log(string.Format("IKRig Decoder {0}=>{1}", hand, rigData.LeftHandPosition));

                _LeftElbowFollow.position = _LeftArm.position + rigData.LeftElbowPosition;               _LeftElbowFollow.rotation = _LeftArm.rotation * rigData.LeftElbowRotation;
                _LeftElbowFollow.rotation = _LeftArm.rotation * rigData.LeftElbowRotation;
                _LeftHandFollow.position = _LeftArm.position + rigData.LeftHandPosition;
                _LeftHandFollow.rotation = _LeftElbowFollow.rotation * rigData.LeftHandRotation;
                _RightElbowFollow.position = _RightArm.position + rigData.RightElbowPosition;
                _RightElbowFollow.rotation = _RightArm.rotation * rigData.RightElbowRotation;
                _RightHandFollow.position = _RightArm.position + rigData.RightHandPosition;
                _RightHandFollow.rotation = _RightElbowFollow.rotation * rigData.RightHandRotation;
            }
        }
    }
}
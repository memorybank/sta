using Animancer;
using System;
using System.Collections.Generic;
using UnityEngine;

using Playa.Animations;
using System.Linq;
using Playa.Common;
using Playa.Common.Utils;

using cfg.gesture;

namespace Playa.Avatars
{
    public class IDUMetronomicState : AvatarActionState
    {
        [SerializeField] private AnimationRepository _Repo;
        [SerializeField] private AnimationSequenceGenerator _SequenceGenerator;
        [SerializeField] private int _CurrentIndex = -1;
        [SerializeField] private ClipTransition _Clip;

        public AnimationRepository Repo => _Repo;
        public int CurrentIndex => _CurrentIndex;

        public void Awake()
        {
            _Repo.Init();
            _SequenceGenerator.Init();
        }

        public override bool CanEnterState
        {
            get
            {
                Debug.Assert(AvatarUser.AvatarBrainGestureBehavior is MetronomicGestureBehavior);
                if (((MetronomicGestureBehavior)AvatarUser.AvatarBrainGestureBehavior).NewSequence)
                {
                    _CurrentIndex = AvatarUser.MatchBrainBehavior(AvatarStateType.IDUMetronomic);
                    // Should always find a closest choice
                    Debug.Assert(_CurrentIndex > -1);
                    return _CurrentIndex > -1;
                }
                else
                {
                    return true;
                }
            }
        }

        private void OnEnable()
        {
            if (((MetronomicGestureBehavior)AvatarUser.AvatarBrainGestureBehavior).NewSequence) {
                _SequenceGenerator.PickStartPoint(_CurrentIndex);
                ((MetronomicGestureBehavior)AvatarUser.AvatarBrainGestureBehavior).NewSequence = false;
            }

            // comment: reflect
            switch (_SequenceGenerator)
            { 
                case MetronomicSequenceGenerator _:
                    _Clip = ((MetronomicSequenceGenerator)_SequenceGenerator).GenCurrentClip();
                    break;
                case InteractMetronomicSequenceGenerator _:
                    _Clip = ((InteractMetronomicSequenceGenerator)_SequenceGenerator).GetCurrentClip();
                    break;
            }
            
            ChainNextAnimation();
        }

        private void ChainNextAnimation()
        {
            var fadeTime = UserStateTransitionConstants.ActionLayerCommonTransitionFadeDuration * AvatarUser.AvatarBrain.GetSpeed();
            AvatarLayeredAnimationManager.SetActionLayerMask(((GestureClipInfo)_Repo.AnimationClipInfos[_Clip.Clip.name]).AvatarMask);
            AvatarLayeredAnimationManager.PlayAction(_Clip, fadeTime, FadeMode.FromStart);
            // metronomic pose should not be looping
            Debug.Assert(!_Clip.Clip.isLooping);
            var state = AvatarLayeredAnimationManager.CurrentActionLayerState();
            if (AvatarUser.MetronomicLoopToggle.isOn)
            {
                state.Events.OnEnd = TryReEnterStateLog;
            }
        }

        private void LogBehaviorReEnter(bool isSwitchToNext)
        {
            AvatarUser.BehaviorPlanner.LogBehavior(AvatarBehaviorStateType.MetronomicGestureBehavior, _SequenceGenerator.CurrentClipName, TimeUtils.GetMSTimestamp(), isSwitchToNext);
            AvatarUser.SetBrainGestureBehavior(new MetronomicGestureBehavior());
            AvatarUser.GetStateFunction(StateActionType.ReEnterIDUMonotronic)?.Invoke();
        }

        public void TryReEnterStateLog()
        {
            bool isNext = false;
            if (((GestureClipInfo)_Repo.AnimationClipInfos[_Clip.Clip.name]).GestureMark.IsLoop != IsLoop.LOOP)
            {
                _Clip = _SequenceGenerator.GenNextClip();
                isNext = true;
            }
            LogBehaviorReEnter(isNext);
        }

        public void TryReEnterStateLog(bool isForceToNext)
        {
            if (isForceToNext)
            {
                if (_CurrentIndex == -1)
                {
                    Debug.LogWarning("IDUMetronomicState.TryReEnterStateLog current index not init");

                    //TODO:后续match和reEnterState解耦
                    _CurrentIndex = AvatarUser.MatchBrainBehavior(AvatarStateType.IDUMetronomic);
                    _SequenceGenerator.PickStartPoint(_CurrentIndex);
                }
                _Clip = _SequenceGenerator.GenNextClip();
            }

            LogBehaviorReEnter(isForceToNext);
        }

    }

}
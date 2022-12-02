using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playa.Animations;
using Animancer.FSM;
using TMPro;
using Playa.Common;
using Playa.Common.Utils;

namespace Playa.Avatars
{
    using ItemId = UInt64;
    public class GestureBehaviorPlanner : MonoBehaviour
    {
        // Internal
        private float TPS;

        protected const float _DefaultTPS = 5.0f;

        protected const float _MaxSpeed = 2f;

        protected const float _MinSpeed = 0.7f;

        protected float _BaseTPS = _DefaultTPS;

        private float _SigmoidPIDTargetValue;

        // ui component
        [SerializeField] private TextMeshProUGUI _tpsText;
        [SerializeField] private TMP_InputField _tpsBaseInput;

        // Parent Pointer
        [SerializeField] private AvatarUser _AvatarUser;
        public AvatarUser AvatarUser => _AvatarUser;

        // Source
        public AvatarBrain AvatarBrain => _AvatarUser.AvatarBrain;

        // Destination
        public Action ForceIdleState { get; private set; }
        public Action BackToIDUMonotronic { get; private set; }
        public Action ReEnterIDUMonotronic { get; private set; }
        public Action ReEnterNextIDUMonotronic { get; private set; }

        public Action ReEnterSilence { get; private set; }

        public Action BackToIdle { get; private set; }


        public StateMachine<AvatarActionState> AvatarActionStateMachine => _AvatarUser.AvatarActionStateMachine;

        public StateMachine<AvatarBaseState> AvatarBaseStateMachine => _AvatarUser.AvatarBaseStateMachine;

        public float AllowSwitchNormalizedPlayDepth = 0.48f;

        [SerializeField] private AnimationRepository StartAnimationRepository;
        [SerializeField] private AnimationRepository FinishAnimationRepository;
        [SerializeField] private AnimationSequenceGenerator IDUMetronomicAnimationRepository;
        [SerializeField] private AnimationRepository IDUStrokeAnimationRepository;

        //c#没有template specialization,之后根据使用情况提交fix
        private Matcher<AnimationRepository> _StartTalkingMatcher;
        private Matcher<AnimationRepository> _FinishTalkingMatcher;
        private Matcher<AnimationSequenceGenerator> _IDUMetronomicMatcher;
        private Matcher<AnimationRepository> _IDUStrokeMatcher;

        public Matcher<AnimationRepository> IDUStrokeMatcher => _IDUStrokeMatcher;

        public AvatarLayeredAnimationManager AvatarLayeredAnimationManager;

        private void Awake()
        {
            ForceIdleState = () =>
            {
                AvatarUser.AvatarBaseStateMachine.ForceSetState(AvatarUser.AvatarAnimator.BaseIdleState);
                AvatarUser.AvatarActionStateMachine.ForceSetState(AvatarUser.AvatarAnimator.ActionIdleState);
            };
            BackToIDUMonotronic = () =>
            {
                AvatarBrain.Behavior.GestureBehavior = new MetronomicGestureBehavior();
                ((MetronomicGestureBehavior)AvatarBrain.Behavior.GestureBehavior).NewSequence = true;
                ((AvatarActionState)AvatarUser.GetAvatarState(AvatarStateType.IDUMetronomic)).TryReEnterState();
            };
            BackToIdle = () =>
            {
                ((AvatarBaseState)AvatarUser.GetAvatarState(AvatarStateType.BaseIdle)).TryReEnterState();
                if (AvatarUser.AvatarActionStateMachine.CurrentState is ActionIdleState)
                {
                    ((AvatarActionState)AvatarUser.GetAvatarState(AvatarStateType.ActionIdle)).TryReEnterState();
                }
            };
            ReEnterIDUMonotronic = () =>
            {
                ((AvatarActionState)AvatarUser.GetAvatarState(AvatarStateType.IDUMetronomic)).TryReEnterState();
            };
            ReEnterNextIDUMonotronic = () =>
            {
                ((IDUMetronomicState)AvatarUser.GetAvatarState(AvatarStateType.IDUMetronomic)).TryReEnterStateLog(true);
            };
            ReEnterSilence = () => ((AvatarActionState)AvatarUser.GetAvatarState(AvatarStateType.Silence)).TryReEnterState();

            _StartTalkingMatcher = new SimpleRandomMatcher(StartAnimationRepository);
            _FinishTalkingMatcher = new SimpleRandomMatcher(FinishAnimationRepository);
            _IDUMetronomicMatcher = new RandomSequenceMatcher(IDUMetronomicAnimationRepository);
            _IDUStrokeMatcher = new KeywordMatcher(IDUStrokeAnimationRepository);

            _tpsBaseInput.onValueChanged.AddListener(OnTPSBaseInputChanged);

        }

        protected void OnTPSBaseInputChanged(string value)
        {
            if (!_AvatarUser.IsActive)
            {
                return;
            }

            float tps;
            tps = float.Parse(value);
            if (Mathf.Abs(tps - 0.0f) < 1e-6)
            {
                return;
            }

            _BaseTPS = _DefaultTPS / tps;
        }

        public void ApplyIntentTPS(float targetTPS)
        {
            if (AvatarUser.AvatarActivityType == AvatarActivityType.IsPausing)
            {
                return;
            }

            _SigmoidPIDTargetValue = _MinSpeed + MathUtils.Sigmoid(0.6 * (targetTPS - _BaseTPS)) * (_MaxSpeed - _MinSpeed);
            SetAvatarAnimancerSpeed(_SigmoidPIDTargetValue);
            _tpsText.text = _SigmoidPIDTargetValue.ToString();
        }

        public void MustSetTheIntentTPS(float toSet, bool isPause)
        {
            _SigmoidPIDTargetValue = toSet;
            if (isPause)
            {
                AvatarUser.AvatarActivityType = AvatarActivityType.IsPausing;
            }

            SetAvatarAnimancerSpeed(toSet);
            _tpsText.text = toSet.ToString();
        }
        public int BehaviorPlannerMatchBrainBehavior(AvatarStateType type)
        {
            switch (type)
            {
                case AvatarStateType.ActionIdle:
                    return -1;
                case AvatarStateType.StartTalking:
                    return _StartTalkingMatcher.match(AvatarBrain.Behavior).AnimationClipIndex;
                case AvatarStateType.IDUMetronomic:
                    return _IDUMetronomicMatcher.match(AvatarBrain.Behavior).AnimationClipIndex;
                case AvatarStateType.IDURelax:
                    return -1;
                case AvatarStateType.IDUStroke:
                    return _IDUStrokeMatcher.match(AvatarBrain.Behavior).AnimationClipIndex;
                case AvatarStateType.FinishTalking:
                    return _FinishTalkingMatcher.match(AvatarBrain.Behavior).AnimationClipIndex;
            }
            return -1;
        }

        public void SetBrainGestureBehavior(GestureBehavior gestureBehavior)
        {
            AvatarBrain.Behavior.GestureBehavior = gestureBehavior;
        }
        public void SetAvatarAnimancerSpeed(float speed)
        {
            AvatarUser.AvatarAnimator.Animancer.Layers[1].Speed = speed;
            _tpsText.text = speed.ToString();
        }

        public float GetAvatarAnimancerSpeed()
        {
            return AvatarUser.AvatarAnimator.Animancer.Layers[1].Speed;
        }

        //layer status manager
        public void OnMaskInit(AvatarMask avatarMask)
        {
            if (AvatarActionStateMachine.CurrentState is not IDUMetronomicState &&
                AvatarActionStateMachine.CurrentState is not StrokeState &&
                AvatarLayeredAnimationManager.IsAnyPlayingAction())
            {
                AvatarLayeredAnimationManager.FadeOutUpperBodyWithCustomDuration(UserStateTransitionConstants.IdleStateFastFadeDuration);
            }

            AvatarLayeredAnimationManager.SetSubLayerMask(avatarMask);
            AvatarLayeredAnimationManager.SetActionLayerMask(avatarMask);
            AvatarLayeredAnimationManager.SetInteractionLayerMask(avatarMask);
        }
        public ItemStatusGroup GetStatusGroup(AvatarStateType stateType, int groupIndex=0)
        {
            //todo refactor
            AvatarState state = AvatarUser.GetAvatarState(stateType);
            if (state != null)
            {
                switch (stateType)
                {
                    case AvatarStateType.BaseIdle:
                        return ((BaseIdleState)state).IdleStatusGroup;
                        if (groupIndex == 1)
                        {
                            return ((BaseIdleState)state).SecondIdleStatusGroup;
                        }
                    case AvatarStateType.ActionIdle:
                        return ((ActionIdleState)state).ActionStatusGroup;
                    case AvatarStateType.Silence:
                        return ((SilenceState)state).SilenceStatusGroup;
                    default:
                        return null;
                }
               
            }
            return null;
        }

        public void SetStatusGroup(ItemId itemid, AvatarStateType stateType, ItemStatusGroup group, ItemStatusGroup secondGroup=null)
        {
            AvatarState state = AvatarUser.GetAvatarState(stateType);
            if (state != null)
            {
                switch (stateType)
                {
                    case AvatarStateType.BaseIdle:
                        ((BaseIdleState)state).SetStatusGroup(itemid, group, secondGroup);
                        BackToIdle();
                        break;
                    case AvatarStateType.ActionIdle:
                        ((ActionIdleState)state).SetStatusGroup(itemid, group);
                        if (AvatarActionStateMachine.CurrentState == state)
                        {
                            ((AvatarActionState)AvatarUser.GetAvatarState(AvatarStateType.ActionIdle)).TryReEnterState();
                        }
                        break;
                    case AvatarStateType.Silence:
                        ((SilenceState)state).SetStatusGroup(itemid, group);
                        if (AvatarActionStateMachine.CurrentState == state)
                        {
                            ReEnterSilence();
                        }
                        break;
                }
                
            }
        }

        public ItemIdleSubStatusGroup GetIdleSubStatusGroup()
        {
            return AvatarLayeredAnimationManager.IdleSubStatusGroup;
        }

        public void SetSubStatus(ItemId id, ItemIdleSubStatusGroup idleSubStatus, int priority)
        {
            AvatarLayeredAnimationManager.SetSubStatusAnim(id, idleSubStatus, priority);
        }
    }
}

using Animancer;
using System.Collections.Generic;
using UnityEngine;

using Playa.Animations;
using System.Linq;
using Playa.Common;

namespace Playa.Avatars
{
    public class FinishTalkingState : AvatarActionState
    {
        [SerializeField] private AnimationRepository _Repo;
        [SerializeField] private int _AnimationIndex = -1;

        public AnimationRepository Repo
        {
            get => _Repo;
            set
            {
                _Repo = value;
            }
        }

        public int AnimationIndex
        {
            get => _AnimationIndex;
            set
            {
                _AnimationIndex = value;
            }
        }

        public void Awake()
        {
            _Repo.Init();
        }

        public override bool CanEnterState
        {
            get
            {
                // FinishTalking state should be able to preempt any state other than idle
                if (Avatar.ActionStateMachine.CurrentState.GetType() == typeof(ActionIdleState))
                {
                    return false;
                }

                _AnimationIndex = AvatarUser.MatchBrainBehavior(AvatarStateType.FinishTalking);
                // Should always find a closest choice
                Debug.Assert(_AnimationIndex > -1);
                return _AnimationIndex > -1;
            }
        }

        private void OnEnable()
        {
            var animation = _Repo.AnimationClips[_AnimationIndex];
            AvatarLayeredAnimationManager.PlayAction(animation, UserStateTransitionConstants.CommonTransitionFadeNormalizedDuration, FadeMode.NormalizedFromStart);
            // finish talking pose shouldn't be looping
            Debug.Assert(!animation.Clip.isLooping);

            var state = AvatarLayeredAnimationManager.CurrentActionLayerState();
            state.Events.OnEnd = () =>
            {
                AvatarLayeredAnimationManager.FadeOutUpperBodyWithCustomDuration(UserStateTransitionConstants.ActionLayerTransitionFadeDuration);
                AvatarUser.GetStateFunction(StateActionType.ReEnterNextIDUMonotronic)?.Invoke();
            };

            _AnimationIndex = -1;
        }

    }

}
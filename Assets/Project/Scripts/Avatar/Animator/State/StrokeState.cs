using Animancer;
using System.Collections.Generic;
using UnityEngine;

using Playa.Animations;
using TMPro;
using Playa.Common;

namespace Playa.Avatars
{
    public class StrokeState : AvatarActionState
    {
        [SerializeField] private AnimationRepository _Repo;
        [SerializeField] private int _AnimationIndex = 0;

        // UI components
        [SerializeField] private TextMeshProUGUI _Keyword;

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

        // Only enters this state if has matched animation
        public override bool CanEnterState {
            get 
            {
                _AnimationIndex = AvatarUser.MatchBrainBehavior(AvatarStateType.IDUStroke);

                return _AnimationIndex > -1;
            }
        }

        private void OnEnable()
        {
            Debug.Assert(_AnimationIndex > -1);
            _Keyword.text = ((StrokeGestureBehavior)AvatarUser.AvatarBrainGestureBehavior).Keyword;

            var animation = _Repo.AnimationClips[_AnimationIndex];
            var fadeTime = UserStateTransitionConstants.ActionLayerCommonTransitionFadeDuration * AvatarUser.AvatarBrain.GetSpeed();
            AvatarLayeredAnimationManager.SetActionLayerMask(((GestureClipInfo)_Repo.AnimationClipInfos[animation.Clip.name]).AvatarMask);
            AvatarLayeredAnimationManager.PlayAction(animation, fadeTime, FadeMode.FromStart);

            Debug.Log("Stroke state playing " + _Keyword.text);

            // stroke pose shouldn't be looping
            Debug.Assert(!animation.Clip.isLooping);
            var state = AvatarLayeredAnimationManager.CurrentActionLayerState();
            state.Events.OnEnd = () =>
            {
                AvatarLayeredAnimationManager.FadeOutUpperBodyWithCustomDuration(UserStateTransitionConstants.ActionLayerTransitionFadeDuration);
                AvatarUser.GetStateFunction(StateActionType.BackToIDUMonotronic)?.Invoke();
            };
        }
    }

}
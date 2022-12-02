using System;
using Animancer;
using Playa.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Playa.Common;
using Playa.App;

namespace Playa.Avatars
{
    using ItemId = UInt64;
    public sealed class BaseIdleState : AvatarBaseState
    {
        [SerializeField] private ItemIdleStatusGroup _IdleStatusGroup;
        [SerializeField] private ItemSecondIdleStatusGroup _SecondIdleStatusGroup;
        public ItemIdleStatusGroup IdleStatusGroup => _IdleStatusGroup;
        public ItemSecondIdleStatusGroup SecondIdleStatusGroup => _SecondIdleStatusGroup;
        

        private int _StatusIndex => Avatar.IdleStatusIndex;

        private bool _CanPlayActionFullBody;

        public void Awake()
        {
            _GroupOptions = new OptionsStore(new List<OptionClass>(), "BaseIdleGroup");
            _GroupOptions.AddOption(new BaseGroupOption(0, _IdleStatusGroup, _SecondIdleStatusGroup));
        }

        private void OnEnable()
        {
            float fadeTime = UserStateTransitionConstants.IdleStateFastFadeDuration;
            ClipTransition toPlayTransition;
            BaseGroupOption baseIdleGroupOption = (BaseGroupOption)_GroupOptions.GetOption();
            _IdleStatusGroup = baseIdleGroupOption.IdleStatusGroup;
            _SecondIdleStatusGroup = baseIdleGroupOption.IdleSecondStatusGroup;
            AvatarMask toSetBaseMask = _IdleStatusGroup._AvatarMasks[_StatusIndex];
            toPlayTransition = _IdleStatusGroup._StatusAnimations[_StatusIndex];
            if (AvatarUser.AvatarActivityType != AvatarActivityType.IsIdle)
            {
                toPlayTransition = _SecondIdleStatusGroup._StatusAnimations[_StatusIndex];
                fadeTime = UserStateTransitionConstants.IdleStateFastFadeDuration;
                toSetBaseMask = _SecondIdleStatusGroup._AvatarMasks[_StatusIndex];
            }

            AvatarLayeredAnimationManager.PlayBase(toPlayTransition, false, fadeTime, FadeMode.NormalizedFromStart);
            AvatarLayeredAnimationManager.SetBaseLayerMask(toSetBaseMask);
            var state = AvatarLayeredAnimationManager.CurrentBaseLayerState();
            state.Events.OnEnd = () =>
            {
                AvatarUser.GetStateFunction(StateActionType.BackToIdle)?.Invoke();
            };
        }
    }

    
}
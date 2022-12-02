using System;
using Animancer;
using Playa.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Playa.Common;
using Random = UnityEngine.Random;
using Animancer.FSM;
using Playa.App;

namespace Playa.Avatars
{
    using ItemId = UInt64;
    public sealed class ActionIdleState : AvatarActionState
    {
        private float fadeDuration = AnimancerPlayable.DefaultFadeDuration;

        [SerializeField] private ItemActionStatusGroup _ActionStatusGroup;
        private int _StatusIndex => Avatar.ActionStatusIndex;

        public ItemActionStatusGroup ActionStatusGroup => _ActionStatusGroup;

        private void Awake()
        {
            fadeDuration = UserStateTransitionConstants.ActionLayerFinishTalkingFadeDuration;
            _GroupOptions = new OptionsStore(new List<OptionClass>(), "ActionIdleGroup");
        }

        private void OnEnable()
        {
            ActionGroupOption ago = (ActionGroupOption)_GroupOptions.GetOption();
            if (ago != null)
            {
                _ActionStatusGroup = ago.ActionStatusGroup;
            }
            else
            {
                _ActionStatusGroup = new ItemActionStatusGroup();
            }
            if (_ActionStatusGroup._StatusAnimations != null && _StatusIndex < _ActionStatusGroup._StatusAnimations.Length)
            {
                float fadeTime = UserStateTransitionConstants.IdleStateFastFadeDuration;
                AvatarLayeredAnimationManager.SetActionLayerMask(_ActionStatusGroup._AvatarMasks[_StatusIndex]);
                var state = AvatarLayeredAnimationManager.CurrentActionLayerState();
                AvatarLayeredAnimationManager.PlayAction(_ActionStatusGroup._StatusAnimations[_StatusIndex], fadeTime, FadeMode.FromStart);
                //comment: null => not null
                state = AvatarLayeredAnimationManager.CurrentActionLayerState();
                state.Events.OnEnd = () =>
                {
                    if (_Avatar.ActionStateMachine.CurrentState == this)
                    {
                        ((AvatarActionState)AvatarUser.GetAvatarState(AvatarStateType.ActionIdle)).TryReEnterState();
                    }
                };
            }
            else
            {
                AvatarLayeredAnimationManager.FadeOutUpperBodyWithCustomDuration(fadeDuration);
            }
        }

        public void SetStatusGroup(ItemId id, ItemStatusGroup group)
        {
            if (group != null)
            {
                _GroupOptions.AddOption(new ActionGroupOption(id, (ItemActionStatusGroup)group));
            }
            else
            {
                _GroupOptions.RemoveOption(new ActionGroupOption(id, null));
            }
        }
    }

    public class ActionGroupOption : OptionClass
    {
        private ItemId _ItemId;
        private ItemActionStatusGroup _ActionStatusGroup;
        public ItemId ItemId => _ItemId;
        public ItemActionStatusGroup ActionStatusGroup => _ActionStatusGroup;
        public ActionGroupOption(ItemId itemid, ItemActionStatusGroup idleStatusGroup)
        {
            _ItemId = itemid;
            _ActionStatusGroup = idleStatusGroup;
        }

        public override string ToString()
        {
            return string.Format("ActionIdleGroupOption({0},{1})", ItemId, _ActionStatusGroup);
        }

        public override bool Compare(OptionClass obj)
        {
            Debug.Assert(obj is ActionGroupOption, string.Format("Option Manager : try to compare {0} with {1}", this, obj));
            bool result = true;
            ActionGroupOption bigo = (ActionGroupOption)obj;
            if (_ItemId != bigo.ItemId)
            {
                result = false;
            }
            Debug.Log(string.Format("Option Compare : {0} {1}equals to {2}", ToString(), (result ? "" : "not "), bigo.ToString()));
            return result;
        }

        public override void OnUpdate()
        {
            //do nothing
        }

        public override void OnRemove()
        {
            //do nothing
        }
    }
}
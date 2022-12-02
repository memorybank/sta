using System;
using Animancer;
using Playa.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Playa.Common;
using Playa.App;
using Random = UnityEngine.Random;

namespace Playa.Avatars
{
    using ItemId = UInt64;
    public sealed class SilenceState : AvatarActionState
    {
        [SerializeField] private ItemSilenceStatusGroup _SilenceStatusGroup;

        public ItemSilenceStatusGroup SilenceStatusGroup => _SilenceStatusGroup;

        private int _StatusIndex => Avatar.SilenceStatusIndex;

        public void Awake()
        {
            _GroupOptions = new OptionsStore(new List<OptionClass>(), "SilenceGroup");
        }

        private void OnEnable()
        {
            SilenceGroupOption sgo = (SilenceGroupOption)_GroupOptions.GetOption();
            if (sgo != null)
            {
                _SilenceStatusGroup = sgo.SilenceStatusGroup;
            }
            else
            {
                _SilenceStatusGroup = new ItemSilenceStatusGroup();
            }
            if ( _SilenceStatusGroup._StatusAnimations != null && _StatusIndex < _SilenceStatusGroup._StatusAnimations.Length)
            {
                float fadeTime = UserStateTransitionConstants.SilenceStateFastFadeDuration;
                AvatarLayeredAnimationManager.SetActionLayerMask(_SilenceStatusGroup._AvatarMasks[_StatusIndex]);
                var state = AvatarLayeredAnimationManager.CurrentActionLayerState();
                AvatarLayeredAnimationManager.PlayAction(_SilenceStatusGroup._StatusAnimations[_StatusIndex], fadeTime,FadeMode.FromStart);
                state = AvatarLayeredAnimationManager.CurrentActionLayerState();
                state.Events.OnEnd = () =>
                {
                    if (_Avatar.ActionStateMachine.CurrentState == this)
                    {
                        AvatarUser.GetStateFunction(StateActionType.ReEnterSilence)?.Invoke();
                    }
                };
            }
            else
            {
                AvatarLayeredAnimationManager.FadeOutUpperBodyWithCustomDuration(AnimancerPlayable.DefaultFadeDuration);
            }
        }

        public void SetStatusGroup(ItemId id, ItemStatusGroup group)
        {
            if (group != null)
            {
                _GroupOptions.AddOption(new SilenceGroupOption(id, (ItemSilenceStatusGroup)group));
            }
            else
            {
                _GroupOptions.RemoveOption(new SilenceGroupOption(id, null));
            }
        }
    }
    public class SilenceGroupOption : OptionClass
    {
        private ItemId _ItemId;
        private ItemSilenceStatusGroup _SilenceStatusGroup;
        public ItemId ItemId => _ItemId;
        public ItemSilenceStatusGroup SilenceStatusGroup => _SilenceStatusGroup;
        public SilenceGroupOption(ItemId itemid, ItemSilenceStatusGroup idleStatusGroup)
        {
            _ItemId = itemid;
            _SilenceStatusGroup = idleStatusGroup;
        }

        public override string ToString()
        {
            return string.Format("SilenceGroupOption({0},{1})", ItemId, _SilenceStatusGroup);
        }

        public override bool Compare(OptionClass obj)
        {
            Debug.Assert(obj is SilenceGroupOption, string.Format("Option Manager : try to compare {0} with {1}", this, obj));
            bool result = true;
            SilenceGroupOption bigo = (SilenceGroupOption)obj;
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
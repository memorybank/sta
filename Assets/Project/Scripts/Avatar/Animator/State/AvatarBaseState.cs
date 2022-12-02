using Animancer;
using Animancer.FSM;
using System;
using System.Collections.Generic;
using UnityEngine;
using Playa.Common;
using Playa.App;

namespace Playa.Avatars
{
    using ItemId = UInt64;
    public abstract class AvatarBaseState : AvatarState, IOwnedState<AvatarBaseState>
    {
        protected OptionsStore _GroupOptions;
        public AvatarAnimator Avatar
        {
            get => _Avatar;
            set
            {
                if (_Avatar != null &&
                    _Avatar.BaseStateMachine.CurrentState == this)
                    ((AvatarBaseState)_AvatarUser.GetAvatarState(AvatarStateType.BaseIdle)).ForceEnterState();

                _Avatar = value;
            }
        }

        public AvatarUser AvatarUser
        {
            get => _AvatarUser;
            set
            {
                if (_AvatarUser != null &&
                    _Avatar.BaseStateMachine.CurrentState == this)
                    _AvatarUser = value;
            }
        }

        public StateMachine<AvatarBaseState> OwnerStateMachine => _Avatar.BaseStateMachine;

        public void SetStatusGroup(ItemId itemid, ItemStatusGroup group, ItemStatusGroup secondGroup) {
            if (group != null)
            {
                _GroupOptions.AddOption(new BaseGroupOption(itemid, (ItemIdleStatusGroup)group, (ItemSecondIdleStatusGroup)secondGroup));
            }
            else
            {
                _GroupOptions.RemoveOption(new BaseGroupOption(itemid, null, null));
            }
        }
    }

    public class BaseGroupOption : OptionClass
    {
        private ItemId _ItemId;
        private ItemIdleStatusGroup _IdleStatusGroup;
        private ItemSecondIdleStatusGroup _IdleSecondStatusGroup;

        public ItemId ItemId => _ItemId;
        public ItemIdleStatusGroup IdleStatusGroup => _IdleStatusGroup;
        public ItemSecondIdleStatusGroup IdleSecondStatusGroup => _IdleSecondStatusGroup;

        public BaseGroupOption(ItemId itemid, ItemIdleStatusGroup idleStatusGroup, ItemSecondIdleStatusGroup idleSecondStatusGroup)
        {
            _ItemId = itemid;
            _IdleStatusGroup = idleStatusGroup;
            _IdleSecondStatusGroup = idleSecondStatusGroup;
        }

        public override string ToString()
        {
            return string.Format("BaseIdleGroupOption({0},{1},{2})", ItemId, _IdleStatusGroup, IdleSecondStatusGroup);
        }

        public override bool Compare(OptionClass obj)
        {
            Debug.Assert(obj is BaseGroupOption, string.Format("Option Manager : try to compare {0} with {1}", this, obj));
            bool result = true;
            BaseGroupOption bigo = (BaseGroupOption)obj;
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
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
    public abstract class AvatarActionState : AvatarState, IOwnedState<AvatarActionState>
    {
        protected OptionsStore _GroupOptions;
        public AvatarAnimator Avatar
        {
            get => _Avatar;
            set
            {
                if (_Avatar != null &&
                    _Avatar.ActionStateMachine.CurrentState == this)
                    ((AvatarActionState)_AvatarUser.GetAvatarState(AvatarStateType.ActionIdle)).ForceEnterState();

                _Avatar = value;
            }
        }
        public AvatarUser AvatarUser
        {
            get => _AvatarUser;
            set
            {
                if (_AvatarUser != null &&
                    _Avatar.ActionStateMachine.CurrentState == this)
                    ((AvatarActionState)_AvatarUser.GetAvatarState(AvatarStateType.IDUMetronomic)).ForceEnterState();

                _AvatarUser = value;
            }
        }

        public StateMachine<AvatarActionState> OwnerStateMachine => _Avatar.ActionStateMachine;

    }
}
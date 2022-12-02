using Animancer;
using Animancer.FSM;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars
{
    public abstract class AvatarState : StateBehaviour
    {
        [SerializeField]
        protected AvatarAnimator _Avatar;

        [SerializeField]
        protected AvatarUser _AvatarUser;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            _Avatar = gameObject.GetComponentInParentOrChildren<AvatarAnimator>();
        }
#endif

        public AvatarLayeredAnimationManager AvatarLayeredAnimationManager;
    }
}
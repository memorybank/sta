using System;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace Playa.Common
{
    [Serializable]
    public class ItemStatusGroup
    {
        [SerializeField] public ClipTransition[] _StatusAnimations;
        [SerializeField] public AvatarMask[] _AvatarMasks;

        public ItemStatusGroup Get()
        { 
            return this;
        }

        public void Copy(ItemStatusGroup itemStatusGroup)
        {
            if (itemStatusGroup._StatusAnimations != null)
            {
                _StatusAnimations = new ClipTransition[itemStatusGroup._StatusAnimations.Length];
                Array.Copy(itemStatusGroup._StatusAnimations, _StatusAnimations, itemStatusGroup._StatusAnimations.Length);
            }

            if (itemStatusGroup._AvatarMasks != null)
            {
                _AvatarMasks = new AvatarMask[itemStatusGroup._AvatarMasks.Length];
                Array.Copy(itemStatusGroup._AvatarMasks, _AvatarMasks, itemStatusGroup._AvatarMasks.Length);
            }
        }
    }
    [Serializable]
    public class ItemIdleStatusGroup:ItemStatusGroup
    {
    }

    [Serializable]
    public class ItemSecondIdleStatusGroup:ItemStatusGroup
    {
    }

    [Serializable]
    public class ItemIdleSubStatusGroup:ItemStatusGroup
    {
    }

    [Serializable]
    public class ItemSilenceStatusGroup:ItemStatusGroup
    {
    }

    [Serializable]
    public class ItemActionStatusGroup:ItemStatusGroup
    {
    }

    [Serializable]
    public class ItemInteractionStatusGroup : ItemStatusGroup
    {
    }

}
using Playa.Avatars;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RootMotion.FinalIK;
using Playa.Common;
using Playa.Avatars.IK;
using Playa.Item;

namespace Playa.App.Actors
{
    using ItemId = UInt64;
    using InteractionId = UInt64;

    public abstract class ActorsApi : MonoBehaviour
    {
        // Single actor Apis
        abstract public Transform GetAvatarPosition(AvatarUser user);
        abstract public void SetAvatarIdleStatus(ItemId id, AvatarUser user, ItemIdleStatusGroup group, ItemSecondIdleStatusGroup secondGroup);
        abstract public void SetAvatarIdleSubStatus(ItemId id, AvatarUser user, ItemIdleSubStatusGroup group, int priority);
        abstract public void SetAvatarSilenceStatus(ItemId id, AvatarUser user, ItemSilenceStatusGroup group);
        abstract public void SetAvatarActionStatus(ItemId id, AvatarUser user, ItemActionStatusGroup group);

        //todo ik object init by asset path

        abstract public void SetIKLookAtObject(ItemId item_id, VoiceActivityType voiceActivityType, AvatarUser avatarUser, GameObject lookAtGameObject, int priority, float headWeight, float bodyWeight);

        abstract public void SetAvatarPrefab(AvatarUser user, int index);

        abstract public void RegisterSlots(BaseItem item, AvatarUser user, List<SlotName> slotNames, Dictionary<IKEffectorName, IKTarget> targets);
        abstract public void UpdateSlots(BaseItem item, AvatarUser user, List<SlotName> slotNames, Dictionary<IKEffectorName, IKTarget> targets);

        abstract public void ClearSlots(AvatarUser user, List<SlotName> slotNames);

        // Multiple actors apis
        abstract public void SetAvatarPositionSlot(BaseItem item, int priority, AvatarUser user, int slotIndex, int userIndex);

        abstract public void RemoveAvatarPositionSlot(BaseItem item, AvatarUser user, int slotIndex);
    }

}
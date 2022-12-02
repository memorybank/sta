using Playa.App.Stage;
using System.Collections;
using System.Collections.Generic;
using Playa.Avatars;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.Events;
using Playa.Common;

namespace Playa.App
{
    public class ActorsCmd { }

    public class SetAvatarIdleStatusCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public ItemIdleStatusGroup group;
        public ItemSecondIdleStatusGroup secondGroup;

        public SetAvatarIdleStatusCmd(AvatarUser avatarUser, ItemIdleStatusGroup group, ItemSecondIdleStatusGroup secondGroup)
        {
            this.avatarUser = avatarUser;
            this.group = group;
            this.secondGroup = secondGroup;
        }
    }

    public class SetAvatarIdleSubStatusCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public ItemIdleSubStatusGroup group;
        public int priority;

        public SetAvatarIdleSubStatusCmd(AvatarUser avatarUser, ItemIdleSubStatusGroup group, int priority)
        {
            this.avatarUser = avatarUser;
            this.group = group;
            this.priority = priority;
        }
    }

    public class SetAvatarSilenceStatusCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public ItemSilenceStatusGroup group;

        public SetAvatarSilenceStatusCmd(AvatarUser avatarUser, ItemSilenceStatusGroup group)
        {
            this.avatarUser = avatarUser;
            this.group = group;
        }
    }

    public class SetAvatarActionStatusCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public ItemActionStatusGroup group;

        public SetAvatarActionStatusCmd(AvatarUser avatarUser, ItemActionStatusGroup group)
        {
            this.avatarUser = avatarUser;
            this.group = group;
        }
    }

    public class SetLookAtIKCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public GameObject lookAtGobj;
        public int priority;
        public float headWeight;
        public float bodyWeight;
        public VoiceActivityType voiceActivityType;
        public SetLookAtIKCmd(AvatarUser avatarUser, VoiceActivityType voiceActivityType, GameObject lookAtGobj, int priority, float headWeight, float bodyWeight)
        {
            this.avatarUser = avatarUser;
            this.lookAtGobj = lookAtGobj;
            this.priority = priority;
            this.headWeight = headWeight;
            this.bodyWeight = bodyWeight;
            this.voiceActivityType = voiceActivityType;
        }
    }

    public class SetAvatarPrefabCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public int index;

        public SetAvatarPrefabCmd(AvatarUser avatarUser, int index)
        {
            this.avatarUser = avatarUser;
            this.index = index;
        }
    }

    public class RegisterAvatarItemSlotCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public List<SlotName> slotNames;
        public Dictionary<IKEffectorName, IKTarget> ikTargets;

        public RegisterAvatarItemSlotCmd(AvatarUser avatarUser, List<SlotName> slotNames, Dictionary<IKEffectorName, IKTarget> ikTargets)
        {
            this.avatarUser = avatarUser;
            this.slotNames = slotNames;
            this.ikTargets = ikTargets;
        }
    }

    public class UpdateAvatarItemSlotCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public List<SlotName> slotNames;
        public Dictionary<IKEffectorName, IKTarget> ikTargets;

        public UpdateAvatarItemSlotCmd(AvatarUser avatarUser, List<SlotName> slotNames, Dictionary<IKEffectorName, IKTarget> ikTargets)
        {
            this.avatarUser = avatarUser;
            this.slotNames = slotNames;
            this.ikTargets = ikTargets;
        }
    }

    public class ClearAvatarItemSlotCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public List<SlotName> slotNames;

        public ClearAvatarItemSlotCmd(AvatarUser avatarUser, List<SlotName> slotNames)
        {
            this.avatarUser = avatarUser;
            this.slotNames = slotNames;
        }
    }

    public class SetAvatarPositionSlotCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public int slotIndex;
        public int userIndex;
        public int priority;

        public SetAvatarPositionSlotCmd(AvatarUser _avatarUser, int _priority, int _slotIndex, int _userIndex)
        {
            avatarUser = _avatarUser;
            slotIndex = _slotIndex;
            userIndex = _userIndex;
            priority = _priority;
        }
    }

    public class RemoveAvatarPositionSlotCmd : ActorsCmd
    {
        public AvatarUser avatarUser;
        public int slotIndex;

        public RemoveAvatarPositionSlotCmd(AvatarUser _avatarUser, int _slotIndex)
        {
            avatarUser = _avatarUser;
            slotIndex = _slotIndex;
        }
    }
}
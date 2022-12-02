using Playa.Avatars;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RootMotion.FinalIK;
using Playa.Common;
using Animancer;
using Playa.Item;

namespace Playa.App.Actors
{
    using ItemId = UInt64;
    using InteractionId = UInt64;
    using static Playa.Item.BaseItem;

    public class ActorsManager : ActorsApi
    {
        public class InteractionUserComp
        {
            public AvatarUser user;
            public FullBodyBipedEffector effectorType;

            public InteractionUserComp(AvatarUser _user, FullBodyBipedEffector _effectorType)
            {
                user = _user;
                effectorType = _effectorType;
            }
        }

        [SerializeField] private ItemManager _ItemManager;

        // private Dictionary<ItemId, List<AvatarUser>>
        private Dictionary<ItemId, List<AvatarUser>> _ItemAvatarUsers = new();
        private Dictionary<AvatarUser, ItemId> _AvatarUserStatusItem = new();

        override public Transform GetAvatarPosition(AvatarUser user)
        {
            return user.GetAvatarPosition();
        }

        override public void SetAvatarIdleStatus(ItemId id, AvatarUser user, ItemIdleStatusGroup group, ItemSecondIdleStatusGroup secondGroup)
        {
            var item = _ItemManager.FindItemById(id);
            if (item == null)
            {
                return;
            }

            user.GestureBehaviorPlanner.SetStatusGroup(id, AvatarStateType.BaseIdle, group, secondGroup);

            // comment item => avatar index effect hard coded
            if (_AvatarUserStatusItem.ContainsKey(user))
            {
                ItemId olditemid = _AvatarUserStatusItem[user];
                if (olditemid != 0)
                {
                    if (_ItemAvatarUsers.ContainsKey(olditemid))
                    {
                        _ItemAvatarUsers[olditemid].Remove(user);
                    }
                }
            }
            _AvatarUserStatusItem[user] = id;
            if (!_ItemAvatarUsers.ContainsKey(id))
            {
                _ItemAvatarUsers[id] = new List<AvatarUser>();
            }

            _ItemAvatarUsers[id].Add(user);
        }

        override public void SetAvatarIdleSubStatus(ItemId id, AvatarUser user, ItemIdleSubStatusGroup group, int priority)
        {
            var item = _ItemManager.FindItemById(id);
            if (item == null)
            {
                return;
            }

            user.GestureBehaviorPlanner.SetSubStatus(id, group, priority);

            if (!_ItemAvatarUsers.ContainsKey(id))
            {
                _ItemAvatarUsers[id] = new List<AvatarUser>();
            }
            _ItemAvatarUsers[id].Add(user);
        }

        override public void SetAvatarSilenceStatus(ItemId id, AvatarUser user, ItemSilenceStatusGroup group)
        {
            var item = _ItemManager.FindItemById(id);
            if (item == null)
            {
                return;
            }

            user.GestureBehaviorPlanner.SetStatusGroup(id, AvatarStateType.Silence, group);

            if (!_ItemAvatarUsers.ContainsKey(id))
            {
                _ItemAvatarUsers[id] = new List<AvatarUser>();
            }
            _ItemAvatarUsers[id].Add(user);
        }

        override public void SetAvatarActionStatus(ItemId id, AvatarUser user, ItemActionStatusGroup group)
        {
            var item = _ItemManager.FindItemById(id);
            if (item == null)
            {
                return;
            }

            user.GestureBehaviorPlanner.SetStatusGroup(id, AvatarStateType.ActionIdle, group);

            if (!_ItemAvatarUsers.ContainsKey(id))
            {
                _ItemAvatarUsers[id] = new List<AvatarUser>();
            }
            _ItemAvatarUsers[id].Add(user);
        }

        override public void SetIKLookAtObject(ItemId item_id, VoiceActivityType voiceActivityType, AvatarUser avatarUser, GameObject lookAtGameObject, int priority, float headWeight, float bodyWeight)
        {
            var item = _ItemManager.FindItemById(item_id);
            if (priority >= 0)
            {
                HeadIKOption headIKOption = new HeadIKOption(item_id, avatarUser, priority, lookAtGameObject.transform, headWeight, bodyWeight);
                avatarUser.HeadIKManager.AddHeadIK(voiceActivityType, headIKOption);
            }
            else
            {
                HeadIKOption headIKOption = new HeadIKOption(item_id, avatarUser, priority);
                avatarUser.HeadIKManager.RemoveHeadIK(voiceActivityType, headIKOption);
            }
            Debug.Log(string.Format("Item Events Set lookat ik {0} {1} {2} {3}", avatarUser.name, lookAtGameObject, priority, item.ItemProperties.Name));
        }

        override public void SetAvatarPrefab(AvatarUser user, int index)
        {
            user.SetAvatarPrefab(index);
        }

        override public void RegisterSlots(BaseItem item, AvatarUser user, List<SlotName> slotNames, Dictionary<IKEffectorName, IKTarget> targets)
        {
            user.SlotManager.RegisterSlots(slotNames, item, targets);
        }

        public override void UpdateSlots(BaseItem item, AvatarUser user, List<SlotName> slotNames, Dictionary<IKEffectorName, IKTarget> targets)
        {
            user.SlotManager.UpdateSlots(slotNames, targets);
        }

        override public void ClearSlots(AvatarUser user, List<SlotName> slotNames)
        {
            user.SlotManager.ClearSlots(slotNames);
        }

        public override void SetAvatarPositionSlot(BaseItem item, int priority, AvatarUser user, int slotIndex, int userIndex)
        {
            if (!item.ItemSlotTransformDictionary.ContainsKey(slotIndex))
            {
                Debug.LogWarning(string.Format("{0} Try slot avatar {1} fail slot {2} not exist", item.name, user.name, slotIndex));
                return;
            }

            if (item.ItemSlotUserDictionary.ContainsKey(slotIndex))
            {
                Debug.LogWarning(string.Format("{0} Try slot avatar {1} fail slot {2} occupy {3}", item.name, user.name, slotIndex, item.ItemSlotUserDictionary[slotIndex].AvatarUser.name));
                return;
            }

            Transform t = item.ItemSlotTransformDictionary[slotIndex];
            if (t != null)
            {

                user.SetPrefabPositionRotationToTarget(item.ItemId, priority, t.position, t.localRotation);
            }
            else
            {
                user.ResetPrefabPositionRotationToTarget();
            }
            
            item.ItemSlotUserDictionary[slotIndex] = new AvatarUserIndexBundle(user, userIndex);
        }

        public override void RemoveAvatarPositionSlot(BaseItem item, AvatarUser user, int slotIndex)
        {
            if (!item.ItemSlotTransformDictionary.ContainsKey(slotIndex))
            {
                Debug.LogWarning(string.Format("{0} Try unslot avatar {1} fail slot {2} not exist", item.name, user.name, slotIndex));
                return;
            }

            if (item.ItemSlotTransformDictionary[slotIndex]==null)
            {
                Debug.Log(string.Format("{0} Try unslot avatar {1} fail slot {2} null", item.name, user.name, slotIndex));
                return;
            }

            if (!item.ItemSlotUserDictionary.ContainsKey(slotIndex) || item.ItemSlotUserDictionary[slotIndex].AvatarUser != user)
            {
                Debug.LogWarning(string.Format("{0} Try unslot avatar {1} fail slot {2} occupy false {3}", item.name, user.name, slotIndex, item.ItemSlotUserDictionary[slotIndex]==null?"": item.ItemSlotUserDictionary[slotIndex].AvatarUser.name));
                return;
            }

            user.RemovePrefabPositionRotation(item.ItemId);
            item.ItemSlotUserDictionary[slotIndex] = null;
        }
    }

}
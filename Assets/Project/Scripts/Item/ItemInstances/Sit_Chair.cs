using Animancer;
using Cinemachine;
using Playa.App;
using Playa.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Playa.Avatars;
using Playa.App.Actors;
using Playa.Common.Utils;

namespace Playa.Item
{
    public class Sit_Chair : BaseItem
    {
        public UnityEvent condition;

        protected override void InitProperties()
        {
            _ItemProperties.Name = "Sit_Chair";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/SM_Chair.prefab";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_sitchair_baseIdle.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body };
        }

        protected override Quaternion ItemRotationFromUser(AvatarUser user)
        {
            return _ActorsUtils._ActorsManager.GetAvatarPosition(user).rotation;
        }

        protected override void RegisterChatEventCallbacks(int slotIndex)
        {
            Transform IKDollNodes = _Objects[_ItemProperties.Name].transform.Find("IKDollNodes"+slotIndex.ToString());

            // Unlock hand when speaking
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftHand, new IKTarget(null, 0, 0, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(null, 0, 0, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Chair SelfSpeaking triggered");
            });

            // Lock hand when not speaking
            ItemEventManager.AddItemEventSelfInactiveListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftHand, new IKTarget(IKDollNodes.Find("IKDollNodesLeftHand"), 1, 1, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(IKDollNodes.Find("IKDollNodesRightHand"), 1, 1, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Chair SelfInactive triggered");
            });

            // Lock hand when silence
            ItemEventManager.AddItemEventAllInactiveListener(this, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftHand, new IKTarget(IKDollNodes.Find("IKDollNodesLeftHand"), 1, 1, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(IKDollNodes.Find("IKDollNodesRightHand"), 1, 1, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Chair AllInactive triggered");
            });
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            // Lock feet and hip no matter what
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Root, new IKTarget(IKDollNodes.Find("IKDollNodesHip"), 1, 0, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Chest, new IKTarget(IKDollNodes.Find("IKDollNodesChest"), 1, 0, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftFoot, new IKTarget(IKDollNodes.Find("IKDollNodesLeftFoot"),1,1,1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightFoot, new IKTarget(IKDollNodes.Find("IKDollNodesRightFoot"),1,1,1));
        }
    }
}



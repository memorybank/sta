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
    public class Table_Chairs : BaseItem
    {
        protected override void InitProperties()
        {
            OnboardOtherAvatars();

            _ItemProperties.Name = "Table_Chairs";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Items/table/model/props/table.prefab";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/table/idle/TableWomanPrefab.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Models/Head_And_Arms.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/table/idle/TableManPrefab.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Models/Head_And_Arms.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body};
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body};
            _ItemProperties.EulerAngleDeformerOptions = EulerAngleDeformer.CreateDefaultOptions();
            _ItemProperties.EulerAngleDeformerOptions.LeftArmTranspose = new Vector3(-15f, 0f, 0f);
            _ItemProperties.EulerAngleDeformerOptions.RightArmTranspose = new Vector3(-15f, 0f, 0f);
            _ItemProperties.EulerAngleDeformerOptions.LeftElbowTranspose = new Vector3(0f, 0f, -30f);
            _ItemProperties.EulerAngleDeformerOptions.RightElbowTranspose = new Vector3(0f, 0f, 30f);
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Root, new IKTarget(IKDollNodes.Find("IKDollNodesHip"), 1, 0, 1));
            //_ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftFoot, new IKTarget(IKDollNodes.Find("IKDollNodesLeftFoot"), 1, 1, 1));
            //_ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightFoot, new IKTarget(IKDollNodes.Find("IKDollNodesRightFoot"), 1, 1, 1));
            //_ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftElbow, new IKTarget(IKDollNodes.Find("IKDollNodesLeftElbow"), 1, 0, 1));
            //_ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightElbow, new IKTarget(IKDollNodes.Find("IKDollNodesRightElbow"), 1, 0, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftHand, new IKTarget(IKDollNodes.Find("IKDollNodesLeftHand"), 1, 1, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightHand, new IKTarget(IKDollNodes.Find("IKDollNodesRightHand"), 1, 1, 1));            
        }

        protected override void RegisterChatEventCallbacks(int slotIndex)
        {
            Transform IKDollNodes = _Objects[_ItemProperties.Name].transform.Find("IKDollNodes" + slotIndex.ToString());

            // Unlock hand when speaking
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftHand, new IKTarget(null, 0, 0, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(null, 0, 0, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events table_chairs SelfSpeaking triggered");
            });

            // Lock hand when not speaking
            ItemEventManager.AddItemEventSelfInactiveListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftHand, new IKTarget(IKDollNodes.Find("IKDollNodesLeftHand"), 1, 1, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(IKDollNodes.Find("IKDollNodesRightHand"), 1, 1, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events table_chairs SelfInactive triggered");
            });

            // Lock hand when silence
            ItemEventManager.AddItemEventAllInactiveListener(this, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftHand, new IKTarget(IKDollNodes.Find("IKDollNodesLeftHand"), 1, 1, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(IKDollNodes.Find("IKDollNodesRightHand"), 1, 1, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events table_chairs AllInactive triggered");
            });
        }
    }
}

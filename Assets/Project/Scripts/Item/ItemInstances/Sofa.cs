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
    public class Sofa : BaseItem
    {
        protected override void InitProperties()
        {
            OnboardOtherAvatars();

            _ItemProperties.Name = "Sofa";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Items/sofa/modle/props/sofa.prefab";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/sofa/idle/SofaWomanPrefab.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Models/Head_And_Arms.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/sofa/idle/SofaManPrefab.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Models/Head_And_Arms.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body };
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Root, new IKTarget(IKDollNodes.Find("IKDollNodesHip"), 1, 0, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Chest, new IKTarget(IKDollNodes.Find("IKDollNodesChest"), 1, 0, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftFoot, new IKTarget(IKDollNodes.Find("IKDollNodesLeftFoot"), 1, 1, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightFoot, new IKTarget(IKDollNodes.Find("IKDollNodesRightFoot"), 1, 1, 1));
        }
    }
}



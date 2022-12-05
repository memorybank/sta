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
    public class Knee : BaseItem
    {
        protected override void InitProperties()
        {
            _ItemProperties.Name = "Knee";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/knee/idle/KneeIdle.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.SubStatusAnimPath.Add("Assets/Items/knee/idle/Pray.prefab");
            _ItemProperties.SubStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body };
            ItemSlotTransformDictionary[0] = null;
        }
    }
}

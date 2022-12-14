using Animancer;
using Cinemachine;
using Playa.Animations;
using Playa.App;
using Playa.App.Cinemachine;
using Playa.Avatars;
using Playa.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Playa.Item
{

    public class Seesaw : BaseItem
    {
        protected override void InitProperties()
        {
            OnboardOtherAvatars();

            _ItemProperties.Name = "Seesaw";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/M_SeeSaw.prefab";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_seesaw_baseIdle_0.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_Only.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_seesaw_baseIdle_1.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_Only.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand, SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Hand, SlotName.Body };
            _ItemProperties.IsMovable = true;
        }
    }
}
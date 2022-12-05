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
    public class DiscoBall : BaseItem
    {
        protected override void InitProperties()
        {
            OnboardOtherAvatars();

            _ItemProperties.Name = "DiscoBall";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Items/disco/model/props/disco ball.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Items/disco/sound/Quincas Moreira - Sabor Moreno (128 kbps).mp3";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/disco/idle/Hip Hop Dancing_amy.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/disco/idle/Hip Hop Dancing_qingwa.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body , SlotName.Hand };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body , SlotName.Hand };
            ItemSlotTransformDictionary[0] = null;
            ItemSlotTransformDictionary[1] = null;
        }

        protected override void ExecuteExtraCmds()
        {
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Active, ArmatureUtils.FindHead(OtherAvatarUsers[0].ActiveAvatarTransform).gameObject, 1, 1f, 0f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(OtherAvatarUsers[0], VoiceActivityType.Active, ArmatureUtils.FindHead(AffectAvatarUser.ActiveAvatarTransform).gameObject, 1, 1f, 0f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Inactive, _BaseApp._AppStartupConfig.LookAtTargets[0].gameObject, 1, 0f, 0f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(OtherAvatarUsers[0], VoiceActivityType.Inactive, _BaseApp._AppStartupConfig.LookAtTargets[1].gameObject, 1, 0f, 0f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Silence, _BaseApp._AppStartupConfig.LookAtTargets[0].gameObject, 1, 0f, 0f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(OtherAvatarUsers[0], VoiceActivityType.Silence, _BaseApp._AppStartupConfig.LookAtTargets[1].gameObject, 1, 0f, 0f));
            _Objects[_ItemProperties.Name].transform.position = _Objects[_ItemProperties.Name].transform.position + new Vector3(0, 1.9f, 0);
        }
    }
}

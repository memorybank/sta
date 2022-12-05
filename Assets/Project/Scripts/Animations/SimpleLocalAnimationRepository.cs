using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playa.Config;
using UnityEngine.AddressableAssets;

namespace Playa.Animations
{

    public class SimpleLocalAnimationRepository : AnimationRepository
    {

        [SerializeField] private List<AnimationClip> _OriginalAnimationClips;

        [SerializeField] private List<cfg.gesture.Phase> _Phases;

        [SerializeField] private ConfigsLoader _ConfigLoader;

        public override void Init()
        {
            _ConfigLoader = ConfigsLoader.Instance;
            
            foreach (var clip in _OriginalAnimationClips)
            {
                var clipTrans = new Animancer.ClipTransition();
                clipTrans.Clip = clip;
                _AnimationClips.Add(clipTrans);
            }

            AvatarMask fullBodyAvatarMask = Addressables.LoadAssetAsync<AvatarMask>
                        ("Assets/Project/Animations/Masks/Full_Body.mask").WaitForCompletion();
            AvatarMask headAndHandAvatarMask = Addressables.LoadAssetAsync<AvatarMask>
                        ("Assets/Project/Animations/Masks/Head_And_Arms.mask").WaitForCompletion();

            //clip infos
            for (int i = 0; i < _ConfigLoader.Tables.TbGestureMark.DataList.Count; i++)
            {
                if (_Phases.Contains(_ConfigLoader.Tables.TbGestureMark.DataList[i].Phase))
                {
                    var clipInfo = new GestureClipInfo();
                    clipInfo.GestureMark = _ConfigLoader.Tables.TbGestureMark.DataList[i];
                    clipInfo.Id = _ConfigLoader.Tables.TbGestureMark.DataList[i].Id;
                    clipInfo.ClipName = clipInfo.GestureMark.File.Split('.')[0];
                    clipInfo.AvatarMask = _ConfigLoader.Tables.TbGestureMark.DataList[i].IsFullBody? fullBodyAvatarMask:headAndHandAvatarMask;
                    AddAnimationClipInfo(clipInfo);
                }
            }
            GenerateIndices();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Playa.Avatars;
using UnityEngine;
using Animancer;

namespace Playa.Animations
{

    public class AvatarAnimationRepository : AnimationRepository
    {
        // Relaying using another repo
        public AnimationRepository Repo;
        public AvatarProfile Profile;

        private bool _IsInit = false;

        public override void Init()
        {
            if (_IsInit) 
            {
                return;
            }

            // Filter clips from the full-blown repo using profile
            Repo.Init();
            List<ClipTransition> newClip = new List<ClipTransition>();
            Dictionary<string, AnimationClipInfo> newClipInfo = new Dictionary<string, AnimationClipInfo>();

            for (int i = 0; i < Repo.AnimationClipInfos.Count; i++) 
            {
                newClip.Add(Repo.AnimationClips[i]);
            }

            ResetAnimationRepository(newClip, newClipInfo);
            _IsInit = true;
        }
    }

}
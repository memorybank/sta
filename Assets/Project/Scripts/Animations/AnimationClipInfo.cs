using UnityEngine;
using Playa.Common;
using System.Collections.Generic;

namespace Playa.Animations
{

    [System.Serializable]
    public class AnimationClipInfo
    {
        public string ClipName;
        public int Id;
    }

    public class GestureClipInfo : AnimationClipInfo
    {
        public cfg.gesture.GestureMark GestureMark;
        public AvatarMask AvatarMask;
    }
}
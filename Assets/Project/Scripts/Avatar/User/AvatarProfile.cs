using Animancer.Units;
using System;
using UnityEngine;
using Playa.Common;

namespace Playa.Avatars
{

    [Serializable]
    public sealed class AvatarProfile:MonoBehaviour
    {
        // 与AnimationClipInfo部分对应的一些标识
        //性别
        public Gender gender;
        //年龄
        public AgeSpan age;
        //人格
        public PersonalityMBTI personality;
    }

}
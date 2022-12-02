using Animancer.Units;
using System;
using UnityEngine;
using Playa.Common;

namespace Playa.Avatars
{

    [Serializable]
    public sealed class AvatarProfile:MonoBehaviour
    {
        // ��AnimationClipInfo���ֶ�Ӧ��һЩ��ʶ
        //�Ա�
        public Gender gender;
        //����
        public AgeSpan age;
        //�˸�
        public PersonalityMBTI personality;
    }

}
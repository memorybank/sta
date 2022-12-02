using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Common
{
    public enum Gender
    {
        Unknown = 0,
        Female = 1,
        Male = 2,
    }

    public enum AgeSpan
    {
        Invalid = 0,
        Kids = 1,
        YoungAdults = 2,
        MiddleAged = 3,
        Senious = 4,
        Elders = 5,
    }

    //MBTI 16
    //https://www.16personalities.com/ch/%E4%BA%BA%E6%A0%BC%E6%B5%8B%E8%AF%95
    public enum PersonalityMBTI
    {
        Invalid = 0,
        INTJ = 1,           //建筑师
        INTP = 2,           //逻辑学家
        ENTJ = 3,           //指挥官
        ENTP = 4,           //辩论家
        INFJ = 5,           //提倡者
        INFP = 6,           //调停者
        ENFJ = 7,           //主人公
        ENFP = 8,           //竞选者
        ISTJ = 9,           //物流师
        ISFJ = 10,          //守卫者
        ESTJ = 11,          //总经理
        ESFJ = 12,          //执政官
        ISTP = 13,          //鉴赏家
        ISFP = 14,          //探险家
        ESTP = 15,          //企业家
        ESFP = 16,          //表演家
    }
}
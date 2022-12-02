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
        INTJ = 1,           //����ʦ
        INTP = 2,           //�߼�ѧ��
        ENTJ = 3,           //ָ�ӹ�
        ENTP = 4,           //���ۼ�
        INFJ = 5,           //�ᳫ��
        INFP = 6,           //��ͣ��
        ENFJ = 7,           //���˹�
        ENFP = 8,           //��ѡ��
        ISTJ = 9,           //����ʦ
        ISFJ = 10,          //������
        ESTJ = 11,          //�ܾ���
        ESFJ = 12,          //ִ����
        ISTP = 13,          //���ͼ�
        ISFP = 14,          //̽�ռ�
        ESTP = 15,          //��ҵ��
        ESFP = 16,          //���ݼ�
    }
}
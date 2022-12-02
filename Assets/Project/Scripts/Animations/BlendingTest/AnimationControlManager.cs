using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class AnimationControlManager : MonoBehaviour
{
    public float FadeDuration;

    public bool IsRinComUsingFromNormalizedStart;

    private void Awake()
    {
        FadeDuration = 0;
        IsRinComUsingFromNormalizedStart = false;
    }
}

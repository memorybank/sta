using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class AnimationDataManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimationLayer
    {
        public AnimationClip[] Clips;
    }


    [SerializeField] public AnimationLayer[] AnimationLayers;

    [SerializeField] public AvatarMask[] AvatarMasks;

    [SerializeField] public AnimationClip Idle;
}

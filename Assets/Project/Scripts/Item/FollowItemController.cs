using Animancer;
using Animancer.FSM;
using Animancer.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using Playa.Item;
using Playa.Avatars;

public class FollowItemController : MonoBehaviour
{
    [SerializeField] AnimationClip currentClip;
    private Dictionary<float, AnimationClip> _Clips;
    private AnimancerComponent _AnimancerComponent;

    public void Init()
    {
        _AnimancerComponent = gameObject.AddComponent<AnimancerComponent>();
        _AnimancerComponent.Animator = gameObject.GetComponent<Animator>();
        _Clips = new Dictionary<float, AnimationClip>();
    }

    public void AddClip(AnimationClip clip, float minSpeed)
    {
        _Clips.Add(minSpeed, clip);
    }

    public void PlayClip(float speed)
    {
        AnimationClip toPlayClip = null;
        foreach (var kvp in _Clips)
        {
            if (speed >= kvp.Key)
            {
                toPlayClip = kvp.Value;
            }
        }
        if (toPlayClip != null)
        {
            _AnimancerComponent.Play(toPlayClip);
        }
    }
}

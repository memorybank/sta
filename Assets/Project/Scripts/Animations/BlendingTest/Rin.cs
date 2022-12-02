using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

public class Rin : MonoBehaviour
{
    [SerializeField] private AnimancerComponent _Animancer;

    public AnimationDataManager DataManager;
    public AnimationControlManager ControlManager;

    public Queue<int> Sequence;

    private void Awake()
    {
        Sequence = new Queue<int>();
        Sequence.Clear();
    }

    private void Start()
    {
        _Animancer.Play(DataManager.Idle);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && Sequence.Count > 0)
        {
            int currentIndex = Sequence.Dequeue();
            if (ControlManager.IsRinComUsingFromNormalizedStart)
            {
                Debug.Log("_Sequence Has Pop " + currentIndex.ToString());

                Animancer.AnimancerState state = _Animancer.Play(
                    DataManager.AnimationLayers[0].Clips[currentIndex], ControlManager.FadeDuration, FadeMode.NormalizedFromStart);
                state.Events.OnEnd = OnActionEnd;
            }
            else
            {
                Debug.Log("_Sequence Has Pop " + currentIndex.ToString());

                Animancer.AnimancerState state = _Animancer.Play(
                    DataManager.AnimationLayers[0].Clips[currentIndex], ControlManager.FadeDuration, FadeMode.FromStart);
                state.Events.OnEnd = OnActionEnd;
            }
        }

    }
    private void OnActionEnd()
    {
        _Animancer.Play(DataManager.Idle, 1.0f);
    }

    public void Clear(int layer)
    {
        _Animancer.Layers[layer].Stop();
    }

    public void Play(int layer, string name)
    {
        _Animancer.Layers[layer].SetMask(DataManager.AvatarMasks[layer]);
        foreach (var clip in DataManager.AnimationLayers[layer].Clips)
        {
            if (clip.name == name)
            {
                _Animancer.Layers[layer].Play(clip, 0.2f, FadeMode.FromStart);
                return;
            }
        }
    }
}

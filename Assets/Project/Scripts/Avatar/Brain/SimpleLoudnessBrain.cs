using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer.FSM;

using Playa.Common;
using Playa.Audio.VAD;

namespace Playa.Avatars
{

    public sealed class SimpleLoudnessBrain : AvatarBrain
    {
        [SerializeField] private MicrophoneLoudnessDetector _MicrophoneLoudinessDetector;

        private void Start()
        {
            _MicrophoneLoudinessDetector.VoiceLoudnessEvent.AddListener(OnVoiceActivityReady);
        }

        private void OnVoiceActivityReady(VoiceLoudnessUnit voiceLoudnessUnit)
        {
            if (voiceLoudnessUnit.Loudness > 100.0f)
            {
                ((AvatarActionState)GestureBehaviorPlanner.AvatarUser.GetAvatarState(AvatarStateType.StartTalking)).TryReEnterState();
            }
            else if (voiceLoudnessUnit.Loudness < 10.0f)
            {
                ((AvatarActionState)GestureBehaviorPlanner.AvatarUser.GetAvatarState(AvatarStateType.FinishTalking)).TryReEnterState();
            }

        }
    }
}

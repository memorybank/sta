using UnityEngine;

using Playa.Event;
using Playa.Audio.MicrophoneEngine;
using Playa.Avatars;

namespace Playa.Audio
{

    public abstract class AudioDetector : MonoBehaviour
    {
        // Source
        public AvatarAudioSource _SpeechSource;

        // Destination
        public AvatarBrain _AvatarBrain;

        public EventSequencerManager _EventSequencerManager;

        public abstract string GetDetectedResult();
    }
}
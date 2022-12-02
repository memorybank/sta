using UnityEngine;

namespace Playa.Audio.Agora
{
    public class AudioSourceManager : Singleton<AudioSourceManager>
    {
        public AudioClip GetPlayBackClip()
        {
            AudioClip clip = null;

            return clip;
        }
    }

    public enum AudioSourceType
    {
        None,
        Local,
        Remote,
    }
}


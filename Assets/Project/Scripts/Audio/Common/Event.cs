using Playa.Common;
using System;
using UnityEngine.Events;

namespace Playa.Audio.Event
{
    [Serializable]
    public class VoiceActivityEvent : UnityEvent<VoiceActivityUnit, int>
    {
    }

    [Serializable]
    public class VoiceLoudnessEvent : UnityEvent<VoiceLoudnessUnit>
    {
    }

    [Serializable]
    public class IdeationalUnitEvent : UnityEvent<IdeationalUnit>
    {
    }
}
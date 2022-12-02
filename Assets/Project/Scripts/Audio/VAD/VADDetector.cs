using Playa.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Audio.VAD
{
    public class VADDetector : AudioDetector
    {
        protected VoiceActivityType _VoiceActivity;
        public VoiceActivityType voiceActivityType { get { return _VoiceActivity; } }

        override public string GetDetectedResult()
        {
            switch (_VoiceActivity)
            {
                case VoiceActivityType.Inactive: return "Not speaking";
                case VoiceActivityType.Active: return "Speaking";
                case VoiceActivityType.Punctuated: return "Punctuated";
                default: return "Not ready";
            }
        }
    }
}
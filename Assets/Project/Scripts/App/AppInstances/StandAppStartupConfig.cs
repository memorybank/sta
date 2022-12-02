using Playa.Audio.VAD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Playa.App
{
    public class StandAppStartupConfig : BaseAppStartupConfig
    {
        public WebRTCVADDetector VADDetector;
        public Dropdown MethodDropdown;
    }
}
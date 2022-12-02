using Playa.App;
using Playa.Avatars;
using UnityEngine;
using Playa.Common;
using Playa.Audio.VAD;
using UnityEngine.UI;
using Playa.Audio;
using Playa.Audio.ASR;

namespace Playa.UI
{
    public class ItemSceneUI : MonoBehaviour
    {
        public TMPro.TMP_InputField AvatarIndexText;

        [SerializeField] private Dropdown _AppDropDown;
        void Start()
        {
            AvatarIndexText.text = "0";
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                switch (AvatarIndexText.text)
                {
                    case "0":
                        AvatarIndexText.text = "1";
                        SwitchAudioDetectorBrain("1");
                        break;
                    case "1":
                        AvatarIndexText.text = "0";
                        SwitchAudioDetectorBrain("0");
                        break;
                    default:
                        AvatarIndexText.text = "0";
                        SwitchAudioDetectorBrain("0");
                        break;
                }
            }
        }

        public void SwitchAudioDetectorBrain(string s)
        {
            var currentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text).GetComponent<BaseApp>();

            AvatarUser Avatar0 = currentApp._AppStartupConfig.AvatarUsers[0];
            AvatarUser Avatar1 = currentApp._AppStartupConfig.AvatarUsers[1];
            Transform t0 = GameUtils.FindDeepChild(Avatar0.transform, "VADDetector");
            Transform t1 = GameUtils.FindDeepChild(Avatar1.transform, "VADDetector");
            Transform a0 = GameUtils.FindDeepChild(Avatar0.transform, "AudioSource");
            Transform a1 = GameUtils.FindDeepChild(Avatar1.transform, "AudioSource");

            Debug.Assert(!a0.gameObject.GetComponent<AvatarAudioSource>().IsPlaying && !a1.gameObject.GetComponent<AvatarAudioSource>().IsPlaying, "should switch before audio start");

            GCSRDetector g0 = GameUtils.FindDeepChild(Avatar0.transform, "GCSRDetector").gameObject.GetComponent<GCSRDetector>();
            GCSRDetector g1 = GameUtils.FindDeepChild(Avatar1.transform, "GCSRDetector").gameObject.GetComponent<GCSRDetector>();
            switch (s)
            {
                case "0":
                    t0.gameObject.GetComponent<WebRTCVADDetector>()._SpeechSource = a0.gameObject.GetComponent<AvatarAudioSource>();
                    t1.gameObject.GetComponent<WebRTCVADDetector>()._SpeechSource = a1.gameObject.GetComponent<AvatarAudioSource>();
                    g0._SpeechSource = a0.gameObject.GetComponent<AvatarAudioSource>();
                    g1._SpeechSource = a1.gameObject.GetComponent<AvatarAudioSource>();
                    break;
                case "1":
                    t0.gameObject.GetComponent<WebRTCVADDetector>()._SpeechSource = a1.gameObject.GetComponent<AvatarAudioSource>();
                    t1.gameObject.GetComponent<WebRTCVADDetector>()._SpeechSource = a0.gameObject.GetComponent<AvatarAudioSource>();
                    g0._SpeechSource = a1.gameObject.GetComponent<AvatarAudioSource>();
                    g1._SpeechSource = a0.gameObject.GetComponent<AvatarAudioSource>();
                    break;
                default:
                    break;
            }
        }
    }
}
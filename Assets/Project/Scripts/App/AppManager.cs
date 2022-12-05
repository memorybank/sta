using Playa.Audio.VAD;
using Playa.Avatars;
using Playa.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Playa.App
{
    public class AppManager : MonoBehaviour
    {
        public Dropdown _AppDropdown;

        public BaseAppStartupConfig _AppStartupConfig;

        public string _AppName;

        void Awake()
        {
            _AppDropdown.options.Add(new Dropdown.OptionData("BaseApp"));
            _AppDropdown.options.Add(new Dropdown.OptionData("StandApp"));

            _AppDropdown.onValueChanged.AddListener(index =>
            {
                SceneManager.UnloadSceneAsync(_AppName);
                _AppName = _AppDropdown.options[index].text;

                // TODO: Decouple app param init from UI
                switch (index)
                {
                    // Null app
                    case 0:
                        break;
                    // Stand app
                    case 1:
                        {
                            _AppStartupConfig = new StandAppStartupConfig();
                            (_AppStartupConfig as StandAppStartupConfig).MethodDropdown = GameObject.Find("MethodDropdown").GetComponent<Dropdown>();
                            (_AppStartupConfig as StandAppStartupConfig).VADDetector = GameObject.Find("VADDetector").GetComponent<WebRTCVADDetector>();
                            break;
                        }
                    default:
                        // Do nothing
                        break;
                }

                // Base configs
                _AppStartupConfig.AvatarUsers = new Avatars.AvatarUser[2];
                _AppStartupConfig.AvatarUsers[0] = GameObject.Find("AvatarGen").GetComponent<Playa.Avatars.AvatarAnimator>().AvatarUser;
                _AppStartupConfig.AvatarUsers[1] = GameObject.Find("AvatarGenSecond").GetComponent<Playa.Avatars.AvatarAnimator>().AvatarUser;
                _AppStartupConfig.LookAtTargets = new Transform[2];
                _AppStartupConfig.LookAtTargets[0] = GameObject.Find("LookAtTarget1").transform;
                _AppStartupConfig.LookAtTargets[1] = GameObject.Find("LookAtTarget2").transform;

                SceneManager.LoadScene(_AppDropdown.options[index].text, LoadSceneMode.Additive);
            });

            _AppName = "BaseApp";
            SceneManager.LoadScene(_AppName, LoadSceneMode.Additive);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
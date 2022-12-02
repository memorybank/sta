using Animancer.FSM;
using Playa.Avatars;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Playa.UI
{
    public class AudioSceneUI : MonoBehaviour
    {
        //[SerializeField] private Avatars.AvatarAnimator _AvatarAnimator;
        [SerializeField] private Avatars.AvatarUser _AvatarUser;
        private StateMachine<AvatarActionState> _StateMachine;

        public TextMeshProUGUI VADText;
        public TextMeshProUGUI LoudnessText;
        public TextMeshProUGUI AvatarState;
        public TextMeshProUGUI LipText;

        void Start()
        {
            //Get them_Animator, which you attach to the GameObject you intend to animate.
            //_AvatarAnimator = GameObject.Find("Rin New Prefab").GetComponent<Avatars.AvatarAnimator>();
            _StateMachine = _AvatarUser.AvatarActionStateMachine;
        }

        // Update is called once per frame
        void Update()
        {
            VADText.text = (_AvatarUser.AvatarBrain as AudioBrain).VADDetector.GetDetectedResult();
            //LoudnessText.text = (_AvatarUser.AvatarBrain as AudioBrain).MicrophoneLoudnessDetector.GetDetectedResult();
            AvatarState.text = _StateMachine.CurrentState.ToString();
            LipText.text = _AvatarUser.FacialBehaviorPlanner.GetVisemeMultiplier().ToString();
        }
        
        // Todo multi user or multi avatar change manage
        public void OnAvatarChange(Dropdown change)
        {
            var camera = GameObject.Find("Main Camera");
            switch (change.value)
            {
                case 0:
                    camera.transform.position = new Vector3(-0.1f, 1.35f, -9.0f);
                    camera.transform.rotation = Quaternion.Euler(new Vector3(3.0f, 0, 0));
                    break;
                //case 1:
                //    camera.transform.position = new Vector3(-1.25f, 0.9f, -9.0f);
                //    break;
                //case 2:
                //    camera.transform.position = new Vector3(0.75f, 1.5f, -9.0f);
                //    break;
                default:
                    break;
            }
        }
    }
}
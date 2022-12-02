using Animancer.FSM;
using Playa.Avatars;
using UnityEngine;

namespace Playa.UI
{
    public class GeneratedSceneUI : MonoBehaviour
    {
        public UnityEngine.UI.Text TextOutput;
        [SerializeField] private Avatars.AvatarAnimator _Avatar;
        private StateMachine<AvatarActionState> _StateMachine;

        void Start()
        {
            //Get them_Animator, which you attach to the GameObject you intend to animate.
            //_AvatarAnimator = GameObject.Find("Rin New Prefab").GetComponent<Avatars.AvatarAnimator>();
            _StateMachine = _Avatar.ActionStateMachine;
        }

        // Update is called once per frame
        void Update()
        {
            if (_StateMachine.CurrentState.GetType() == typeof(ActionIdleState))
            {
                TextOutput.text = "Animation Name: Idle";
            }
            if (_StateMachine.CurrentState.GetType() == typeof(IDUMetronomicState))
            {
                var state = (IDUMetronomicState)_StateMachine.CurrentState;
                TextOutput.text = "\nAnimation Name: " + state.Repo.AnimationClips[state.CurrentIndex].Clip.name;
            }
        }
    }
}
using Playa.Avatars;
using Playa.Item;
using UnityEngine;

namespace Playa.App
{
    public class StandApp : BaseApp
    {
        [SerializeField] private BaseItem _DefaultItem;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
        }

        protected void Start()
        {
            Init();
            _AgoraManager.JoinChannel();
            // TODO: Init Head IK
            Debug.Log("StandApp load finished");
        }


        private void OnDestroy()
        {
            _AgoraManager.LeaveChannel();
            Debug.Log("StandApp destroy finished");    
        }

        public void Init()
        {
            Debug.Log("Stand app init");
            var config = _AppStartupConfig as StandAppStartupConfig;

            config.AvatarUsers[0].AvatarAnimator.IdleStatusIndex = 0;
            config.AvatarUsers[1].AvatarAnimator.IdleStatusIndex = 0;
            config.AvatarUsers[1].AvatarAnimator.SilenceStatusIndex = 0;
            config.AvatarUsers[1].AvatarAnimator.SilenceStatusIndex = 0;

            config.AvatarUsers[0].AvatarBrain.Behavior.GestureBehavior = new IdleGestureBehavior();
            config.AvatarUsers[0].GetStateFunction(StateActionType.BackToIdle)?.Invoke();
            config.AvatarUsers[1].AvatarBrain.Behavior.GestureBehavior = new IdleGestureBehavior();
            config.AvatarUsers[1].GetStateFunction(StateActionType.BackToIdle)?.Invoke();

            config.AvatarUsers[0].ChangeIKManager();
            config.AvatarUsers[1].ChangeIKManager();

            config.AvatarUsers[0].AvatarBrain.EventSequencer.StartSequence();
            config.AvatarUsers[1].AvatarBrain.EventSequencer.StartSequence();

            _DefaultItem = this.gameObject.AddComponent<House>();
            _DefaultItem._BaseApp = this;            
            _DefaultItem.ActivateByUser("1001");
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
using Playa.App.Actors;
using Playa.App.Cinemachine;
using Playa.App.Stage;
using Playa.Audio.Agora;
using Playa.Common;
using Playa.Item;
using UnityEngine;

namespace Playa.App
{
    public class BaseApp : MonoBehaviour
    {
        public BaseAppStartupConfig _AppStartupConfig;

        public ServiceLocator _ServiceLocator;

        public StageApi _StageManager;

        public CinemachineApi _CinemachineManager;

        public CameraSwitchManager _CameraSwitchManager;

        public ActorsApi _ActorsManager;

        public ItemManager _ItemManager;

        public AgoraManager _AgoraManager;

        public ItemEventManager _ItemEventManager;

        protected virtual void Awake()
        {
            _AppStartupConfig = GameObject.Find("AppManager").GetComponent<AppManager>()._AppStartupConfig;
            _ItemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
            Debug.Assert(_ItemManager != null, "Item manager cant be null");
            _StageManager = GameObject.Find("StageManager").GetComponent<StageApi>();
            _CinemachineManager = GameObject.Find("CinemachineManager").GetComponent<CinemachineApi>();
            _ActorsManager = GameObject.Find("ActorsManager").GetComponent<ActorsApi>();
            _ServiceLocator = GameObject.Find("ServiceLocator").GetComponent<ServiceLocator>();
            _AgoraManager = GameObject.Find("AgoraManager").GetComponent<AgoraManager>();
            _ItemEventManager = GameObject.Find("ItemEventManager").GetComponent<ItemEventManager>();
            _ItemEventManager.Init();
            _CameraSwitchManager = GameObject.Find("CinemachineManager").GetComponent<CameraSwitchManager>();
            _CameraSwitchManager._CinemachineManager = _CinemachineManager;
            _CameraSwitchManager._ItemEventManager = _ItemEventManager;

            Debug.Log("Base app load finished");
        }
    }
}
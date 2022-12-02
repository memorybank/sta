using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using agora_gaming_rtc;

namespace Playa.Audio.Agora
{
    public class AgoraManager : MonoBehaviour
    {
        //[SerializeField] private AudioComponent _AudioComponent;
        [SerializeField] private ChatRoomManager _ChatRoomManager;

        public ChatRoomManager ChatRoomManager => _ChatRoomManager;

        public TextMeshProUGUI channelText;
        public TextMeshProUGUI userJoinText;

        private void Awake()
        {
            // instantiate it
            _ChatRoomManager.eventLocalUserJoinedChannel += OnEventLocalChannelReady;
            _ChatRoomManager.eventRemoteUserJoined += OnEventRemoteUserJoined;
        }

        private void OnEventLocalChannelReady(string channelName, uint uid, IRtcEngine channelData)
        {
            channelText.text = string.Format("joinChannel callback uid: {0}, channel: {1}, version: {2}", uid, channelName, IRtcEngine.GetSdkVersion());
        }

        private void OnEventRemoteUserJoined(uint uid)
        {
            userJoinText.text = string.Format("onUserJoined callback uid {0}", uid);
        }

        public void JoinChannel()
        {
            _ChatRoomManager.JoinChannel();
        }

        public void LeaveChannel()
        {
            _ChatRoomManager.LeaveChannel();
        }

        private void OnDestroy()
        {
            _ChatRoomManager.OnAppQuit();            
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


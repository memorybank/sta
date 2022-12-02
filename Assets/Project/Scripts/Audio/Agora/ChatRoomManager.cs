using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;

namespace Playa.Audio.Agora
{
    public class ChatRoomManager : MonoBehaviour
    {
        //todo deprecate this
        public Action<int, bool> eventGoChatRoom;
        public Action<string, uint, IRtcEngine> eventLocalUserJoinedChannel;
        public Action<uint> eventRemoteUserJoined;

        public string AppID = "1947c970395a4f3c810c64f8ebd12e44";
        public string TokenKey = "007eJxTYNgRlqQ+98EiIYtvK+dMvJCWwT87r2/mVP3o6py85UYPT0UoMCSZGCWlGlpYGlkmGZoYJVomplqkJZkkJ5ukGpsYmqekvAhTSM5coZj8ktGWkZEBAkF8VoaCnMTKRAYGAKmkIYw=";
        public string LobbyChannelName = "playa";
        public IRtcEngine mRtcEngine = null;

        void Awake()
        {
            mRtcEngine = IRtcEngine.GetEngine(AppID);
            mRtcEngine.SetDefaultAudioRouteToSpeakerphone(true);
            // Set VAD settings        
            mRtcEngine.EnableAudioVolumeIndication(200, 3, true);
            InitRtcEngine();
        }

        private void InitRtcEngine()
        {
            mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) =>
            {
                string joinSuccessMessage = string.Format("joinChannel callback uid: {0}, channel: {1}, version: {2}", uid, channelName, getSdkVersion());
                Debug.Log(joinSuccessMessage);
                //mShownMessage.GetComponent<Text>().text = (joinSuccessMessage);
                //muteButton.enabled = true;

                eventLocalUserJoinedChannel?.Invoke(channelName, uid, mRtcEngine);
            };

            mRtcEngine.OnLeaveChannel += (RtcStats stats) =>
            {
                string leaveChannelMessage = string.Format("onLeaveChannel callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}",
                    stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate);
                Debug.Log(leaveChannelMessage);
                //mShownMessage.GetComponent<Text>().text = (leaveChannelMessage);
                //muteButton.enabled = false;
                // reset the mute button state
                if (isMuted)
                {
                    MuteButtonTapped();
                }

                LeaveChannel();
                OnAppQuit();
            };

            mRtcEngine.OnUserJoined += (uint uid, int elapsed) =>
            {
                string userJoinedMessage = string.Format("onUserJoined callback uid {0} {1}", uid, elapsed);
                Debug.Log(userJoinedMessage);
                //txtRemoteUserInfo.text = userJoinedMessage;
                eventRemoteUserJoined?.Invoke(uid);
            };

            mRtcEngine.OnUserOffline += (uint uid, USER_OFFLINE_REASON reason) =>
            {
                string userOfflineMessage = string.Format("onUserOffline callback uid {0} {1}", uid, reason);
                //txtRemoteUserInfo.text = userOfflineMessage;
                Debug.Log(userOfflineMessage);
            };

            mRtcEngine.OnUserMutedAudio += (uint uid, bool muted) =>
            {
                string userMutedMessage = string.Format("onUserMuted callback uid {0} {1}", uid, muted);
                Debug.Log(userMutedMessage);
            };

            mRtcEngine.OnWarning += (int warn, string msg) =>
            {
                string description = IRtcEngine.GetErrorDescription(warn);
                string warningMessage = string.Format("onWarning callback {0} {1} {2}", warn, msg, description);
                Debug.Log(warningMessage);
            };

            mRtcEngine.OnError += (int error, string msg) =>
            {
                string description = IRtcEngine.GetErrorDescription(error);
                string errorMessage = string.Format("onError callback {0} {1} {2}", error, msg, description);
                Debug.Log(errorMessage);
            };

            mRtcEngine.OnRtcStats += (RtcStats stats) =>
            {
                string rtcStatsMessage = string.Format("onRtcStats callback duration {0}, tx: {1}, rx: {2}, " +
                    "tx kbps: {3}, rx kbps: {4}, tx(a) kbps: {5}, rx(a) kbps: {6} users {7}",
                    stats.duration, stats.txBytes, stats.rxBytes,
                    stats.txKBitRate, stats.rxKBitRate, stats.txAudioKBitRate, stats.rxAudioKBitRate, stats.userCount);
                Debug.Log(rtcStatsMessage);

                int lengthOfMixingFile = mRtcEngine.GetAudioMixingDuration();
                int currentTs = mRtcEngine.GetAudioMixingCurrentPosition();

                string mixingMessage = string.Format("Mixing File Meta {0}, {1}", lengthOfMixingFile, currentTs);
                Debug.Log(mixingMessage);
            };

            mRtcEngine.OnAudioRouteChanged += (AUDIO_ROUTE route) =>
            {
                string routeMessage = string.Format("onAudioRouteChanged {0}", route);
                Debug.Log(routeMessage);
            };

            mRtcEngine.OnRequestToken += () =>
            {
                string requestKeyMessage = string.Format("OnRequestToken");
                Debug.Log(requestKeyMessage);
            };

            mRtcEngine.OnConnectionInterrupted += () =>
            {
                string interruptedMessage = string.Format("OnConnectionInterrupted");
                Debug.Log(interruptedMessage);
            };

            mRtcEngine.OnConnectionLost += () =>
            {
                string lostMessage = string.Format("OnConnectionLost");
                Debug.Log(lostMessage);
            };

            mRtcEngine.SetLogFilter(LOG_FILTER.INFO);

            mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);

            // mRtcEngine.SetChannelProfile (CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);
            // mRtcEngine.SetClientRole (CLIENT_ROLE.BROADCASTER);
        }

        public void JoinChannel()
        {
            var lobbyChannelName = LobbyChannelName;
            var channelName = lobbyChannelName;
            Debug.Log(string.Format("tap joinChannel with channel name {0}", channelName));
            if (string.IsNullOrEmpty(channelName))
            {
                return;
            }

            var tokenKey = TokenKey;
            mRtcEngine.JoinChannelByKey(tokenKey, channelName, "extra", 0);
        }

        // todo invoke this outside stand app
        public void LeaveChannel()
        {
            mRtcEngine.LeaveChannel();
            var lobbyChannelName = LobbyChannelName;
            var channelName = lobbyChannelName;
            Debug.Log(string.Format("left channel name {0}", channelName));
        }

        public string getSdkVersion()
        {
            string ver = IRtcEngine.GetSdkVersion();
            return ver;
        }


        bool isMuted = false;
        void MuteButtonTapped()
        {
            string labeltext = isMuted ? "Mute" : "Unmute";
            //Text label = muteButton.GetComponentInChildren<Text>();
            // if (label != null)
            // {
            //     label.text = labeltext;
            // }
            isMuted = !isMuted;
            mRtcEngine.EnableLocalAudio(!isMuted);
        }


        //comment: check this
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
    private bool IsAndroid12AndUp()
    {
        // android12VersionCode is hardcoded because it might not be available in all versions of Android SDK
        const int android12VersionCode = 31;
        AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION");
        int buildSdkVersion = buildVersionClass.GetStatic<int>("SDK_INT");

        return buildSdkVersion >= android12VersionCode;
    }

    private string GetBluetoothConnectPermissionCode()
    {
        if (IsAndroid12AndUp())
        {
            // UnityEngine.Android.Permission does not contain the BLUETOOTH_CONNECT permission, fetch it from Android
            AndroidJavaClass manifestPermissionClass = new AndroidJavaClass("android.Manifest$permission");
            string permissionCode = manifestPermissionClass.GetStatic<string>("BLUETOOTH_CONNECT");

            return permissionCode;
        }

        return "";
    }
#endif

        //退出应用或者释放 IRtcEngine
        void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            if (mRtcEngine != null)
            {
                IRtcEngine.Destroy();
            }
        }

        //todo should destroy at some pls
        public void OnAppQuit()
        {
            if (mRtcEngine != null)
            {
                // 销毁 IRtcEngine。
                IRtcEngine.Destroy();
                mRtcEngine = null;
            }
        }
    }
}


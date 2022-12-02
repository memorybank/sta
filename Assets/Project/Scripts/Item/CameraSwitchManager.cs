using System.Collections;
using System.Collections.Generic;
using Timer = Playa.Common.Utils.Timer;
using Cinemachine;
using Playa.App;
using Playa.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using Playa.Avatars;
using System;
using Playa.App.Cinemachine;


namespace Playa.Item
{
    using ItemId = UInt64;

    public class CameraSwitchManager : MonoBehaviour
    {
        public CinemachineApi _CinemachineManager;
        public ItemEventManager _ItemEventManager;

        UnityAction _OnSpeakerChange;

        //comment: occupy moving and replace previous moving
        private GameObject currentTurnaroundsCam;
        private Timer currentTurnaroundsCamTimer;

        private Dictionary<int, BaseItem.AvatarUserIndexBundle> CurrentItemSlotUserDictionary;

        public void OnInitItem(BaseItem baseItem)
        {
            _OnSpeakerChange = OnSpeakerChange;
            baseItem.ItemEventManager.AddItemEventSpeakerChangeListener(baseItem, _OnSpeakerChange);
            CurrentItemSlotUserDictionary = new Dictionary<int, BaseItem.AvatarUserIndexBundle>();
            CurrentItemSlotUserDictionary.Add(0, new BaseItem.AvatarUserIndexBundle(baseItem._BaseApp._AppStartupConfig.AvatarUsers[0], 0));
            CurrentItemSlotUserDictionary.Add(1, new BaseItem.AvatarUserIndexBundle(baseItem._BaseApp._AppStartupConfig.AvatarUsers[1], 1));
        }

        public void SetCameraSwitchManagerSlotUser(BaseItem baseItem)
        {
            //todo: add slot replace strategy
        }

        public void RemoveMovingCam()
        {
            currentTurnaroundsCam = null;
            currentTurnaroundsCamTimer = null;
            OnSpeakerChange();
        }

        private void Update()
        {
            if (currentTurnaroundsCam != null)
            {
                float keepTime = 2.0f;
                Animator camAnimator = currentTurnaroundsCam.GetComponent<Animator>();
                if (camAnimator != null)
                {
                    keepTime = camAnimator.runtimeAnimatorController.animationClips[0].length;                    
                }

                if (currentTurnaroundsCamTimer.ElapsedTime() > keepTime)
                {
                    currentTurnaroundsCamTimer.StopTimer();
                    currentTurnaroundsCam = null;
                    OnSpeakerChange();
                }

                return;
            }

            var option = _CinemachineManager.CameraLists[CameraTiming.Turnaround].GetOption() as CameraOption;
            if (option != null)
            {
                _CinemachineManager.CameraLists[CameraTiming.Turnaround].RemoveOption(option);
                _CinemachineManager.ActivateCamera(option.item_ID, option.camera.name);
                currentTurnaroundsCam = option.camera;
                if (currentTurnaroundsCamTimer == null)
                {
                    currentTurnaroundsCamTimer = new Timer();
                }
                currentTurnaroundsCamTimer.StartTimer();
            }
        }

        public void OnSpeakerChange()
        {
            if (currentTurnaroundsCam != null)
            {
                return;
            }

            if (CurrentItemSlotUserDictionary == null)
            {
                return;
            }

            CameraOption option = null;
            int speakerUUID = _ItemEventManager.eventSequencerManager.GetCurrentSpeaker();
            Debug.Log("Camera switch get uuid " + speakerUUID.ToString());
            if (speakerUUID == -1)
            {
                option = _CinemachineManager.CameraLists[CameraTiming.AllInactive].GetOption() as CameraOption;
                Debug.Log("Camera switch manager speaker -1");
            }
            else
            {
                for (int i = 0; i < CurrentItemSlotUserDictionary.Count; i++)
                {
                    if (speakerUUID == CurrentItemSlotUserDictionary[i].AvatarUser.AvatarUUID)
                    {
                        option = _CinemachineManager.CameraLists[CameraTiming.Speaker0+i].GetOption() as CameraOption;
                        Debug.Log("Camera switch manager speaker " + i.ToString());
                        break;
                    }
                }
            }

            if (option != null)
            {
                _CinemachineManager.ActivateCamera(option.item_ID, option.camera.name);
            }
            Debug.Log("Item Events Camera SpeakerChange triggered");
        }
    }
}
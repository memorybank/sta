using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Cinemachine;
using Playa.Item;
using Playa.Avatars;
using UnityEngine.AddressableAssets;

namespace Playa.App.Cinemachine
{
    using ItemId = UInt64;

    public class CinemachineManager : CinemachineApi
    {
        [SerializeField] private CinemachineClearShot _CameraGroup;

        private Dictionary<CameraTiming, OptionsStore> _CameraLists;

        private Dictionary<ItemId, List<CameraOption>> _ItemCameraOptions;

        // TODO: This should be (item, name) tuple to camera
        private Dictionary<string, GameObject> _CameraIndicesByName;

        override public Dictionary<CameraTiming, OptionsStore> CameraLists => _CameraLists;

        public GameObject curActiveExCamObject;

        private void Awake()
        {
            _CameraLists = new Dictionary<CameraTiming, OptionsStore>();
            foreach (CameraTiming vat in Enum.GetValues(typeof(CameraTiming)))
            {
                _CameraLists.Add(vat, new OptionsStore(new List<OptionClass>(), vat + "Camera"));
            }
            _CameraIndicesByName = new Dictionary<string, GameObject> { };
            _ItemCameraOptions = new Dictionary<ItemId, List<CameraOption>> { };
        }

        override public void ActivateCamera(ItemId id, string name)
        {
            if (curActiveExCamObject != null)
            {
                curActiveExCamObject.SetActive(false);
            }

            _CameraIndicesByName[name].SetActive(true);
            curActiveExCamObject = _CameraIndicesByName[name];
        }

        override public void AddCameraRule(ItemId id, CameraTiming timing, int priority, string name, string path, Transform parent, Transform lookAtTransform)
        {
            var localCam = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
            var cam = Instantiate(localCam);
            cam.name = name;
            cam.transform.SetParent(parent);
            if (lookAtTransform != null)
            {
                CinemachineVirtualCamera vcam = cam.GetComponent<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    vcam.LookAt = lookAtTransform;
                }
            }
            cam.SetActive(false);

            var cameraOption = new CameraOption(id, timing, cam, priority);
            _CameraLists[timing].AddOption(cameraOption);
            _CameraIndicesByName[cam.name] = cam;
            if (!_ItemCameraOptions.ContainsKey(id))
            {
                _ItemCameraOptions[id] = new List<CameraOption>();
            }
            _ItemCameraOptions[id].Add(cameraOption);
        }

        override public void RemoveCameraRule(ItemId id)
        {
            if (!_ItemCameraOptions.ContainsKey(id))
            {
                return;
            }
            foreach(var cameraOption in _ItemCameraOptions[id])
            {
                _CameraLists[cameraOption.timing].RemoveOption(cameraOption);
                _CameraIndicesByName.Remove(cameraOption.camera.name);
            }
            _ItemCameraOptions.Remove(id);
        }

        override public void EnableFreeCamera(ItemId id, float Duration)
        {
            throw new System.Exception("Not implemented");
        }
    }

}
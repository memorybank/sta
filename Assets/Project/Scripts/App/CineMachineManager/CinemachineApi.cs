using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Cinemachine;
using Playa.Item;

namespace Playa.App.Cinemachine
{
    using ItemId = UInt64;
    public enum CameraTiming
    {
        Turnaround = 0,
        AllInactive = 1,
        Speaker0 = 2,
        Speaker1 = 3,
    }

    public class CameraOption : OptionClass
    {
        public ItemId item_ID;
        public CameraTiming timing;
        public GameObject camera;
        public int priority;

        public CameraOption(ItemId item_ID, CameraTiming timing, GameObject camera, int priority)
        {
            this.item_ID = item_ID;
            this.timing = timing;
            this.camera = camera;
            this.priority = priority;
        }
        public override string ToString()
        {
            return string.Format("CameraOption({0},{1})", item_ID, camera.name);
        }

        public override bool Compare(OptionClass obj)
        {
            Debug.Assert(obj is CameraOption, string.Format("Option Manager : try to compare {0} with {1}", this, obj));
            CameraOption hipObj = (CameraOption)obj;
            bool result = true;
            if ((this.item_ID != hipObj.item_ID)
                || (this.camera.name != hipObj.camera.name)
                ) result = false;
            Debug.Log(string.Format("Option Compare : {0} {1}equals to {2}", ToString(), (result ? "" : "not "), hipObj.ToString()));
            return result;
        }

        public override void OnUpdate()
        {
            //do nothing
        }

        public override void OnRemove()
        {
            //do nothing
        }
    }
    public abstract class CinemachineApi : MonoBehaviour
    {
        abstract public Dictionary<CameraTiming, OptionsStore> CameraLists { get; }

        abstract public void AddCameraRule(ItemId id, CameraTiming timing, int priority, string name, string path, Transform parent, Transform lookattransform);

        abstract public void ActivateCamera(ItemId id, string name);

        abstract public void RemoveCameraRule(ItemId id);

        abstract public void EnableFreeCamera(ItemId id, float Duration);
    }

}
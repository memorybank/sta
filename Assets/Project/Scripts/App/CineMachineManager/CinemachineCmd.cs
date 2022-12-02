using Cinemachine;
using Playa.App.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Playa.App
{
    public class CinemachineCmd { }

    public class AddCameraRuleCmd : CinemachineCmd
    {
        public CameraTiming timing;
        public int priority;
        public string name;
        public string path;
        public Transform parent;
        public Transform lookAtTransform;

        public AddCameraRuleCmd(CameraTiming timing, int priority, string name, string path, Transform parent)
        {
            this.timing = timing;
            this.priority = priority;
            this.name = name;
            this.path = path;
            this.parent = parent;
        }

        public AddCameraRuleCmd(CameraTiming timing, int priority, string name, string path, Transform parent, Transform lookattransform)
        {
            this.timing = timing;
            this.priority = priority;
            this.name = name;
            this.path = path;
            this.parent = parent;
            this.lookAtTransform = lookattransform;
        }
    }

    public class ActivateCameraCmd : CinemachineCmd
    {
        public string cameraName;

        public ActivateCameraCmd(string cameraName)
        {
            this.cameraName = cameraName;
        }
    }

    public class RemoveCameraRuleCmd : CinemachineCmd { }
}
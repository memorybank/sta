using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.App.Cinemachine;
using Playa.Item;
using System;
using Cinemachine;

namespace Playa.App
{
    using ItemId = UInt64;

    public class CinemachineUtils
    {
        public BaseItem _Item;

        public CinemachineApi _CinemachineManager => _Item._BaseApp._CinemachineManager;
        public CameraSwitchManager _CameraSwitchManager => _Item._BaseApp._CameraSwitchManager;

        List<CinemachineCmd> _DeactivateCmd;

        public CinemachineUtils(BaseItem item)
        {
            _DeactivateCmd = new List<CinemachineCmd>();
            _Item = item;
        }

        public void ExecuteCmd(CinemachineCmd cmd)
        {
            var dcmd = new CinemachineCmd();

            if (cmd.GetType() == typeof(AddCameraRuleCmd))
            {
                // This resets cameraswitch
                dcmd = new RemoveCameraRuleCmd();
            }

            ExecuteCmd(cmd, dcmd);
        }

        public void Deactivate()
        {
            // Execute in reverse order
            for (var i = _DeactivateCmd.Count - 1; i >= 0; i--)
            {
                Execute(_DeactivateCmd[i]);
            }
        }
        public void ExecuteCmd(CinemachineCmd cmd, CinemachineCmd dcmd)
        {
            Execute(cmd);
            // Add corresponding _DeactivateCmd.Add(cmd);
            _DeactivateCmd.Add(dcmd);
        }

        private void Execute(CinemachineCmd cmd)
        {
            if (cmd.GetType() == typeof(AddCameraRuleCmd))
            {
                var cmcmd = (AddCameraRuleCmd)cmd;
                _CinemachineManager.AddCameraRule(_Item.ItemId, cmcmd.timing, cmcmd.priority, cmcmd.name, cmcmd.path, cmcmd.parent, cmcmd.lookAtTransform);
            }
            else if (cmd.GetType() == typeof(ActivateCameraCmd))
            {
                var accmd = (ActivateCameraCmd)cmd;
                _CinemachineManager.ActivateCamera(_Item.ItemId, accmd.cameraName);
            }
            else if (cmd.GetType() == typeof(RemoveCameraRuleCmd))
            {
                var rcrcmd = (RemoveCameraRuleCmd)cmd;
                _CinemachineManager.RemoveCameraRule(_Item.ItemId);
                _CameraSwitchManager.RemoveMovingCam();
            }
        }

    }

}
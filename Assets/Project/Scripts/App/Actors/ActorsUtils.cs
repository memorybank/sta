using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.App.Actors;
using Playa.Item;
using Playa.Avatars.IK;

namespace Playa.App
{

    public class ActorsUtils
    {
        public BaseItem _Item;
        public ActorsApi _ActorsManager => _Item._BaseApp._ActorsManager;

        public ActorsUtils(BaseItem item)
        {           
            _Item = item;
        }       

        public void ExecuteCmd(ActorsCmd cmd)
        {
            if (cmd.GetType() == typeof(SetAvatarIdleStatusCmd))
            {
                var sascmd = (SetAvatarIdleStatusCmd)cmd;
                _ActorsManager.SetAvatarIdleStatus(_Item.ItemId, sascmd.avatarUser, sascmd.group, sascmd.secondGroup);
            }
            else if (cmd.GetType() == typeof(SetAvatarIdleSubStatusCmd))
            {
                var sasscmd = (SetAvatarIdleSubStatusCmd)cmd;
                _ActorsManager.SetAvatarIdleSubStatus(_Item.ItemId, sasscmd.avatarUser, sasscmd.group, sasscmd.priority);
            }
            else if (cmd.GetType() == typeof(SetAvatarSilenceStatusCmd))
            {
                var sasscmd = (SetAvatarSilenceStatusCmd)cmd;
                _ActorsManager.SetAvatarSilenceStatus(_Item.ItemId, sasscmd.avatarUser, sasscmd.group);
            }
            else if (cmd.GetType() == typeof(SetAvatarActionStatusCmd))
            {
                var sasscmd = (SetAvatarActionStatusCmd)cmd;
                _ActorsManager.SetAvatarActionStatus(_Item.ItemId, sasscmd.avatarUser, sasscmd.group);
            }
            else if (cmd.GetType() == typeof(SetAvatarPrefabCmd))
            {
                var sapcmd = (SetAvatarPrefabCmd)cmd;
                _ActorsManager.SetAvatarPrefab(sapcmd.avatarUser, sapcmd.index);
            }
            else if (cmd.GetType() == typeof(SetLookAtIKCmd))
            {
                var slaicmd = (SetLookAtIKCmd)cmd;
                _ActorsManager.SetIKLookAtObject(_Item.ItemId, slaicmd.voiceActivityType, slaicmd.avatarUser, slaicmd.lookAtGobj, slaicmd.priority, slaicmd.headWeight, slaicmd.bodyWeight);
            }
            else if (cmd.GetType() == typeof(RegisterAvatarItemSlotCmd))
            {
                var rascmd = (RegisterAvatarItemSlotCmd)cmd;
                _ActorsManager.RegisterSlots(_Item, rascmd.avatarUser, rascmd.slotNames, rascmd.ikTargets);
            }
            else if (cmd.GetType() == typeof(UpdateAvatarItemSlotCmd))
            {
                var uascmd = (UpdateAvatarItemSlotCmd)cmd;
                _ActorsManager.UpdateSlots(_Item, uascmd.avatarUser, uascmd.slotNames, uascmd.ikTargets);
            }
            else if (cmd.GetType() == typeof(ClearAvatarItemSlotCmd))
            {
                var cascmd = (ClearAvatarItemSlotCmd)cmd;
                _ActorsManager.ClearSlots(cascmd.avatarUser, cascmd.slotNames);
            }
            else if (cmd.GetType() == typeof(SetAvatarPositionSlotCmd))
            {
                var sapscmd = (SetAvatarPositionSlotCmd)cmd;
                _ActorsManager.SetAvatarPositionSlot(_Item, sapscmd.priority, sapscmd.avatarUser, sapscmd.slotIndex, sapscmd.userIndex);
            }
            else if (cmd.GetType() == typeof(RemoveAvatarPositionSlotCmd))
            {
                var eapscmd = (RemoveAvatarPositionSlotCmd)cmd;
                _ActorsManager.RemoveAvatarPositionSlot(_Item, eapscmd.avatarUser, eapscmd.slotIndex);
            }
        }
    }
}
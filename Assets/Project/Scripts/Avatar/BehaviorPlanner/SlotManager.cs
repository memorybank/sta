using Playa.Item;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars
{

    public enum SlotName
    {
        NA,
        Hand,
        Body,
        Face,
        Head,
        Follow,
    }

    public class SlotManager
    {
        public class PerSlot
        {
            public BaseItem Item;
            public Dictionary<IKEffectorName, IKTarget> IkTargets;
            public PerSlot(BaseItem item, Dictionary<IKEffectorName, IKTarget> ikTargets)
            {
                Item = item;
                IkTargets = ikTargets;
            }
        }

        // Parent pointer
        private AvatarUser _AvatarUser;
        private MultiIKManager MultiIKManager => _AvatarUser.MultiIKManager;

        private Dictionary<SlotName, PerSlot> _SlotsHistory;
        private Dictionary<IKEffectorName, int> _CurrentPriority;
        private Dictionary<IKEffectorName, IKTarget> _FinalTargets;

        public SlotManager(AvatarUser avatarUser)
        {
            _CurrentPriority = new Dictionary<IKEffectorName, int>();

            foreach (IKEffectorName ikEffectorName in Enum.GetValues(typeof(IKEffectorName)))
            {
                _CurrentPriority.Add(ikEffectorName, 0);
            }

            _SlotsHistory = new Dictionary<SlotName, PerSlot>();

            _AvatarUser = avatarUser;
        }

        public void RegisterSlots(List<SlotName> slotName, BaseItem item, Dictionary<IKEffectorName, IKTarget> targets)
        {
            foreach(var slot in slotName)
            {
                RegisterSlot(slot, item, targets);
            }
        }

        public void UpdateSlots(List<SlotName> slotName, Dictionary<IKEffectorName, IKTarget> targets)
        {
            foreach (var slot in slotName)
            {
                UpdateSlot(slot, targets);
            }
        }

        public void ClearSlots(List<SlotName> slotNames)
        {
            foreach (var slot in slotNames)
            {
                ClearSlot(slot);
            }
        }

        public void RegisterSlot(SlotName slotName, BaseItem item, Dictionary<IKEffectorName, IKTarget> targets)
        {
            Debug.Log("Item Events RegisterSlot " + slotName);

            if (!_SlotsHistory.ContainsKey(slotName))
            {
                PerSlot perSlot = new PerSlot(item, targets);
                _SlotsHistory.Add(slotName, perSlot);
            }
            else
            {
                ClearSlot(slotName);
                PerSlot perSlot = new PerSlot(item, targets);
                _SlotsHistory.Add(slotName, perSlot);
            }
            UpdateSlot(slotName, targets);
        }

        public void UpdateSlot(SlotName slotName, Dictionary<IKEffectorName, IKTarget> targets)
        {
            // Merge in results if targets come from another dictionary
            if (_SlotsHistory[slotName].IkTargets != targets)
            {
                foreach (var kvp in targets)
                {
                    _SlotsHistory[slotName].IkTargets[kvp.Key] = kvp.Value;

                }
            }

            if (_FinalTargets!= null &&¡¡_FinalTargets.ContainsKey(IKEffectorName.LookAt) && _SlotsHistory.ContainsKey(slotName))
            {
                Debug.Log("Item Events head " + _FinalTargets[IKEffectorName.LookAt].target.gameObject.name + " " + _SlotsHistory[slotName].Item.ItemProperties.Name);
            }

            CalculateFinalTargets();
        }

        public void ClearSlot(SlotName slotName)
        {
            Debug.Log("Item Events ClearSlot " + slotName);

            if (_SlotsHistory.ContainsKey(slotName))
            {
                PerSlot perSlot = _SlotsHistory[slotName];
                perSlot.Item.Deactivate();

                _SlotsHistory.Remove(slotName);

                MultiIKManager.ClearIKAll();

                CalculateFinalTargets();
            }
        }
        private void CalculateFinalTargets()
        {
            var newPrioprity = new Dictionary<IKEffectorName, int>();

            foreach (var kvp in _CurrentPriority)
            {
                newPrioprity[kvp.Key] = 0;
            }
            _CurrentPriority = newPrioprity;

            _FinalTargets = new Dictionary<IKEffectorName, IKTarget>();
            foreach (var kvp in _SlotsHistory)
            {
                foreach (var kv in kvp.Value.IkTargets)
                {
                    if (kv.Value.priority >= _CurrentPriority[kv.Key])
                    {
                        _FinalTargets[kv.Key] = kv.Value;
                        _CurrentPriority[kv.Key] = kv.Value.priority;
                    }

                    var gameObject = "null";
                    if (kv.Value.target != null)
                    {
                        gameObject = kv.Value.target.parent.parent.name;
                    }
                    Debug.Log(string.Format("Item Events Adding back slot {0} {1} {2}", kvp.Key, kv.Key, gameObject));
                }
            }
            MultiIKManager.SetIKTargets(_FinalTargets);
        }
    }
}
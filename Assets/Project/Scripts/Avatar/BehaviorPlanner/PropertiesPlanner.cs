using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playa.Animations;
using Animancer.FSM;
using TMPro;
using Playa.Common;
using Playa.Common.Utils;
using UnityEngine.Events;


namespace Playa.Avatars
{
    using ItemID = UInt64;
    public struct AvatarProperties
    {
        public float Speed;
    }

    public class PropertiesPlanner
    {
        private AvatarProperties _AvatarProperties;
        private UnityEvent<bool, float> _SpeedChange;
        private Dictionary<ItemID, UnityAction<bool, float>> _AvatarSpeedListeners;

        public AvatarProperties AvatarProperties => _AvatarProperties;

        public PropertiesPlanner()
        {
            _AvatarProperties = new AvatarProperties();
            _AvatarSpeedListeners = new Dictionary<ItemID, UnityAction<bool, float>>();
            _AvatarProperties.Speed = 0;
            _SpeedChange = new UnityEvent<bool, float>();
        }

        public void AddSpeedChangeListeners(ItemID itemid, Action<bool, float> action)
        {
            UnityAction<bool, float> unityAction = (isSelfChange, speed) => action(isSelfChange, speed);
            _AvatarSpeedListeners.Add(itemid, unityAction);
            _SpeedChange.AddListener(unityAction);
        }

        public void RemoveSpeedChangeListeners(ItemID itemid, Action<bool, float> action)
        {
            if (_AvatarSpeedListeners.ContainsKey(itemid))
            {
                _SpeedChange.RemoveListener(_AvatarSpeedListeners[itemid]);
            }
        }

        public void ChangeSpeed(float newSpeed, bool isSelfChange)
        {
            if (newSpeed != _AvatarProperties.Speed)
            {
                _AvatarProperties.Speed = newSpeed;
                _SpeedChange.Invoke(isSelfChange, _AvatarProperties.Speed);
            }
        }
    }
}

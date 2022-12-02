using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Playa.Item
{
    using ItemId = UInt64;
    public abstract class ItemApi : MonoBehaviour
    {
        abstract public void AddInteraction(string name, UnityEvent condition, UnityAction action);

        abstract public void RemoveInteractionById(string name);

        abstract public void PlayMovableAnimationBySpeed(string movableKey, string animKey, float normalizedTime, float toSet);
    }
}
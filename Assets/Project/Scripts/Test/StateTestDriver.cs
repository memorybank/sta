using Playa.App;
using Playa.Avatars;
using Playa.Common;
using Playa.Item;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Playa.Test
{
    public class StateTestDriver : TestDriver
    {
        GameObject _CurrentApp;
        protected override string TestName => "StateTest";

        protected override IEnumerator Test()
        {
            _CurrentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text);
            _AvatarBrain0.EventSequencer.StartSequence();
            _AvatarBrain1.EventSequencer.StartSequence();
            _Testing.color = Color.red;

            // Speaker 0 speaking
            _AvatarBrain0.EventSequencer.Push(new VoiceActivityUnit(VoiceActivityType.Active));
            yield return StateLoop();

            // Speaker 0 idle
            _AvatarBrain0.EventSequencer.Push(new VoiceActivityUnit(VoiceActivityType.Inactive));
            yield return StateLoop();

            // Speaker 0 silence
            _AvatarBrain0.EventSequencer.Push(new VoiceActivityUnit(VoiceActivityType.Silence));
            yield return StateLoop();

            _Testing.text = "Running test finished";
            yield return new WaitForSeconds(0f);
        }

        private void CreateItem(Type type)
        {
            var test = new GameObject(type.ToString());
            test.transform.SetParent(_CurrentApp.transform);
            test.AddComponent(type);
            BaseItem item;
            item = test.GetComponent<BaseItem>();
            item._BaseApp = _CurrentApp.GetComponent<BaseApp>();
            item.ActivateByUser("1001");
        }

        private IEnumerator StateLoop()
        {
            foreach (var type1 in _SlotToItemIndices[SlotName.Hand])
            {
                CreateItem(type1);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 3.0f));
                foreach (var type2 in _SlotToItemIndices[SlotName.Body])
                {
                    CreateItem(type2);
                    UpdateUI(type1, type2);
                    yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 3.0f));
                }
            }
            CreateItem(typeof(ClearFullbody));
            yield return new WaitForSeconds(3.0f);

            foreach (var type1 in _SlotToItemIndices[SlotName.Body])
            {
                CreateItem(type1);
                yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 3.0f));
                foreach (var type2 in _SlotToItemIndices[SlotName.Hand])
                {
                    CreateItem(type2);
                    UpdateUI(type1, type2);
                    yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 3.0f));
                }
            }
            CreateItem(typeof(ClearFullbody));
            yield return new WaitForSeconds(5.0f);
        }
    }
}
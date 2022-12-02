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
    public class ChatTestDriver : TestDriver
    {
        GameObject _CurrentApp;
        protected override string TestName => "ChatTest";

        protected override IEnumerator Test()
        {
            _CurrentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text);
            _AvatarBrain0.EventSequencer.StartSequence();
            _AvatarBrain1.EventSequencer.StartSequence();
            _Testing.color = Color.red;

            UpdateUI(null, null);
            yield return ChatLoop();

            yield return FullChatLoop();
                       
            TMPro.TMP_InputField AvatarIndexText = GameObject.Find("AvatarIndexText").GetComponent<TMPro.TMP_InputField>();
            AvatarIndexText.text = "1";

            yield return FullChatLoop();

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


        private IEnumerator FullChatLoop()
        {
            foreach (var type1 in _SlotToItemIndices[SlotName.Hand])
            {
                CreateItem(type1);
                UpdateUI(type1, null);
                yield return ChatLoop();
                CreateItem(typeof(ClearFullbody));
            }

            foreach (var type2 in _SlotToItemIndices[SlotName.Body])
            {
                CreateItem(type2);
                UpdateUI(null, type2);
                yield return ChatLoop();
                CreateItem(typeof(ClearFullbody));
            }

            foreach (var type1 in _SlotToItemIndices[SlotName.Hand])
            {
                foreach (var type2 in _SlotToItemIndices[SlotName.Body])
                {
                    CreateItem(type1);
                    CreateItem(type2);
                    UpdateUI(type1, type2);
                    yield return ChatLoop();
                    CreateItem(typeof(ClearFullbody));
                }
            }
        }
        private IEnumerator ChatLoop()
        {
            // Speaker 0 talking
            _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(VoiceActivityType.Active), _AvatarBrain0);
            yield return new WaitForSeconds(UnityEngine.Random.Range(3.0f, 5.0f));

            // Speaker 0 idle, Speaker 1 talking
            _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(VoiceActivityType.Inactive), _AvatarBrain0);
            _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(VoiceActivityType.Active), _AvatarBrain1);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));

            // Speaker 0 speaking, Speaker 1 idle
            _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(VoiceActivityType.Active), _AvatarBrain0);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 2.0f));

            _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(VoiceActivityType.Inactive), _AvatarBrain1);
            yield return new WaitForSeconds(UnityEngine.Random.Range(3.0f, 5.0f));

            // Speaker 0 idle
            _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(VoiceActivityType.Inactive), _AvatarBrain0);
            yield return new WaitForSeconds(3.0f);

            // Silence(wait for 3s)
            yield return new WaitForSeconds(UnityEngine.Random.Range(3.0f, 5.0f));
        }
    }
}
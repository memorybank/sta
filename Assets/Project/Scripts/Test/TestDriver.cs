using Playa.Avatars;
using Playa.Item;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Playa.Event;

namespace Playa.Test
{
    public abstract class TestDriver : MonoBehaviour
    {
        [SerializeField] protected AvatarBrain _AvatarBrain0;
        [SerializeField] protected AvatarBrain _AvatarBrain1;

        [SerializeField] protected TextMeshProUGUI _Testing;
        [SerializeField] protected TMP_Dropdown _TestingDropdown;
        [SerializeField] protected Dropdown _AppDropDown;
        [SerializeField] protected EventSequencerManager _EventSequencerManager;

        virtual protected string TestName { get; }
        protected Dictionary<SlotName, List<Type>> _SlotToItemIndices;

        // Start is called before the first frame update
        void Start()
        {
            _SlotToItemIndices = new Dictionary<SlotName, List<Type>>();
            _SlotToItemIndices[SlotName.Hand] = new List<Type> { typeof(Guitar), typeof(Drinking), typeof(MobilePhone) };
            _SlotToItemIndices[SlotName.Body] = new List<Type> { typeof(Sit_Chair) , typeof(Walking), typeof(Sofa) };           
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && _TestingDropdown.options[_TestingDropdown.value].text == TestName)
            {
                StartCoroutine(Test());
            }
        }

        protected abstract IEnumerator Test();

        protected void UpdateUI(Type type1, Type type2)
        {
            var t1 = type1 != null ? type1.ToString() : "Null";
            var t2 = type2 != null ? type2.ToString() : "Null";
            _Testing.text = String.Format("Running test {0} {1}", t1, t2);
        }
    }
}
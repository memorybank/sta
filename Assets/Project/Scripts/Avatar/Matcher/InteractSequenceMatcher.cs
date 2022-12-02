using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Playa.Common.Anim.Event;

using Playa.Config;
using cfg.gesture;

namespace Playa.Avatars
{

    public class InteractSequenceMatcher : Matcher<Animations.AnimationSequenceGenerator>
    {
        [SerializeField] private int chosen;

        [SerializeField] private Dropdown _SequenceDropdown;

        [SerializeField] private ConfigsLoader _ConfigLoader;

        public MetronomicAnimSequenceEvent metronomicAnimSequenceEvent;

        public InteractSequenceMatcher(Animations.AnimationSequenceGenerator animationSequenceGenerator) : base(animationSequenceGenerator)
        {
        }

        void Start()
        {
            _ConfigLoader = ConfigsLoader.Instance;

            StartCoroutine(WaitForClipMapReady());
            //InitSequenceDropdown();
        }

        public IEnumerator WaitForClipMapReady()
        {
            while (((Playa.Animations.InteractMetronomicSequenceGenerator)DataSource).ClipTypeClipsMap.Count <= 0)
            {
                yield return new WaitForSeconds(0.1f);
            }

            InitSequenceDropdown();
            yield return null;
        }

        private void InitSequenceDropdown()
        {
         
            List<Dropdown.OptionData> SequenceNames = new List<Dropdown.OptionData>();
            
            for (int i = 0; i < _ConfigLoader.Tables.TbGestureSequence.DataList.Count; i++)
            {
                bool isLackType = false;
                foreach (cfg.gesture.Type mtype in _ConfigLoader.Tables.TbGestureSequence.DataList[i].Sequence)
                {
                    if (mtype == cfg.gesture.Type.INVALID)
                    {
                        continue;
                    }

                    if (!((Playa.Animations.InteractMetronomicSequenceGenerator)DataSource).ClipTypeClipsMap.ContainsKey(mtype))
                    {
                        Debug.LogWarning(string.Format("InitSequenceDropdown错误，sequence{0}缺少{1}类型节拍手势", _ConfigLoader.Tables.TbGestureSequence.DataList[i].Id, mtype));
                        isLackType = true;
                        break;
                    }
                }

                if (isLackType)
                {
                    continue;
                }
                SequenceNames.Add(new Dropdown.OptionData("sequence"+_ConfigLoader.Tables.TbGestureSequence.DataList[i].Name));
            }

            _SequenceDropdown.options = SequenceNames; ;

            _SequenceDropdown.value =
                _SequenceDropdown.options.FindIndex(option => option.text == "");

            _SequenceDropdown.onValueChanged.AddListener(index =>
            {
                var optionText = _SequenceDropdown.options[index].text;
                OnChosenChanged(index);
            });
        }

        private void OnChosenChanged(int index)
        {
            chosen = index;

            metronomicAnimSequenceEvent?.Invoke(new MetronomicAnimSequenceUnit(true, chosen));
        }

        public override MatchResult match(AvatarBehavior behavior)
        {
            if (chosen >= DataSource.TotalNetwork)
            {
                return new MatchResult();
            }

            Debug.Log("match " + chosen.ToString());
            MatchResult m = new MatchResult();
            m.AnimationClipIndex = chosen;

            return m;
        }
    }
}
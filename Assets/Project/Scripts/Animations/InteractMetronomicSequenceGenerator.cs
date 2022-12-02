using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Playa.Common.Utils;

using Playa.Config;
using cfg.gesture;
using System.Linq;

using Playa.Common.Anim.Event;
using Animancer;

namespace Playa.Animations
{
    public class InteractMetronomicSequenceGenerator : AnimationSequenceGenerator
    {

        [SerializeField]
        private AnimationRepository _Repository;

        [SerializeField]
        private string _CurrentClipName;

        public override string CurrentClipName => _CurrentClipName;

        private cfg.gesture.Type _CurrentClipType;

        [SerializeField] private ConfigsLoader _ConfigLoader;

        private Dictionary<cfg.gesture.Type, List<string>> _ClipTypeClipsMap;

        public Dictionary<cfg.gesture.Type, List<string>> ClipTypeClipsMap => _ClipTypeClipsMap;

        private List<AnimationNetwork> _Networks;

        private System.Random rnd = new System.Random();

        public override int TotalNetwork => _Networks.Count;

        public int CurrentChooseClipTypeClipIndex;

        [SerializeField] private Dropdown _ClipTypeClipDropdown;

        public MetronomicAnimClipEvent metronomicAnimClipEvent;

        [SerializeField] private Dropdown _NextClipTypeClipDropdown;

        private cfg.gesture.Type _ChosenNextMetronomicType = cfg.gesture.Type.INVALID;

        private int _ChosenNextClipIndex;

        private Dictionary<string, cfg.gesture.Type> _NextChosenClipNameClipTypeDict;

        // UI components
        [SerializeField] private TextMeshProUGUI _resultText;

        private int _NeedWaitResetSignalCounts = 0;

        private int _ResetSignalCounts = 0;

        [SerializeField] private Button _Button;

        public override void Init()
        {
            _ConfigLoader = ConfigsLoader.Instance;

            _Networks = new List<AnimationNetwork>();

            _ClipTypeClipsMap = new Dictionary<cfg.gesture.Type, List<string>>();

            // with config check
            foreach (string clipname in _Repository.AnimationClipInfos.Keys)
            {
                if (((GestureClipInfo)_Repository.AnimationClipInfos[clipname]).GestureMark == null)
                {
                    continue;
                }

                if (_Repository.GetClipIndexByName(clipname) == -1)
                {
                    Debug.LogWarning(string.Format("MetronomicSequenceGenerator 配置解析错误，缺少{0}.fbx 节拍手势文件", clipname));
                    continue;
                }

                if (!_ClipTypeClipsMap.ContainsKey(((GestureClipInfo)_Repository.AnimationClipInfos[clipname]).GestureMark.Type))
                {
                    _ClipTypeClipsMap[((GestureClipInfo)_Repository.AnimationClipInfos[clipname]).GestureMark.Type] = new List<string>();
                }
                _ClipTypeClipsMap[((GestureClipInfo)_Repository.AnimationClipInfos[clipname]).GestureMark.Type].Add(clipname);
            }

            for (int i = 0; i < _ConfigLoader.Tables.TbGestureSequence.DataList.Count; i++)
            {
                bool isLackType = false;
                foreach (cfg.gesture.Type mtype in _ConfigLoader.Tables.TbGestureSequence.DataList[i].Sequence)
                {
                    if (mtype == cfg.gesture.Type.INVALID)
                    {
                        continue;
                    }

                    if (!_ClipTypeClipsMap.ContainsKey(mtype))
                    {
                        Debug.LogWarning(string.Format("MetronomicSequenceGenerator 配置解析错误，sequence{0}缺少{1}类型节拍手势", _ConfigLoader.Tables.TbGestureSequence.DataList[i].Id, mtype));
                        isLackType = true;
                        break;
                    }
                }

                if (isLackType)
                {
                    continue;
                }

                _Networks.Add(new MetronomicNetwork(_ConfigLoader.Tables.TbGestureSequence.DataList[i], _Repository));
            }

            //_CurrentClipType = _ClipTypeClipsMap.Keys.First();
            PickStartPoint(0);
            InitClipTypeClipDropdown();
            InitNextClipTypeClipDropdown();
            _Button.onClick.AddListener(OnClickNext);

            IsInit = true;
        }

        public void IncNeedWaitResetSignalCounts()
        {
            _NeedWaitResetSignalCounts++;
            Debug.Log("++");
        }

        public void OnReceiveResetSignal()
        {
            _ResetSignalCounts++;
            Debug.Log(_ResetSignalCounts + " / " + _NeedWaitResetSignalCounts);
            if (_ResetSignalCounts >= _NeedWaitResetSignalCounts)
            {
                ResetUI();
                _ResetSignalCounts = 0;
            }
        }

        public void ResetClipTypeClipDropdown()
        {
            List<Dropdown.OptionData> ClipTypeClipNames = new List<Dropdown.OptionData>();

            for (int i = 0; i < _ClipTypeClipsMap[_CurrentClipType].Count; i++)
            {
                ClipTypeClipNames.Add(new Dropdown.OptionData(_ClipTypeClipsMap[_CurrentClipType][i]));
            }

            _ClipTypeClipDropdown.options = ClipTypeClipNames;

            _ClipTypeClipDropdown.value = CurrentChooseClipTypeClipIndex;
 //               _ClipTypeClipDropdown.options.FindIndex(option => option.text == "");
        }

        private void InitClipTypeClipDropdown()
        {
            ResetClipTypeClipDropdown();

            _ClipTypeClipDropdown.value = _ClipTypeClipDropdown.options.FindIndex(option => option.text == "");
            //only add listen at first reset
            _ClipTypeClipDropdown.onValueChanged.AddListener(index =>
            {
                var optionText = _ClipTypeClipDropdown.options[index].text;
                OnClipChosenChanged(index);
            });
        }

        private void OnClipChosenChanged(int index)
        {
            CurrentChooseClipTypeClipIndex = index;
            metronomicAnimClipEvent?.Invoke(new MetronomicAnimClipUnit(false, GetCurrentClip()));
        }

        public void ResetNextClipTypeClipDropdown()
        {
            _NextChosenClipNameClipTypeDict = new Dictionary<string, cfg.gesture.Type>();

            List<Dropdown.OptionData> NextClipTypeClipNames = new List<Dropdown.OptionData>();

            List<cfg.gesture.Type> nexts = _Networks[CurrentIndex].GetReachableClipTypes(_CurrentClipType);

            for (int i = 0; i < nexts.Count; i++)
            {
                for (int j = 0; j < _ClipTypeClipsMap[nexts[i]].Count; j++)
                {
                    NextClipTypeClipNames.Add(new Dropdown.OptionData(_ClipTypeClipsMap[nexts[i]][j]));
                    _NextChosenClipNameClipTypeDict[_ClipTypeClipsMap[nexts[i]][j]] = nexts[i];
                }
            }

            _NextClipTypeClipDropdown.options = NextClipTypeClipNames;

            _NextClipTypeClipDropdown.value =
                _NextClipTypeClipDropdown.options.FindIndex(option => option.text == "");
        }

        private void InitNextClipTypeClipDropdown()
        {
            ResetNextClipTypeClipDropdown();

            //only add listen at first reset
            _NextClipTypeClipDropdown.onValueChanged.AddListener(index =>
            {
                var optionText = _NextClipTypeClipDropdown.options[index].text;
                //_ChosenNextClipIndex = index;
                OnNextClipChosenChanged(optionText);
            });
        }

        private void OnNextClipChosenChanged(string optionText)
        {
            if (!_NextChosenClipNameClipTypeDict.TryGetValue(optionText, out cfg.gesture.Type value))
            {
                _ChosenNextMetronomicType = cfg.gesture.Type.INVALID;
                return;
            }
            _ChosenNextMetronomicType = _NextChosenClipNameClipTypeDict[optionText];
            for (int i = 0; i < _ClipTypeClipsMap[_ChosenNextMetronomicType].Count; i++)
            {
                if (_ClipTypeClipsMap[_ChosenNextMetronomicType][i] == optionText)
                {
                    _ChosenNextClipIndex = i;
                }
            }
        }

        private string MatchClipNameByType(cfg.gesture.Type clipType)
        {
            List<string> nexts = _ClipTypeClipsMap[clipType];

            if (CurrentChooseClipTypeClipIndex >= nexts.Count)
            {
                return nexts[0];
            }

            return nexts[CurrentChooseClipTypeClipIndex];
        }

        public void ResetUI()
        {
            _ChosenNextClipIndex = 0;
            ResetClipTypeClipDropdown();
            ResetNextClipTypeClipDropdown();
        }

        public ClipTransition GetCurrentClip()
        {
            _CurrentClipName = MatchClipNameByType(_CurrentClipType);
            var clipIndex = _Networks[CurrentIndex].Repository.GetClipIndexByName(_CurrentClipName);
            Debug.Log("GetCurrentClip " + clipIndex);
            Debug.Assert((clipIndex >= 0 && clipIndex < _Networks[CurrentIndex].Repository.AnimationClips.Count), "AnimationClip Get ClipIndex invalid: " + clipIndex + " current NetWork: " + _Networks[CurrentIndex].ToString());
            var result = _Networks[CurrentIndex].Repository.AnimationClips[clipIndex];
            Debug.Log(result.Clip.name);
            UpdateUI();

            return result;
        }

        public override void PickStartPoint(int index)
        {
            CurrentIndex = index;
            Debug.Log("network current " + _Networks.ToString());
            Debug.Log("network current " + _Networks[CurrentIndex].ToString());
            Debug.Log("network current start point " + _Networks[CurrentIndex].StartPoint);
            _CurrentClipType = (cfg.gesture.Type)Enum.Parse(typeof(cfg.gesture.Type), _Networks[CurrentIndex].StartPoint);

            CurrentChooseClipTypeClipIndex = 0;
            _ChosenNextClipIndex = 0;
            _ChosenNextMetronomicType = cfg.gesture.Type.INVALID;
            _CurrentClipName = MatchClipNameByType(_CurrentClipType);
            UpdateUI();
        }

        public override ClipTransition GenNextClip()
        {
            if (_ChosenNextMetronomicType != cfg.gesture.Type.INVALID)
            {
                _CurrentClipType = _ChosenNextMetronomicType;
                CurrentChooseClipTypeClipIndex = _ChosenNextClipIndex;
                _ChosenNextClipIndex = 0;
                _ChosenNextMetronomicType = cfg.gesture.Type.INVALID;
            }
            else
            {
                var nexts = _Networks[CurrentIndex].GetReachableClipTypes(_CurrentClipType);

                if (nexts != null && nexts.Count > 0)
                {
                    _CurrentClipType = nexts[rnd.Next() % nexts.Count];
                }
            }
            _CurrentClipName = MatchClipNameByType(_CurrentClipType);

            var clipIndex = _Networks[CurrentIndex].Repository.GetClipIndexByName(_CurrentClipName);
            Debug.Assert((clipIndex >= 0 && clipIndex < _Networks[CurrentIndex].Repository.AnimationClips.Count), "AnimationClip Get ClipIndex invalid: " + clipIndex + " current NetWork: " + _Networks[CurrentIndex].ToString());
            var result = _Networks[CurrentIndex].Repository.AnimationClips[clipIndex];
            UpdateUI();

            return result;
        }

        private void OnClickNext()
        {
            ClipTransition clip = GenNextClip();
            metronomicAnimClipEvent?.Invoke(new MetronomicAnimClipUnit(true, clip));
        }

        private void UpdateUI()
        {
            _resultText.text = string.Format("sequence:{0}, type:{1}, clip:{2}, 幅度: {3}", CurrentIndex, _CurrentClipType.ToString(), _CurrentClipName, ((GestureClipInfo)_Networks[CurrentIndex].Repository.AnimationClipInfos[_CurrentClipName]).GestureMark.Amplitude.ToString());
        }
    }

}
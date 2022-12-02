using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Playa.Common;
using Playa.Common.Utils;

using Playa.Config;
using cfg.gesture;
using Animancer;

namespace Playa.Animations
{
    public class MetronomicSequenceGenerator : AnimationSequenceGenerator
    {
        [SerializeField]
        private AnimationRepository _Repository;

        [SerializeField]
        private string _CurrentClipName;

        public override string CurrentClipName => _CurrentClipName;

        private int _CurrentClipTypeIndexInSequence = 0;

        private cfg.gesture.Type _CurrentClipType;

        [SerializeField] private ConfigsLoader _ConfigLoader;

        private Dictionary<cfg.gesture.Type, List<string>> _ClipTypeClipsMap;

        private List<AnimationNetwork> _Networks;

        private System.Random rnd = new System.Random();

        public override int TotalNetwork => _Networks.Count;

        // UI components
        [SerializeField] private TextMeshProUGUI _resultText;
        public override void Init()
        {
            _ConfigLoader = ConfigsLoader.Instance;

            _Networks = new List<AnimationNetwork>();

            _ClipTypeClipsMap = new Dictionary<cfg.gesture.Type, List<string>>();

            List<cfg.gesture.Type> metronomicTypeConfigCheckList = new List<cfg.gesture.Type>();
            for (int i = 0; i < _ConfigLoader.Tables.TbGestureSequence.DataList.Count; i++)
            {
                _Networks.Add(new MetronomicNetwork(_ConfigLoader.Tables.TbGestureSequence.DataList[i], _Repository));

                foreach (cfg.gesture.Type mtype in _ConfigLoader.Tables.TbGestureSequence.DataList[i].Sequence)
                {
                    if (metronomicTypeConfigCheckList.Contains(mtype))
                    {
                        metronomicTypeConfigCheckList.Add(mtype);
                    }
                }
            }

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
            foreach(string clipname in _Repository.AnimationClipInfos.Keys)
            { 
                if (!_ClipTypeClipsMap.ContainsKey(((GestureClipInfo)_Repository.AnimationClipInfos[clipname]).GestureMark.Type))
                {
                    _ClipTypeClipsMap[((GestureClipInfo)_Repository.AnimationClipInfos[clipname]).GestureMark.Type] = new List<string>();
                }
                _ClipTypeClipsMap[((GestureClipInfo)_Repository.AnimationClipInfos[clipname]).GestureMark.Type].Add(clipname);

                Debug.Assert(_Repository.GetClipIndexByName(clipname) != -1, string.Format("MetronomicSequenceGenerator 配置解析错误，缺少{0}.fbx 节拍手势文件", clipname));
            }

            // config check
            foreach (cfg.gesture.Type mtype in metronomicTypeConfigCheckList)
            {
                if (mtype == cfg.gesture.Type.INVALID)
                {
                    continue;
                }
                Debug.Assert(_ClipTypeClipsMap.ContainsKey(mtype), string.Format("MetronomicSequenceGenerator 配置解析错误，缺少{0}类型节拍手势", mtype));
            }
        }

        private string MatchClipNameByType(cfg.gesture.Type clipType)
        {
            List<string> nexts = _ClipTypeClipsMap[clipType];
            return nexts[rnd.Next() % nexts.Count];
        }

        public override void PickStartPoint(int index)
        {
            if (index < 0)
            {
                Debug.LogWarning("MetronomicSequenceGenerator.PickStartPoint current pick index less then 0 " + index.ToString());
                index = 0;
            }
            CurrentIndex = index;
            _CurrentClipTypeIndexInSequence = 0;
            _CurrentClipType = (cfg.gesture.Type)Enum.Parse(typeof(cfg.gesture.Type), _Networks[CurrentIndex].StartPoint);

            _CurrentClipName = MatchClipNameByType(_CurrentClipType);
            UpdateUI();
        }

        private ClipTransition GenClipByCurrentClipName()
        {
            var nexts = _Networks[CurrentIndex].GetReachableClipTypes(_CurrentClipType);

            if (nexts != null && nexts.Count > 0)
            {
                _CurrentClipType = nexts[rnd.Next() % nexts.Count];
                _CurrentClipName = MatchClipNameByType(_CurrentClipType);
            }
            var clipIndex = _Networks[CurrentIndex].Repository.GetClipIndexByName(_CurrentClipName);
            Debug.Assert((clipIndex >= 0 && clipIndex < _Networks[CurrentIndex].Repository.AnimationClips.Count), 
                "AnimationClip Get ClipIndex invalid: " + clipIndex + " current NetWork: " + _Networks[CurrentIndex].ToString());
            var result = _Networks[CurrentIndex].Repository.AnimationClips[clipIndex];
            UpdateUI();

            return result;
        }

        public ClipTransition GenCurrentClip()
        {
            _CurrentClipName = MatchClipNameByType(_CurrentClipType);

            return GenClipByCurrentClipName();
        }

        public override ClipTransition GenNextClip()
        {
            int newIndexInSequence = 0;
            MetronomicSequenceUnit nextSequenceUnit = _Networks[CurrentIndex].GetNextClipType(_CurrentClipTypeIndexInSequence);
            _CurrentClipType = nextSequenceUnit.MetronomicType;
            _CurrentClipName = MatchClipNameByType(_CurrentClipType);

            _CurrentClipTypeIndexInSequence = nextSequenceUnit.NewIndexInSequence;

            return GenClipByCurrentClipName();
        }

        public Amplitude GenAmplitudeBySentimentUnit(float v)
        {
            if (v <= -0.5)
            {
                return Amplitude.SMALL;
            }
            else if (v >= 0.5)
            {
                return Amplitude.LARGE;
            }
            else
            {
                return Amplitude.MEDIUM;
            }
        }

        private void UpdateUI()
        {
            _resultText.text = string.Format("sequence:{0}, type:{1}, clip:{2}", CurrentIndex, _CurrentClipType.ToString(), _CurrentClipName);
        }
    }

}
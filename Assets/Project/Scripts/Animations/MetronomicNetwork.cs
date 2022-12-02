using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Playa.Config;
using cfg.gesture;

namespace Playa.Animations
{

    public class MetronomicNetwork : AnimationNetwork
    {
        [SerializeField] private string _NetworkName;

        //todo deprecate
        private Dictionary<cfg.gesture.Type, List<cfg.gesture.Type>> _ReachableClipTypes;

        private List<cfg.gesture.Type> _ClipTypesSequence;

        private bool _Init;


        public MetronomicNetwork(GestureSequence sequence, AnimationRepository repo)
        {
            _Repository = repo;

            try
            {
                _NetworkName = sequence.Id.ToString();
                _StartPoint = sequence.Sequence[0].ToString();

                _ReachableClipTypes = new Dictionary<cfg.gesture.Type, List<cfg.gesture.Type>>();
                _ClipTypesSequence = new List<cfg.gesture.Type>();

                for (int i = 0; i < sequence.Sequence.Count; i++)
                {
                    _ClipTypesSequence.Add(sequence.Sequence[i]);

                    if (!_ReachableClipTypes.ContainsKey(sequence.Sequence[i]))
                    {
                        _ReachableClipTypes[sequence.Sequence[i]] = new List<cfg.gesture.Type>();
                    }

                    if (i == sequence.Sequence.Count - 1)
                    {
                        // link to head
                        _ReachableClipTypes[sequence.Sequence[i]].Add(sequence.Sequence[0]);
                        continue;
                    }

                    _ReachableClipTypes[sequence.Sequence[i]].Add(sequence.Sequence[i+1]);
                }

                _Init = true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public override List<string> GetReachableClips(string clipName)
        {
            return null;
        }

        public override List<cfg.gesture.Type> GetReachableClipTypes(cfg.gesture.Type clipType)
        {
            if (!_Init || clipType == cfg.gesture.Type.INVALID)
            {
                return null;
            }

            if (!_ReachableClipTypes.ContainsKey(clipType))
            {
                return new List<cfg.gesture.Type>();
            }

            return _ReachableClipTypes[clipType];
        }

        public override MetronomicSequenceUnit GetNextClipType(int clipSequenceIndex)
        {
            var nextIndex = (clipSequenceIndex + 1) % _ClipTypesSequence.Count;
            return new MetronomicSequenceUnit(_ClipTypesSequence[nextIndex], nextIndex);
        }
    }
}
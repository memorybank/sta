using Playa.Animations;
using System.Collections.Generic;
using UnityEngine;

using Playa.Config;
using Playa.Common.Utils;
using cfg.keywords;

namespace Playa.Avatars
{
    public class FacialKeywordMatcher : Matcher<Animations.AnimationRepository>
    {
        // comment behavior、entry保持独立
        public class MatchEntry
        {
            public int ClipId;
            public int RuleId;
            public int OptionId;
            public int Phase;
            public int MaxPhase;
            public double Timestamp;
            public List<FacialExpressionType> FacialExpression;
            public List<int> FacialExpressionAmplitude;
        }

        private const double MaxWaitTime = 3.0 * 1000f;

        private Dictionary<string, List<MatchEntry>> _PoseSets;
        private bool _IsInit = false;

        private MatchEntry _OngoingMatchEntry;

        [SerializeField] private ConfigsLoader _ConfigLoader;

        private Dictionary<string, string> _RegexFragments;

        private System.Random rnd = new System.Random();

        // todo Matcher抽象更新，构造函数不一定需要repository
        public FacialKeywordMatcher(Animations.AnimationRepository animationRepository) : base(animationRepository)
        {
            // use no repository
        }

        public override MatchResult match(AvatarBehavior behavior)
        {
            if (!_IsInit)
            {
                Init();
            }

            if (_OngoingMatchEntry.Phase > 0 && TimeUtils.GetMSTimestamp() - _OngoingMatchEntry.Timestamp > MaxWaitTime)
            {
                Debug.Log("Stroke keyword wait expired");
                ResetState();
            }

            if (_PoseSets.TryGetValue(((FacialExpressionBehavior)behavior.FacialBehavior).Keyword, out List<MatchEntry> value))
            {
                foreach (var entry in value)
                {
                    if ((_OngoingMatchEntry.Phase == 0) || (_OngoingMatchEntry.Phase == entry.Phase &&
                        _OngoingMatchEntry.RuleId == entry.RuleId && _OngoingMatchEntry.OptionId == entry.OptionId))
                    {
                        if (_OngoingMatchEntry.Phase == 0)
                        {
                            _OngoingMatchEntry = entry;
                        }
                        _OngoingMatchEntry.Phase++;
                        _OngoingMatchEntry.Timestamp = TimeUtils.GetMSTimestamp();
                        if (_OngoingMatchEntry.Phase == _OngoingMatchEntry.MaxPhase)
                        {
                            ResetState();
                        }
                        MatchResult m = new MatchResult();
                        m.FacialExpression = entry.FacialExpression;
                        m.FacialExpressionAmplitude = entry.FacialExpressionAmplitude;
                        return m;
                    }
                }
            }
            return new MatchResult();
        }

        public override void ResetState()
        {
            _OngoingMatchEntry.Phase = 0;
            _OngoingMatchEntry.MaxPhase = 1;
        }

        private void Init()
        {
            _PoseSets = new Dictionary<string, List<MatchEntry>>();
            _RegexFragments = new Dictionary<string, string>();
            _ConfigLoader = ConfigsLoader.Instance;
            _OngoingMatchEntry = new MatchEntry();
            _OngoingMatchEntry.Phase = 0;

            foreach (var data in _ConfigLoader.Tables.TbKeywordRegex.DataList)
            {
                _RegexFragments[data.Name] = data.Pattern;
            }

            for (int rule = 0; rule < _ConfigLoader.Tables.TbKeywordRules.DataList.Count; rule++)
            {
                var data = _ConfigLoader.Tables.TbKeywordRules.DataList[rule];
                for (int option = 0; option < data.Rule.Count; option++)
                {
                    var phases = data.Rule[option].Split(',');
                    if (phases.Length != data.Anim.Count)
                    {
                        Debug.LogWarning("Rule option doesn't match total anims");
                    }

                    if (phases.Length == data.Anim.Count)
                    {
                        for (int phase = 0; phase < phases.Length; phase++)
                        {
                            var entry = new MatchEntry();
                            entry.ClipId = data.Anim[phase];
                            entry.RuleId = rule;
                            entry.OptionId = option;
                            entry.Phase = phase;
                            entry.MaxPhase = data.Anim.Count;
                            entry.FacialExpression = data.FacialExpression;
                            entry.FacialExpressionAmplitude = data.FacialExpressionAmplitude;
                            ExpandPattern("", phases[phase], 0, entry);
                        }
                    }
                }
            }

            _IsInit = true;

            Debug.Log("Keyword matcher init success");
        }

        private void ExpandPattern(string expand, string pattern, int cursor, MatchEntry entry)
        {
            if (cursor >= pattern.Length)
            {
                if (!_PoseSets.ContainsKey(expand))
                {
                    _PoseSets[expand] = new List<MatchEntry>();
                }
                _PoseSets[expand].Add(entry);
                return;
            }

            if (pattern[cursor] == '(')
            {
                // Skip '('
                cursor++;
                string varName = "";
                while (cursor < pattern.Length && pattern[cursor] != ')')
                {
                    varName += pattern[cursor++];
                }
                // Skip ')'
                cursor++;
                if (!_RegexFragments.ContainsKey(varName))
                {
                    Debug.LogError("FacialKeywordMatcher : Regex fragment doesn't exist " + varName);
                    //Debug.Assert(_RegexFragments.ContainsKey(varName), "FacialKeywordMatcher : Regex fragment doesn't exist " + varName);
                }
                else
                {
                    foreach (var option in _RegexFragments[varName].Split('|'))
                    {
                        ExpandPattern(expand + option, pattern, cursor, entry);
                    }
                }
                
            }
            else
            {
                ExpandPattern(expand + pattern[cursor], pattern, cursor + 1, entry);
            }
        }
    }
}

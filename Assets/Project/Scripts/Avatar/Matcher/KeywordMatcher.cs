using Playa.Animations;
using System.Collections.Generic;
using UnityEngine;

using Playa.Config;
using Playa.Common.Utils;

namespace Playa.Avatars
{
    public class KeywordMatcher : Matcher<Animations.AnimationRepository>
    {
        public class MatchEntry
        {
            public int ClipId;
            public int RuleId;
            public int OptionId;
            public int Phase;
            public int MaxPhase;
            public double Timestamp;
        }

        private const double MaxWaitTime = 3.0 * 1000f;

        private Dictionary<string, List<MatchEntry>> _PoseSets;
        private bool _IsInit = false;

        private MatchEntry _OngoingMatchEntry;

        [SerializeField] private ConfigsLoader _ConfigLoader;

        private Dictionary<string, string> _RegexFragments;

        private System.Random rnd = new System.Random();

        public KeywordMatcher(Animations.AnimationRepository animationRepository) : base(animationRepository)
        {
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
                // Force reset
                _OngoingMatchEntry.Phase = 0;
                _OngoingMatchEntry.MaxPhase = 1;
            }

            // Debug.Log("Keyword matcher try matching " + ((StrokeGestureBehavior)behavior.GestureBehavior).Keyword);
            if (_PoseSets.TryGetValue(((StrokeGestureBehavior)behavior.GestureBehavior).Keyword, out List<MatchEntry> value))
            {
                foreach (var entry in value)
                {
                    if ((_OngoingMatchEntry.Phase == 0 && _OngoingMatchEntry.Phase == entry.Phase) || 
                        (_OngoingMatchEntry.Phase == entry.Phase && _OngoingMatchEntry.RuleId == entry.RuleId && _OngoingMatchEntry.OptionId == entry.OptionId))
                    {
                        if (_OngoingMatchEntry.Phase == 0)
                        {
                            _OngoingMatchEntry.RuleId = entry.RuleId;
                            _OngoingMatchEntry.OptionId = entry.OptionId;
                            _OngoingMatchEntry.MaxPhase = entry.MaxPhase;
                        }
                        _OngoingMatchEntry.Phase++;
                        _OngoingMatchEntry.Timestamp = TimeUtils.GetMSTimestamp();
                        ResetState();
                        MatchResult m = new MatchResult();
                        m.AnimationClipIndex = DataSource.GetClipIndexById(entry.ClipId);
                        Debug.Log("Keyword matcher matched " + ((StrokeGestureBehavior)behavior.GestureBehavior).Keyword + ", " + entry.ClipId + ", " + m.AnimationClipIndex);
                        return m;
                    }
                }
            }
            return new MatchResult();
        }

        public override void ResetState()
        {
            if (_OngoingMatchEntry == null)
            {
                return;
            }

            // Reset only if the stroke sequence finished
            if (_OngoingMatchEntry.Phase >= _OngoingMatchEntry.MaxPhase)
            {
                Debug.Log("Reset triggered");
                _OngoingMatchEntry.Phase = 0;
                _OngoingMatchEntry.MaxPhase = 1;
            }
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
                // Debug.Log(string.Format("Keyword matcher add {0}, {1}", entry.ClipId, expand));
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
                    Debug.LogError("KeyWordMatcher: Regex fragment doesn't exist " + varName);
                    //Debug.Assert(_RegexFragments.ContainsKey(varName), "KeyWordMatcher : Regex fragment doesn't exist " + varName);
                }
                else
                {
                    foreach (var option in _RegexFragments[varName].Split('|'))
                    {
                        // Prevent manual error
                        if (option != "")
                        {
                            ExpandPattern(expand + option, pattern, cursor, entry);
                        }
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

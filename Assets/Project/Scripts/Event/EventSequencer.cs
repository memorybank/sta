using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Priority_Queue;

using Playa.Common;
using Playa.Common.Utils;

using Playa.Audio.Event;
using Playa.NLP.Event;
using TMPro;
using Playa.Avatars;

namespace Playa.Event
{
    public class EventSequencer : MonoBehaviour
    {
        [SerializeField] private string _Name;
        [SerializeField] private SimplePriorityQueue<BaseUnit> _Events;

        // Parent pointer
        [SerializeField] private AvatarBrain _AvatarBrain;
        public AvatarBrain AvatarBrain => _AvatarBrain;

        // TODO: make event list generic
        public VoiceActivityEvent voiceActivityEvent;
        public IdeationalUnitEvent ideationalUnitEvent;
        public SyntaxRootEvent syntaxRootEvent;
        public KeywordEvent keywordEvent;
        public SentimentEvent sentimentEvent;

        private double _RecordedTimestamp;

        private bool _Running = false;

        // UI
        public TextMeshProUGUI _EventTrigger;

        private void Awake()
        {
            _Events = new SimplePriorityQueue<BaseUnit>();
        }

        public void Push(BaseUnit baseUnit)
        {
            lock (_Events)
            {
                _Events.Enqueue(baseUnit, baseUnit.DetectedTimestamp);
            }
        }
        public void StartSequence()
        {
            _RecordedTimestamp = TimeUtils.GetMSTimestamp();
            _Running = true;
        }

        public void StopSequence()
        {
            _Running = false;
        }

        private void Update()
        {
            if (!_Running)
            {
                return;
            }

            lock (_Events)
            {
                _Events.TryFirst(out BaseUnit first);
                if (first != null)
                {
                    var type = first.GetType();
                    string text = "";
                    if (type == typeof(VoiceActivityUnit))
                    {
                        text = string.Format("Avatar {0} Voice activity event {1} triggered at {2}", _Name, (first as VoiceActivityUnit).ActivityType, first.DetectedTimestamp);
                        Debug.Log(text);
                        voiceActivityEvent?.Invoke(first as VoiceActivityUnit, AvatarBrain.AvatarUser.AvatarUUID);
                    }
                    if (type == typeof(IdeationalUnit))
                    {
                        text = string.Format("Avatar {0} IDU event {1} triggered at {2}", _Name, (first as IdeationalUnit).Phrases.Count, first.DetectedTimestamp);
                        Debug.Log(text);
                        ideationalUnitEvent?.Invoke(first as IdeationalUnit);
                    }
                    if (type == typeof(PunctuationUnit))
                    {
                        text = string.Format("Avatar {0} Punctuation event {1} triggered at {2}", _Name, (first as PunctuationUnit).IsSentenceFinished, first.DetectedTimestamp);
                        Debug.Log(text);
                        syntaxRootEvent?.Invoke(first as PunctuationUnit);
                    }
                    if (type == typeof(KeywordUnit))
                    {
                        text = string.Format("Avatar {0} Keyword event {1} triggered at {2}", _Name, (first as KeywordUnit).keyword, first.DetectedTimestamp);
                        Debug.Log(text);
                        keywordEvent?.Invoke(first as KeywordUnit); 
                    }
                    if (type == typeof(SentimentUnit))
                    {
                        text = string.Format("Avatar {0} Sentiment event {1} triggered at {2}", _Name, (first as SentimentUnit).Score, first.DetectedTimestamp);
                        Debug.Log(text);
                        sentimentEvent?.Invoke(first as SentimentUnit);
                    }

                    _EventTrigger.text = "Event trigger " + text;
                    _Events.Dequeue();
                }
            }
        }
    }
}
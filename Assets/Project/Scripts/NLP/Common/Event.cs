using Playa.Common;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Playa.NLP.Event
{
    [Serializable]
    public class CommaPunctuationEvent : UnityEvent<PunctuationUnit>
    {
        //comment ����
    }

    [Serializable]
    public class SyntaxRootEvent : UnityEvent<PunctuationUnit>
    {
        //comment NLPͣ��, current:����
    }

    [Serializable]
    public class KeywordEvent : UnityEvent<KeywordUnit>
    {
    }

    [Serializable]
    public class WordInfo
    {
        public string Word;
        public float StartOffsetInSecond;
        public float EndOffsetInSecond;
    }

    [Serializable]
    public class SpeechStreamingRecognitionResult
    {
        public bool IsFinal;
        public float StartOffsetInSecond;
        public string Transcript;
        public List<WordInfo> WordInfos;
    }
    public class VoiceLoudnessEvent : UnityEvent<VoiceLoudnessUnit>
    {
    }

    [Serializable]
    public class SentimentEvent : UnityEvent<SentimentUnit>
    {
    }
}
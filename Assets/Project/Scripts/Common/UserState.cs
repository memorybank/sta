using System;
using System.Collections.Generic;

namespace Playa.Common
{
    [Serializable]
    public class BaseUnit
    {
        public float DetectedTimestamp;

        public BaseUnit(float detectedTimestamp)
        {
            DetectedTimestamp = detectedTimestamp;
        }
    }

    [Serializable]
    public class Phrase
    {
        public string Text;
        public float Duration;

        public Phrase(string text, float duration)
        {
            Text = text;
            Duration = duration;
        }
    }

    [Serializable]
    public class IdeationalUnit : BaseUnit
    {
        public List<Phrase> Phrases;

        public IdeationalUnit() : base(0.0f)
        {
        }
        public IdeationalUnit(float detectedTimestamp) : base(detectedTimestamp)
        {
        }
    }

    [Serializable]
    public class PunctuationUnit : BaseUnit
    {
        public bool IsSentenceFinished;

        public PunctuationUnit(bool finished) : base(0.0f)
        {
            IsSentenceFinished = finished;
        }

        public PunctuationUnit(bool finished, float detectedTimestamp) : base(detectedTimestamp)
        {
            IsSentenceFinished = finished;
        }
    }

    [Serializable]
    public class KeywordUnit : BaseUnit
    {
        public string keyword;

        public KeywordUnit(string value) : base(0.0f)
        {
            keyword = value;
        }
        public KeywordUnit(string value, float detectedTimestamp) : base(detectedTimestamp)
        {
            keyword = value;
        }
    }

    [Serializable]
    public class SentimentUnit : BaseUnit
    {
        public float Score;

        public float Magnitude;

        public EmotionStatusType EmotionStatusType;

        public SentimentUnit(float score, float magnitude) : base(0.0f)
        {
            Score = score;
            Magnitude = magnitude;
        }
        public SentimentUnit(float score, float magnitude, float detectedTimestamp) : base(detectedTimestamp)
        {
            Score = score;
            Magnitude = magnitude;
        }
    }

    public enum VoiceActivityType
    {
        Invalid = 0,
        Inactive = 1,
        Punctuated = 2,
        Active = 3,
        Silence = 4,
    }

    public enum AvatarActivityType
    {
        Invalid = 0,
        IsIdle = 1,
        IsTalking = 2,
        IsPausing = 3,
    }

    [Serializable]
    public class VoiceActivityUnit : BaseUnit
    {
        public VoiceActivityType ActivityType;
        public VoiceActivityUnit(VoiceActivityType activeType) : base(0.0f)
        {
            ActivityType = activeType;
        }

        public VoiceActivityUnit(VoiceActivityType activeType, float detectedTimestamp) : base(detectedTimestamp)
        {
            ActivityType = activeType;
        }
    }

    [Serializable]
    public class VoiceLoudnessUnit : BaseUnit
    {
        public float Loudness;

        public VoiceLoudnessUnit(float loudiness) : base(0.0f)
        {
            Loudness = loudiness;
        }

        public VoiceLoudnessUnit(float loudiness, float detectedTimestamp) : base(detectedTimestamp)
        {
            Loudness = loudiness;
        }
    }


    //todo
    //freq band list
    //amplifier

    public enum UserTextGrammarType
    {
        Invalid = 0,
        ZhuWeiBing = 1,  // ÷˜”ÔŒΩ”Ô±ˆ”Ô
        XiuShiYu = 2,     // –ﬁ Œ”Ô
        DeDeDi= 3,      // –ﬁ Œ”Ô¡¨Ω”¥ £¨ µƒ µ√ µÿ
        LianCi = 4,    // ¡¨¥ 
        ChaRuYu = 5,     // ∏–Ãæ¥ £®≤Â»Î”Ô£©
    }

    public enum EmotionStatusType
    {
        Neutral = 0,
        Positive = 1,
        Negative = 2,
        Mixed = 3,
    }

    [Serializable]
    public class EmotionStatus
    {
        public EmotionStatusType Type;
        public float Intensity;

        public EmotionStatus(EmotionStatusType e_type)
        {
            Type = e_type;
        }
    }
}
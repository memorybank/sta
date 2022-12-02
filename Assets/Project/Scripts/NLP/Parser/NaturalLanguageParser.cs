using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Google.Cloud.Language.V1;

using Playa.NLP.Event;
using Playa.Event;

namespace Playa.NLP.Parser
{
    public class ParseRequest
    {
        public StringInfo Info;
        public float StartTimestamp;

        public ParseRequest(StringInfo info, float startTimestamp)
        {
            Info = info;
            StartTimestamp = startTimestamp;
        }
    }

    public abstract class NaturalLanguageParser : MonoBehaviour
    {
        public bool IsInit;

        public SyntaxRootEvent SyntaxRootEvent;

        public CommaPunctuationEvent commaPunctuationEvent;

        public KeywordEvent keywordEvent;

        public SentimentEvent sentimentEvent;

        public EventSequencer EventSequencer;

        protected Playa.Common.Utils.Timer _Timer; 

        abstract public void Init();

        abstract public void HandleInterimText(ParseRequest request);

        abstract public void HandleFinalText(ParseRequest request);

        public void StartTimer()
        {
            _Timer = new();
            _Timer.StartTimer();
        }

    }

    public abstract class NLPResultParser : MonoBehaviour
    {
        protected LanguageServiceClient client;

        public bool Active = true; // user parameter

        public bool IsInit;

        public SyntaxRootEvent ParserSyntaxRootEvent;

        public CommaPunctuationEvent parserCommaPunctuationEvent;

        public KeywordEvent parserKeywordEvent;

        public SentimentEvent parserSentimentEvent;
        public void PassClient(LanguageServiceClient c)
        {
            client = c;
            IsInit = true;
        }

        public void EscapeClient()
        {
            client = null;
            IsInit = false;
        }

        abstract public void Parse(ParseRequest request);

        abstract public void ParseFinal(ParseRequest request);
    }
}

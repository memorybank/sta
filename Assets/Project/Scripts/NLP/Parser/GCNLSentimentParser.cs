using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;

using UnityEngine;
using Playa.NLP.Event;
using Google.Cloud.Language.V1;

using Playa.Common;
using Playa.Common.Utils;

namespace Playa.NLP.Parser
{
    public class GCNLSentimentParser : NLPResultParser
    {
        public class SentimentRequest
        {
            public string NewPart;
            public float StartTimestamp;

            public SentimentRequest(string newPart, float startTimestamp)
            {
                NewPart = newPart;
                StartTimestamp = startTimestamp;
            }
        }

        private ConcurrentQueue<SentimentRequest> _NLParserQueue;          //Queue for text, unrealtime handler

        private bool _IsRunning;

        private SentimentUnit sentimentUnit;
        private bool _IsSentimentUnitNew;

        void Start()
        {
            _NLParserQueue = new ConcurrentQueue<SentimentRequest>();
            Task.Run(() => TimerCoroutine());
        }

        void Update()
        {
            if (_IsSentimentUnitNew)
            {
                parserSentimentEvent?.Invoke(sentimentUnit);
                _IsSentimentUnitNew = false;
            }
        }

        private void OnDestroy()
        {
            _NLParserQueue.Clear();
            _IsRunning = false;
        }

        //async IEnumerator TimerCoroutine()
        public async Task TimerCoroutine()
        {
            _IsRunning = true;
            while(_IsRunning)
            {
                await Task.Delay((int)(ThreadsConstants.GCNLParserCoroutineIntervalTime*1000));
                if (IsInit)
                {
                    await NLRequestDetectSentiment();
                }
            }
        }

        public async Task NLRequestDetectSentiment()
        {
            string content = "";
            SentimentRequest request;
            float requestTimestamp = 0;
            while (_NLParserQueue.TryDequeue(out request))
            {
                content = request.NewPart;
                requestTimestamp = request.StartTimestamp;
            }

            if (content != "")
            {
                SentimentUnit s = HasSentimentDetectedWithTimeout(content, requestTimestamp, ThreadsConstants.GCNLParserRequestTimeout);
                if (s != null)
                {
                    sentimentUnit = s;
                    _IsSentimentUnitNew = true;
                }
            }
        }

        private SentimentUnit MustHasSentimentDetected(string content, float requestTimestamp)
        {
            Document document = Document.FromPlainText(content);
            AnalyzeSentimentResponse response = client.AnalyzeSentiment(document);
            if (response.DocumentSentiment == null)
            {
                Debug.LogWarning("sentiment analysis fail no result!");
                return null;
            }
            
            SentimentUnit sentiment = new SentimentUnit(
                response.DocumentSentiment.Score, response.DocumentSentiment.Magnitude, requestTimestamp);
            if (sentiment.Score == 0 && sentiment.Magnitude == 0)
            {
                return null;
            }

            // todo rule insert for determine EmotionStatusType
            if (sentiment.Magnitude > 0.5)
            {
                sentiment.EmotionStatusType = EmotionStatusType.Positive;
            }
            else if (sentiment.Magnitude < 0.5)
            {
                sentiment.EmotionStatusType = EmotionStatusType.Negative;
            }
            else
            {
                sentiment.EmotionStatusType = EmotionStatusType.Neutral;
            }

            return sentiment;
        }

        private SentimentUnit HasSentimentDetectedWithTimeout(string content, float requestTimestamp, double timeout)
        {
            double tstart = TimeUtils.GetMSTimestamp();

            SentimentUnit s = MustHasSentimentDetected(content, requestTimestamp);
            if (s != null)
            {
                double t = TimeUtils.GetMSTimestamp() - tstart;
                if (t < timeout)
                {
                    return s;
                }
            }

            return null;
        }

        public override void Parse(ParseRequest request)
        {
            if (request.Info.LengthInTextElements < 4)
            {
                //Ì«¶Ì
                return;
            }

            //parse verb punctuation
            if (IsInit)
            {
                _NLParserQueue.Enqueue(new SentimentRequest(request.Info.String, request.StartTimestamp));
            }
        }

        public override void ParseFinal(ParseRequest request)
        {
            // Do nothing
        }
    }
}
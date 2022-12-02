using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.Common;

namespace Playa.NLP
{
    public class IdeationalUnitBasicPairGenerator : IdeationalUnitGenerator
    {
        public override void Init()
        {
            // do nothing
        }

        public override IdeationalUnit GenerateUnitWithTextAndDuration(string[] sent, float[] duration)
        {
            IdeationalUnit ideationalUnit = new IdeationalUnit();
            ideationalUnit.Phrases = new List<Phrase>();

            for (int i = 0; i < sent.Length; i += 2)
            {
                if (i == sent.Length - 1)
                {
                    ideationalUnit.Phrases.Add(new Phrase(sent[i], duration[i]));
                    break;
                }
                ideationalUnit.Phrases.Add(new Phrase(sent[i] + sent[i + 1], duration[i] + duration[i + 1]));
            }

            return ideationalUnit;
        }

        public override IdeationalUnit GenerateUnitWithTextAndDuration(string sent2, float duration2)
        {
            return GenerateUnitWithTextAndDuration(new string[1] { sent2 }, new float[1] { duration2 });
        }
    }
}

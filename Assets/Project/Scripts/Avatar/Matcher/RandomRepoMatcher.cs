using Playa.Common;
using UnityEngine;

namespace Playa.Avatars
{

    public class RandomRepoMatcher : Matcher<Animations.AnimationRepository>
    {
        private System.Random rnd = new System.Random();

        public RandomRepoMatcher(Animations.AnimationRepository animationRepository) : base(animationRepository)
        {
        }

        public override MatchResult match(AvatarBehavior behavior)
        {
            MatchResult m = new MatchResult();
            m.AnimationClipIndex = rnd.Next() % DataSource.AnimationClipInfos.Count;
            return m;
        }


    }
}
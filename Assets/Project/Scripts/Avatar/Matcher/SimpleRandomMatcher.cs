using UnityEngine;
using Playa.Common;

namespace Playa.Avatars
{

    public class SimpleRandomMatcher : Matcher<Animations.AnimationRepository>
    {
        private System.Random rnd = new System.Random();

        public SimpleRandomMatcher(Animations.AnimationRepository animationRepository) : base(animationRepository)
        {
        }

        void Start()
        {
            Debug.Log("ct " + DataSource.AnimationClips.Count.ToString());
            Debug.Log(DataSource.ToString());
        }

        public override MatchResult match(AvatarBehavior behavior)
        {
            MatchResult m = new MatchResult();
            m.AnimationClipIndex = rnd.Next() % DataSource.AnimationClips.Count;
            return m;
        }
    }
}
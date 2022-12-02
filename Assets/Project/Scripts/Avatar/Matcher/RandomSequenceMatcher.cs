using Playa.Common;

namespace Playa.Avatars
{

    public class RandomSequenceMatcher : Matcher<Animations.AnimationSequenceGenerator>
    {
        private System.Random rnd = new System.Random();

        public RandomSequenceMatcher(Animations.AnimationSequenceGenerator animationRepository) : base(animationRepository)
        {
        }

        public override MatchResult match(AvatarBehavior behavior)
        {
            MatchResult m = new MatchResult();
            m.AnimationClipIndex = rnd.Next() % DataSource.TotalNetwork;
            return m;
        }
    }
}
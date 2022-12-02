using Playa.Common;

namespace Playa.Avatars
{

    public class SimpleMatcher : Matcher<Animations.AnimationRepository>
    {

        public SimpleMatcher(Animations.AnimationRepository animationRepository) : base(animationRepository)
        {
        }

        public override MatchResult match(AvatarBehavior behavior)
        {

            MatchResult m = new MatchResult();
            m.AnimationClipIndex = 0;
            return m;
        }
    }
}
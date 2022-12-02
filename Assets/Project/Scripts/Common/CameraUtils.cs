using UnityEngine;

namespace Playa.Common
{
    public static class UserStateTransitionConstants
    {
        public static float CommonTransitionFadeNormalizedDuration = 0.25f; // Used on States OnEnable
        public static float ActionLayerTransitionFadeDuration = 0.2f; // gesture layer.OnEnd
        public static float InteractionLayerTransitionFadeDuration = 0.2f; // gesture layer.OnEnd

        public static float IdleStateFastFadeDuration = 0.02f;
        public static float IdleStateSlowFadeDuration = 0.1f;
        public static float SilenceStateFastFadeDuration = 0.02f;
        public static float InteractionFastFadeDuration = 0.02f;

        public static float ActionLayerCommonTransitionFadeDuration = 0.4f;
        public static float ActionLayerFinishTalkingFadeDuration = 1.0f;
    }
}
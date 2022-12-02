using Recognissimo.Core;
using UnityEngine;
using Playa.Common;
using Animancer.FSM;
using UnityEngine.UI;
using TMPro;

namespace Playa.Avatars
{
    public sealed class InteractAnimBrain : AvatarBrain
    {
        float residual = 0.0f;

        // ui component
        [SerializeField] private TMP_InputField _keywordInput;

        private void Start()
        {
            AvatarUser.GetStateFunction(StateActionType.BackToIDUMonotronic)?.Invoke();
            _keywordInput.text = "";

            _keywordInput.onValueChanged.AddListener(OnKeywordInputChanged);
        }
        
        protected void OnKeywordInputChanged(string value)
        {
            Behavior.GestureBehavior = new StrokeGestureBehavior();
            ((StrokeGestureBehavior)Behavior.GestureBehavior).Keyword = value;
            ((AvatarActionState)AvatarUser.GetAvatarState(AvatarStateType.IDUStroke)).TryReEnterState();

            //FacialBehaviorPlanner.MatchKeyword(value);
        }

    }

}
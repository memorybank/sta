using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Playa.UI
{
   public class AnimancerDemo1 : MonoBehaviour
    {
        [SerializeField] private Avatars.AvatarAnimator _Avatar;

        [SerializeField] private Slider slider;

        public void toggleReenter()
        {
            _Avatar.AnimationConfig.Reenter = !_Avatar.AnimationConfig.Reenter;
        }


        // Start is called before the first frame update
        void Start()
        {
        }

        void OnEnable()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }




    }
}



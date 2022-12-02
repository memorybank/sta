using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playa;
using Playa.Common;
using Playa.App;

namespace Playa.Avatars
{
    using ItemID = UInt64;
    public class HeadIKOption : OptionClass
    {
        public ItemID item_ID;
        public Transform headIKObj;
        public float headWeight;
        public float bodyWeight;
        public AvatarUser user;

        private Transform originalHeadIKObj;
        private float originalHeadWeight;
        private GameObject calcuTarget;
        private Transform selfHead;

        public HeadIKOption(ItemID item_ID, AvatarUser user, int priority, Transform headIKObj = null, float headWeight = 1, float bodyWeight = 1)
        {
            this.item_ID = item_ID;
            this._Priority = priority;
            this.user = user;
            this.type = "HeadIK";
            this.originalHeadIKObj = headIKObj;
            this.originalHeadWeight = headWeight;
            
            if (headIKObj != null && priority >= 0)
            {
                calcuTarget = new GameObject(string.Format("calcuHeadIK_{0}_{1}_{2}", user, item_ID, priority));
                calcuTarget.transform.parent = user.GetAvatarPosition();
                selfHead = ArmatureUtils.FindHead(user.ActiveAvatarTransform);
                this.headIKObj = calcuTarget.transform;
                if (headWeight != 0)
                {
                    this.headWeight = 1;
                    this.bodyWeight = bodyWeight / headWeight;
                }
                else
                {
                    this.headWeight = 0;
                    this.bodyWeight = 0;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("HeadIKProperty({0},{1},{2},{3},{4},{5})", item_ID, user, _Priority, headIKObj, headWeight, bodyWeight);
        }

        public override bool Compare(OptionClass obj)
        {
            Debug.Assert(obj is HeadIKOption, string.Format("Option Manager : try to compare {0} with {1}", this, obj));
            HeadIKOption hipObj = (HeadIKOption)obj;
            bool result = true;
            if ((this.user != hipObj.user)
                || (this.item_ID != hipObj.item_ID)
                ) result = false;
            Debug.Log(string.Format("Option Compare : {0} {1}equals to {2}", ToString(), (result ? "" : "not "), hipObj.ToString()));
            return result;
        }

        public override void OnUpdate()
        {
            /*
             计算：以自己的头为原点，以人物正向为z轴正向，建立坐标系；
             以实际目标点为向量v1，以正向为向量v2，叉乘取出两个向量所在平面的法线vn；
             以vn为轴对v1进行旋转，旋转角为（1-权重）*（v1与v2的平面夹角）；
             结果：v1将向v2旋转一定角度，使得最终朝向对人物正向有相当于（权重*实际目标的旋转角）的旋转。
             以结果设定计算目标点的坐标。
             */
            Vector3 direction = originalHeadIKObj.position - selfHead.position;
            Vector3 localForward = user.GetAvatarPosition().forward;
            float angle = (1 - originalHeadWeight) * Vector3.Angle(direction, localForward);
            Vector3 normalVec = Vector3.Cross(direction, localForward);
            calcuTarget.transform.position = Quaternion.AngleAxis(angle, normalVec) * direction + selfHead.position;
            Debug.Log(string.Format("HeadIK calcuTarget : {0} from {1}", headIKObj.transform.position, originalHeadIKObj));
        }

        public override void OnRemove()
        {
            if (headIKObj != null)
            {
                GameUtils.RenameDestroy(headIKObj.gameObject);
            }
        }
    }

    public class HeadIKManager
    {
        private Dictionary<VoiceActivityType, OptionsStore> headIKLists;
        public HeadIKManager()
        {
            headIKLists = new Dictionary<VoiceActivityType, OptionsStore>();
            foreach (VoiceActivityType vat in Enum.GetValues(typeof(VoiceActivityType)))
            {
                headIKLists.Add(vat, new OptionsStore(new List<OptionClass>(), vat + "HeadIK"));
            }
        }

        public void AddHeadIK(VoiceActivityType voiceActivityType, HeadIKOption headIKProperty)
        {
            headIKLists[voiceActivityType].AddOption(headIKProperty, false, true);}
        public void RemoveHeadIK(VoiceActivityType voiceActivityType, HeadIKOption headIKProperty)
        {
            headIKLists[voiceActivityType].RemoveOption(headIKProperty, false, false);
        }
        public void ExcuteHeadIK(VoiceActivityType voiceActivityType, bool randomIFsameLevel = false)
        {
            voiceActivityType = Rule(voiceActivityType);
            Dictionary<IKEffectorName, IKTarget> targets = new Dictionary<IKEffectorName, IKTarget>();
            HeadIKOption headIKProperty = (HeadIKOption)headIKLists[voiceActivityType].GetOption(randomIFsameLevel);
            headIKProperty.OnUpdate();
            IKTarget iKTarget = new IKTarget(headIKProperty.headIKObj, headIKProperty.headWeight, headIKProperty.bodyWeight);
            targets.Add(IKEffectorName.LookAt, iKTarget);
            headIKProperty.user.MultiIKManager.SetIKTargets(targets);
        }

        public void UpdateHeadIKOptions()
        {
            foreach (VoiceActivityType vat in Enum.GetValues(typeof(VoiceActivityType)))
            {
                HeadIKOption headIKOption =  (HeadIKOption)headIKLists[vat].GetOption();
                if (headIKOption != null)
                {
                    headIKOption.OnUpdate();
                }
            }
        }

        private VoiceActivityType Rule(VoiceActivityType voiceActivityType)
        {
            if (voiceActivityType == VoiceActivityType.Punctuated)
            {
                voiceActivityType = VoiceActivityType.Active;
            }else if (voiceActivityType == VoiceActivityType.Invalid)
            {
                Debug.LogError("HeadIKManager: VoiceActivityType.Invalid");
                voiceActivityType = VoiceActivityType.Inactive;
            }

            return voiceActivityType;
        }
    }
}


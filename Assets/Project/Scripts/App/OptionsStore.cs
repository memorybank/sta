using System.Collections.Generic;
using UnityEngine;

namespace Playa.App
{
    public class OptionsStore
    {
        List<OptionClass> optionList;
        string optionType;
        public OptionsStore(List<OptionClass> optionList, string type = "(unNamed)")
        {
            this.optionList = optionList;
            this.optionType = type;
        }

        public void AddOption ( OptionClass newOption, bool randomIFsameLevel = false, bool update = true)
        {
            foreach (OptionClass oldOption in optionList)
            {
                if (oldOption.Compare(newOption))
                {
                    optionList.Remove(oldOption);
                    break;
                }
            }
            optionList.Add(newOption);
            Debug.Log(string.Format("Option Manager : Add {0} in {1} list.", newOption.ToString(), optionType));
            if (update) { 
                GetOption(randomIFsameLevel).OnUpdate(); 
            }
        }

        public void RemoveOption(OptionClass oneOption, bool randomIFsameLevel = false, bool update = true)
        {
            bool isExist = false;
            foreach (OptionClass oldOption in optionList)
            {
                if (oldOption.Compare(oneOption))
                {
                    isExist = true;
                    oldOption.OnRemove();
                    optionList.Remove(oldOption);
                    break;
                }
            }
            if (!isExist)
            {
                Debug.Log(string.Format("Option Manager : try to remove {0} not exist in {1} list.", oneOption.ToString(), optionType));
            }
            else
            {
                Debug.Log(string.Format("Option Manager : Remove {0} in {1} list.", oneOption.ToString(), optionType));
                if (update)
                {
                    var option = GetOption(randomIFsameLevel);
                    if (option != null)
                    {
                        option.OnUpdate();
                    }
                }
            }
        }

        public void ResetOption( bool randomIFsameLevel = false)
        {
            GetOption(randomIFsameLevel).OnUpdate();
        }

        public OptionClass GetOption(bool randomIFsameLevel = false)
        {
            OptionClass option;
            List<OptionClass> matchList = new List<OptionClass>();
            //fetch highest priority targets;
            int highestPriority = 0;
            foreach (OptionClass oneOption in optionList)
            {
                if (oneOption._Priority > highestPriority)
                {
                    matchList = new List<OptionClass>();
                    highestPriority = oneOption._Priority;
                }
                else if (oneOption._Priority < highestPriority)
                {
                    continue;
                }
                matchList.Add(oneOption);
            }

            if (matchList.Count == 0)
            {
                Debug.Log(string.Format("Option Manager : Nothing in {0} List", optionType));
                return null;
            }

            //handle if same level + get final result
            if (randomIFsameLevel)
            {
                option = matchList[UnityEngine.Random.Range(0,matchList.Count - 1)];
            }
            else
            {
                option = matchList[matchList.Count - 1];
            }

            Debug.Log(string.Format("Option Manager : Get {0} in {1} matchList({2}), Priority = {3}", option.ToString(), optionType, matchList.Count, highestPriority));
            return option;
        }
    }
}
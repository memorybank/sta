using Playa.App;
using Playa.Client;
using Playa.Common.Utils;
using Protos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Playa.Item
{
    public class ItemFactory : MonoBehaviour
    {
        [SerializeField] private Dropdown _AppDropDown;
        [SerializeField] private TMP_Dropdown _StageItemDropdown;
        [SerializeField] private TMP_Dropdown _HandHoldItemDropdown;
        [SerializeField] private TMP_Dropdown _FullBodyItemDropdown;
        [SerializeField] private TMP_Dropdown _FollowItemDropdown;
        [SerializeField] private Toggle _IsDebug;

        private ItemManager _ItemManager;

        [SerializeField] private PitayaClientImpl pitayaClient;
        public PitayaClientImpl PitayaClient => pitayaClient;

        // Start is called before the first frame update
        void Awake()
        {
            InitStageItemDropdown();

            InitHandHoldItemDropDown();

            InitFullBodyItemDropDown();

            InitFollowItemDropDown();

            pitayaClient.Connect("192.168.1.52", 3351);
            // onItem item.ActivateByUser()
            pitayaClient.SubscribeRoute<UserUseItem>("onUseItem", (UserUseItem data) =>
            {
                Debug.Log("pitaya [connector.room.onuseitem] PUSH RESPONSE = " + data);
                ClientSwitchActivateItems(data.Name, data.Uuid);
            });
        }

        public void RequestActivate(string _ItemName)
        {
            var g = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text);
            if (g == null)
            {
                return;
            }
            var currentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text).GetComponent<BaseApp>();

            UserUseItem userUseItem = new UserUseItem();
            userUseItem.Name = _ItemName;
            TMPro.TMP_InputField AvatarIndexText = GameObject.Find("AvatarIndexText").GetComponent<TMPro.TMP_InputField>();
            int index = int.Parse(AvatarIndexText.text);
            Debug.Assert(index == 0 || index == 1, "item avatar index invalid");
            userUseItem.Uuid = currentApp._AppStartupConfig.AvatarUsers[index].AvatarUUID.ToString();
            userUseItem.Timestamp = (long)TimeUtils.GetMSTimestamp();

            if (_IsDebug.isOn)
            {
                ClientSwitchActivateItems(_ItemName, userUseItem.Uuid);
                return;
            }

            PitayaClient.Request<UserUseItem>("connector.useitem", userUseItem,
                    (UserUseItem data) => {
                        Debug.Log($"pitaya [connector.useitem] - response={data}");
                    }
                    );
        }

        public void InitAvatarUserItem()
        {
            //TODO: clear self only
            ClientSwitchActivateItems("ClearItem", "1001");
        }
        private void InitStageItemDropdown()
        {
            _StageItemDropdown.options.Clear();
            _StageItemDropdown.onValueChanged.RemoveAllListeners();
            _StageItemDropdown.options.Add(new TMP_Dropdown.OptionData(""));
            _StageItemDropdown.options.Add(new TMP_Dropdown.OptionData("ArtGallery"));
            _StageItemDropdown.options.Add(new TMP_Dropdown.OptionData("ForestHouse"));
            _StageItemDropdown.options.Add(new TMP_Dropdown.OptionData("Gameboy"));
            _StageItemDropdown.options.Add(new TMP_Dropdown.OptionData("House"));            
            _StageItemDropdown.options.Add(new TMP_Dropdown.OptionData("Kandinsky"));
            _StageItemDropdown.options.Add(new TMP_Dropdown.OptionData("Stable"));

            _StageItemDropdown.onValueChanged.AddListener(index =>
            {
                var currentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text).GetComponent<BaseApp>();

                BaseItem item;
                string _ItemName = _StageItemDropdown.options[index].text;
                RequestActivate(_ItemName);
            });
        }

        private void ClientSwitchActivateItems(string _ItemName, string uuid)
        {
            var currentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text);
            if (currentApp == null) return;
            Debug.Log("pitaya we are here client " + _ItemName + " " + uuid);
            BaseItem item;

            Assembly curAss = Assembly.GetExecutingAssembly();
            try
            {
                Type t = curAss.GetType("Playa.Item." + _ItemName);
                item = currentApp.AddComponent(t) as BaseItem;
                item._BaseApp = currentApp.GetComponent<BaseApp>();
                item.ActivateByUser(uuid);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private void InitHandHoldItemDropDown()
        {
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("ClearHandhold"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("Cat"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("Drinking"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("Fishing"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("Guitar"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("Knife"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("Like"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("Microphone"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("MobilePhone"));
            _HandHoldItemDropdown.options.Add(new TMP_Dropdown.OptionData("Smoking"));

            _HandHoldItemDropdown.onValueChanged.AddListener(index =>
            {
                var currentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text).GetComponent<BaseApp>();

                BaseItem item;
                string _ItemName = _HandHoldItemDropdown.options[index].text;
                RequestActivate(_ItemName);
            });
        }

        private void InitFullBodyItemDropDown()
        {
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("ClearFullbody"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Bed"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Biking"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Car"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Cart"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("DiscoBall"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Knee"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Movie"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Ninja_Run"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Seesaw"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Sit_Chair"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Skateboard"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Sofa"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Table_Chairs"));
            _FullBodyItemDropdown.options.Add(new TMP_Dropdown.OptionData("Walking"));

            _FullBodyItemDropdown.onValueChanged.AddListener(index =>
            {
                var currentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text).GetComponent<BaseApp>();

                BaseItem item;
                string _ItemName = _FullBodyItemDropdown.options[index].text;
                RequestActivate(_ItemName);
            });
        }

        private void InitFollowItemDropDown()
        {
            _FollowItemDropdown.options.Add(new TMP_Dropdown.OptionData("ClearFollowItem"));
            _FollowItemDropdown.options.Add(new TMP_Dropdown.OptionData("Butterfly"));

            _FollowItemDropdown.onValueChanged.AddListener(index =>
            {
                var currentApp = GameObject.Find(_AppDropDown.options[_AppDropDown.value].text).GetComponent<BaseApp>();
                BaseItem item;
                string _ItemName = _FollowItemDropdown.options[index].text;
                RequestActivate(_ItemName);
            }
                );
        }

    }
}
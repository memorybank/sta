using System.Collections.Generic;
using Animancer.FSM;
using Playa.Avatars;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Playa.UI
{
    public class CameraSceneUI : MonoBehaviour
    {
        [SerializeField] private Dropdown _CameraDropdown;

        void Awake()
        {
            InitCameraDropdown();
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void InitCameraDropdown()
        {
            List<Dropdown.OptionData> CameraNames = new List<Dropdown.OptionData>();

            for (int i = 0; i < 3; i++)
            {
                CameraNames.Add(new Dropdown.OptionData("¾µÍ· " + i.ToString()));
            }

            _CameraDropdown.options = CameraNames;

            _CameraDropdown.value = _CameraDropdown.options.FindIndex(option => option.text == "");

            _CameraDropdown.onValueChanged.AddListener(index =>
            {
                var optionText = _CameraDropdown.options[index].text;
                OnCameraChange(index);
            });
        }

        public void OnCameraChange(int changeIndex)
        {
            var camera = GameObject.Find("Main Camera");
            switch (changeIndex)
            {
                case 0:
                    camera.transform.position = new Vector3(-1f, 1.3f, -10.0f);
                    break;
                case 1:
                    camera.transform.position = new Vector3(-0.15f, 1.35f, -9f);
                    camera.transform.rotation = Quaternion.Euler(new Vector3(3.0f, 0, 0));
                    break;
                case 2:
                    camera.transform.position = new Vector3(-2.0f, 1.5f, -9f);
                    camera.transform.rotation = Quaternion.Euler(new Vector3(3.0f, 0, 0));
                    break;
                default:
                    break;
            }
        }
    }
}
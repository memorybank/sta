using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

namespace Playa.CineMachineManager
{
    public class SwitchMediumCloseUpCamera : MonoBehaviour
    {
        public CinemachineVirtualCameraBase mediumCam; //Camera
        public CinemachineVirtualCameraBase mediumCloseUpCam; //Camera
        // Start is called before the first frame update

        private void Awake()
        {
            mediumCloseUpCam.VirtualCameraGameObject.SetActive(false);
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageUp)) 
            {
                mediumCloseUpCam.VirtualCameraGameObject.SetActive(false);
                mediumCloseUpCam.VirtualCameraGameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                mediumCam.VirtualCameraGameObject.SetActive(false);
                mediumCam.VirtualCameraGameObject.SetActive(true);
            }
        }
    }
}

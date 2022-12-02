using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class SwitchPlayers : MonoBehaviour
{
    public CinemachineVirtualCameraBase mediumCloseUpCamPlayer1; //Camera
    public CinemachineVirtualCameraBase mediumCloseUpCamPlayer2; //Camera
    private void Awake()
    {
        mediumCloseUpCamPlayer2.VirtualCameraGameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mediumCloseUpCamPlayer1.VirtualCameraGameObject.SetActive(false);
            mediumCloseUpCamPlayer1.VirtualCameraGameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            mediumCloseUpCamPlayer2.VirtualCameraGameObject.SetActive(false);
            mediumCloseUpCamPlayer2.VirtualCameraGameObject.SetActive(true);
        }
    }
}

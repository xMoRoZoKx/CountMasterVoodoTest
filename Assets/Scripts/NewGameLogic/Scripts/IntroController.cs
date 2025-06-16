using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : ConnectableMonoBevaviour
{
    public WaypointFollower waypointFollower;
    public CharacterController characterController;
    public ListokView listokView;
    public Transform waterFXObj;
    private void Awake()
    {
        characterController.enabled = false;
        characterController.SetWheelForward();

        connections += waypointFollower.onPathComplete.Subscribe(() =>
        {
            characterController.enabled = true;
            characterController.SetWheelSide();
            listokView.SetTarget(characterController.transform);
            waterFXObj.SetActive(false);
        });
    }
}

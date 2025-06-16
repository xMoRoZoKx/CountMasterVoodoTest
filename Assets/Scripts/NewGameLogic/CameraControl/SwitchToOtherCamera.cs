using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToOtherCamera : MonoBehaviour
{
    public CameraController cameraController;
    public int idx;

    private Transform _target;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (_target != null) cameraController.SetCameraById(idx, _target);
        else cameraController.SetCameraById(idx);
    }

}

using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : ConnectableMonoBevaviour
{
    public List<VCSettings> vcHalpCameras;
    [SerializeField] private CinemachineBrain brain;


    private void Awake()
    {
        if (brain == null)
            brain = Camera.main.GetComponent<CinemachineBrain>();

        // Пример: Установка blend типа и времени
       // brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1);
    }

    public void SetCameraById(int idx, Transform target)
    {
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, vcHalpCameras[idx].blandeTime);
        vcHalpCameras[idx].camera.Follow = target;
        vcHalpCameras[idx].camera.LookAt = target;
        ActivateCamera(vcHalpCameras[idx].camera);
    }

    public void SetCameraById(int idx)
    {
        brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, vcHalpCameras[idx].blandeTime);
        ActivateCamera(vcHalpCameras[idx].camera);
    }
    private void ActivateCamera(CinemachineVirtualCamera targetCam)
    {
        vcHalpCameras.ForEach(cam => cam.camera.Priority = 0);

        targetCam.Priority = 10;
    }
}
[System.Serializable]
public class VCSettings
{
    public CinemachineVirtualCamera camera;
    public float blandeTime;
}

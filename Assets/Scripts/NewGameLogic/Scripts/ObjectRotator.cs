using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [Header("Rotation Speed (degrees per second)")]
    public Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

    [Header("Lock Rotation Axes")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    [Header("Rotation Mode")]
    public bool useLocalRotation = true;

    void Update()
    {
        Vector3 deltaRotation = new Vector3(
            lockX ? 0f : rotationSpeed.x,
            lockY ? 0f : rotationSpeed.y,
            lockZ ? 0f : rotationSpeed.z
        ) * Time.deltaTime;

        if (useLocalRotation)
            transform.Rotate(deltaRotation, Space.Self);
        else
            transform.Rotate(deltaRotation, Space.World);
    }
}

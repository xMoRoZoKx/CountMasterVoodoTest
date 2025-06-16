using UnityEngine;

public class ListokView : MonoBehaviour
{
    public Transform target;

    private Vector3 localPosition;
    private Quaternion localRotation;
    private bool isPaused = true;

    public void SetTarget(Transform target)
    {
        isPaused = false;
        this.target = target;
        if (target != null)
        {
            // ��������� ��������� �������� � ������
            localPosition = target.InverseTransformPoint(transform.position);
           // localRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
        }
    }
    private void FixedUpdate()
    {
        Move();

    }
    private void LateUpdate()
    {
        Move();
    }
    public void Move()
    {

        if (isPaused || target == null) return;

        // ��������� ������� � ��������, ��� ���� �� ������ ��� ��������
        transform.position = target.TransformPoint(localPosition);
       // transform.rotation = target.rotation * localRotation;
    }

    /// <summary>
    /// �������� ��� ��������� ����� ��� ����� �������.
    /// </summary>
    public void SetPaused(bool pause)
    {
        isPaused = pause;
    }
}

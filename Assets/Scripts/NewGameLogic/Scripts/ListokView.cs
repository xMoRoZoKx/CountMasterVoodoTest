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
            // Вычисляем локальное смещение в старте
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

        // Обновляем позицию и вращение, как если бы объект был дочерним
        transform.position = target.TransformPoint(localPosition);
       // transform.rotation = target.rotation * localRotation;
    }

    /// <summary>
    /// Включает или выключает паузу для этого объекта.
    /// </summary>
    public void SetPaused(bool pause)
    {
        isPaused = pause;
    }
}

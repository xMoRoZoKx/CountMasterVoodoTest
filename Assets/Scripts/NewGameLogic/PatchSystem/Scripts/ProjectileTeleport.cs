using UnityEngine;

public class ProjectileTeleporter : MonoBehaviour
{
    [Header("Параметры телепорта")]
    public Transform exitPoint;         // trigger2 — точка выхода
    public Transform redirectTarget;    // цель, в сторону которой повернётся снаряд после телепорта
    public PathFollowerSystem followerSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Debug.Log("Projectile detected");
            // Телепортируем снаряд
            //other.transform.position = exitPoint.position;
            followerSystem.AddObject(other.gameObject, onComplete: () =>
            {
                // Переориентируем на цель 
                ProjectileLogic logic = other.GetComponent<ProjectileLogic>();
                if (logic != null && redirectTarget != null)
                {
                    Vector3 newDir = (redirectTarget.position - other.transform.position).normalized;
                    other.transform.rotation = Quaternion.LookRotation(newDir);
                    logic.OverrideDirection(newDir);
                }
            });
        }
    }
}
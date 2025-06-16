using TMPro;
using UnityEngine;
using DG.Tweening;

public class Bonus : MonoBehaviour
{
    [Header("Bonus Settings")]
    public int multiplier = 2;
    public GameObject particleEffect;
    public TextAnimator textAnimator;

    public void Activate(Vector3 hitDirection, Vector3 hitPosition, GameObject projectilePrefab, float speed, int damage, float range)
    {
        if (particleEffect != null)
        {
            Instantiate(particleEffect, hitPosition, Quaternion.identity);
        }

        textAnimator?.AnimateText();

        int total = multiplier;
        float offsetSpacing = 1.3f;
        Vector3 right = Vector3.Cross(Vector3.up, hitDirection).normalized;

        // Если чётное — смещаем всё влево
        Vector3 evenOffsetShift = (total % 2 == 0) ? -right * (offsetSpacing / 2f) / 2 : Vector3.zero;

        for (int i = 0; i < total; i++)
        {
            float offsetIndex = i - (total - 1) / 2f;
            Vector3 offset = right * offsetIndex * offsetSpacing + evenOffsetShift;
            Vector3 spawnPosition = hitPosition + offset;

            // Проверка на наличие другого Projectile
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.1f);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Projectile"))
                {
                    spawnPosition += hitDirection.normalized * 0.8f; // Смещение вперёд
                    break;
                }
            }

            GameObject newProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(hitDirection));
            ProjectileLogic logic = newProjectile.GetComponent<ProjectileLogic>();
            if (logic != null)
            {
                logic.Initialize(speed, hitDirection, damage, range, true);
            }
        }
    }




}

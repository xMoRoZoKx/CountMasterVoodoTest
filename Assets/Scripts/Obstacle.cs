using UnityEngine;
using TMPro;

public class Obstacle : MonoBehaviour
{
    [Header("Настройки")]
    public int maxLives = 5;
    private int currentLives;

    [Header("Ссылки")]
    public GameObject hitEffect; // визуальный эффект при попадании
    public TextMeshPro lifeText;     // ссылка на TextMeshPro объект
    public Transform vfxRoot;

    public TextAnimator textAnimator;

    private void Start()
    {
        currentLives = maxLives;
        UpdateLifeDisplay();
    }

    public void TakeHit()
    {
        currentLives--;
        textAnimator.AnimateText();
        // Визуальный эффект попадания
        if (hitEffect != null)
        {
            Destroy(Instantiate(hitEffect, vfxRoot.position, Quaternion.identity).gameObject, 5);
        }

        // Обновление текста
        UpdateLifeDisplay();

        // Уничтожение при 0 жизнях
        if (currentLives <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateLifeDisplay()
    {
        if (lifeText != null)
        {
            lifeText.text = currentLives.ToString();
        }
    }
}
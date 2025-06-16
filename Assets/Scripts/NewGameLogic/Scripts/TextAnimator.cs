using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
    public TMP_Text xText;

    [Header("Punch Animation Settings")]
    public float punchPower = 0.5f;
    public float punchDuration = 0.2f;
    public int punchVibrato = 10;
    public float punchElasticity = 1f;
    public Vector3 textRiseOffset = new Vector3(0, 0.2f, 0); // смещение при "подлёте"
    public float textRiseDuration => punchDuration / 2;

    private Vector3 initialLocalPos;
    private Tween punchTween;
    private Tween moveTween;

    private void Start()
    {
        if (xText != null)
            initialLocalPos = xText.transform.localPosition;
    }

    public void AnimateText()
    {
        if (xText == null) return;

        // Прерываем активные анимации
        punchTween?.Kill();
        moveTween?.Kill();

        // Сброс позиции и масштаба
        xText.transform.localScale = Vector3.one;
        xText.transform.localPosition = initialLocalPos;

        // Punch
        punchTween = xText.transform.DOPunchScale(
            Vector3.one * punchPower,
            punchDuration,
            punchVibrato,
            punchElasticity
        ).OnKill(() => punchTween = null);

        // Подъём вверх и возврат
        moveTween = xText.transform.DOLocalMove(initialLocalPos + textRiseOffset, textRiseDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                moveTween = xText.transform.DOLocalMove(initialLocalPos, textRiseDuration)
                    .SetEase(Ease.InCubic)
                    .OnKill(() => moveTween = null);
            });
    }
}

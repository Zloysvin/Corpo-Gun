using System.Collections;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private RectTransform EnemiesDefeatedRect;
    [SerializeField] private RectTransform TimeWarningRect;
    [SerializeField] private RectTransform DamageTakenRect;
    [SerializeField] private RectTransform EscapeRect;

    public void PlayEffect(string effectName, int effectValue = 0)
    {
        switch (effectName)
        {
            case "AllEnemiesDefeated":
                ShakeUI(EnemiesDefeatedRect, effectValue);
                break;
            case "TimeWarning":
                ShakeUI(TimeWarningRect, effectValue);
                break;
            case "DamageTaken":
                ShakeUI(DamageTakenRect, effectValue);
                break;
            case "Escape":
                ShakeUI(EscapeRect, effectValue);
                break;
            default:
                Debug.LogWarning("Effect not recognized: " + effectName);
                break;
        }
    }

    public void ShakeUI(RectTransform target, int urgency)
    {
        float duration = 0.3f + 0.1f * urgency;
        float strength = 5f * urgency;

        StartCoroutine(ShakeRoutine(target, duration, strength));
    }

    IEnumerator ShakeRoutine(RectTransform target, float duration, float strength)
    {
        Vector2 originalPos = target.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            target.anchoredPosition = originalPos + new Vector2(x, y);

            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        target.anchoredPosition = originalPos;
    }
}

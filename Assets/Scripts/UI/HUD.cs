using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Graphic brainUI;
    [SerializeField] private Graphic warningUI;
    [SerializeField] private Graphic HealthUI;
    [SerializeField] private Graphic MentalUI;
    [SerializeField] private Graphic LiveUI;
    [SerializeField] private Volume volumeProfile;
    [SerializeField] private CanvasGroup canvasGroup;

    private static HUD _instance;

    public static HUD Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(LoopFadeEffect(LiveUI, 1f));
        canvasGroup.alpha = 0f;
    }

    public void HideHUD()
    {
        StartCoroutine(FadeGroup(canvasGroup, 1f, 0f, 0.5f));
    }

    public void ShowHUD()
    {
        StartCoroutine(FadeGroup(canvasGroup, 0f, 1f, 0.5f));
    }

    public IEnumerator FadeGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        group.alpha = endAlpha;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public IEnumerator FadeElement(Graphic uiElement, float startAlpha, float endAlpha, float duration)
    {
        Color originalColor = uiElement.color;
        float time = 0f;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }

    public void PlayEffect(string effectName, int effectValue = 0)
    {
        switch (effectName)
        {
            case "OnDamage":
                StartCoroutine(ShakeUI(brainUI, effectValue, 3.0f));
                PlayEffectGroup(new List<Func<IEnumerator>>
                {
                    () => FadeElement(brainUI, 0f, 1f, 0.5f),
                    () => FadeElement(brainUI, 1f, 0f, 0.5f),
                    () => FadeElement(brainUI, 0f, 1f, 0.5f),
                    () => FadeElement(brainUI, 1f, 0f, 0.5f),
                    () => FadeElement(brainUI, 0f, 1f, 0.5f)
                });
                break;
            case "TimeWarning":
                StartCoroutine(ShakeUI(brainUI, effectValue, 3.0f));
                PlayEffectGroup(new List<Func<IEnumerator>>
                {
                    () => FadeElement(warningUI, 0f, 1f, 0.5f),
                    () => FadeElement(warningUI, 1f, 0f, 0.5f),
                    () => FadeElement(warningUI, 0f, 1f, 0.5f),
                    () => FadeElement(warningUI, 1f, 0f, 0.5f),
                    () => FadeElement(warningUI, 0f, 1f, 0.5f)
                });
                break;
            // case "HealthLoss":
            //     ShakeUI(DamageTakenRect, effectValue);
            //     break;
            // case "MentalLoss":
            //     ShakeUI(EscapeRect, effectValue);
            //     break;
            default:
                Debug.LogWarning("Effect not recognized: " + effectName);
                break;
        }
    }

    IEnumerator ShakeUI(Graphic target, int urgency, float duration)
    {
        Vector2 originalPos = target.rectTransform.anchoredPosition;
        float elapsed = 0f;
        float strength = 5f * urgency;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * strength;
            float y = UnityEngine.Random.Range(-1f, 1f) * strength;
            target.rectTransform.anchoredPosition = originalPos + new Vector2(x, y);

            yield return null;
            elapsed += Time.unscaledDeltaTime;
        }

        target.rectTransform.anchoredPosition = originalPos;
    }
    
    IEnumerator LoopFadeEffect(Graphic graphic, float duration)
    {
        while (true)
        {
            yield return FadeElement(graphic, 0f, 1f, duration);
            yield return FadeElement(graphic, 1f, 0f, duration);
        }
    }

    public void PlayEffectGroup(List<Func<IEnumerator>> effects)
    {
        StartCoroutine(PlayEffectGroupCoroutine(effects));
    }

    private IEnumerator PlayEffectGroupCoroutine(List<Func<IEnumerator>> effects)
    {
        foreach (var effect in effects)
        {
            yield return StartCoroutine(effect());
        }
    }
}

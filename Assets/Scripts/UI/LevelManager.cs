using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Controls the events and wavyness of the level

public class LevelManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup levelCanvasGroup;
    [SerializeField] private TypeWriter typeWriter;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private GameObject levelRoot;
    [SerializeField] private Camera tempCam;

    public void Start()
    {
        levelRoot.SetActive(false);
        StartCoroutine(LevelStart());
    }

    IEnumerator LevelStart()
    {
        yield return new WaitForSeconds(3f);
        typeWriter.StartTypeWriter(new List<string>
        {
            "Initializing system...",
            "Booting weapon systems...",
            "Testing suit integrity...",
            "All systems nominal...",
            "Agent " + GameManager.Instance.agentName + " is ready for deployment.",
        }, false, true, () =>
        {
            tempCam.gameObject.SetActive(false);
            levelRoot.SetActive(true);
            StartCoroutine(HUD.Instance.FadeGroup(levelCanvasGroup, 1f, 0f, fadeDuration));
            HUD.Instance.ShowHUD();
        });
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Bootscreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup bootScreenCanvasGroup;
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private List<string> dialogue;
    [SerializeField] private float delay = 0.05f;
    [SerializeField] private float timeBetweenLines = 1f;

    private bool isTyping = false;
    private int currentStringIndex = 0;
    private string currentString = "";
    private bool exitOnNextInput = false;
    private bool skipLine = false;
    private bool isDialogue = false;
    private bool dialoguePlaying = false;

    private void Update()
    {
        if (isTyping && Input.anyKeyDown && isDialogue)
        {
            skipLine = true;
        }

        if (exitOnNextInput && Input.anyKeyDown)
        {
            StartCoroutine(FadeOut());
        }

        if (!isDialogue && Input.anyKeyDown && !dialoguePlaying)
        {
            isDialogue = true;
            StartCoroutine(TypeDialogue(dialogue, false, delay, timeBetweenLines));
        }
    }

    IEnumerator TypeDialogue(List<string> playDialogue, bool clearOnEachLine, float delayLetters, float delayLines)
    {
        textComponent.text = "";
        foreach (string line in playDialogue)
        {
            currentString = line;

            foreach (char c in line)
            {
                isTyping = true;
                textComponent.text += c;
                currentStringIndex++;
                if (skipLine && isDialogue)
                {
                    skipLine = false;
                    textComponent.text += currentString[currentStringIndex..];
                    break;
                }

                yield return new WaitForSeconds(delayLetters);
            }

            currentStringIndex = 0;
            isTyping = false;
            textComponent.text += "\n";
            yield return new WaitForSeconds(delayLines);
            if (clearOnEachLine)
                textComponent.text = "";
        }

        if (isDialogue)
            exitOnNextInput = true;
    }

    public void Init()
    {
        StartCoroutine(initialWaitScreen());
    }

    IEnumerator initialWaitScreen()
    {
        currentStringIndex = 0;
        currentString = "";
        isTyping = false;
        exitOnNextInput = false;
        dialoguePlaying = true;
        yield return StartCoroutine(TypeDialogue(new List<string> { "Loading..." }, true, 0.05f, 1f));
        yield return StartCoroutine(TypeDialogue(new List<string> { "Loading..." }, true, 0.05f, 1f));
        yield return StartCoroutine(TypeDialogue(new List<string> { "Press any key to start..." }, false, 0.05f, 1f));
        dialoguePlaying = false;
    }

    IEnumerator FadeOut()
    {
        float startAlpha = bootScreenCanvasGroup.alpha;
        float time = 0f;

        while (time < 2f)
        {
            time += Time.deltaTime;
            bootScreenCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / 2f);
            yield return null;
        }

        bootScreenCanvasGroup.alpha = 0f;
        bootScreenCanvasGroup.interactable = false;
        bootScreenCanvasGroup.blocksRaycasts = false;
        
        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }
}

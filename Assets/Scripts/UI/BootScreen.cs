using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bootscreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private List<string> dialogue;
    [SerializeField] private float delay = 0.05f;
    [SerializeField] private float timeBetweenLines = 1f;

    private bool isTyping = false;
    private int currentStringIndex = 0;
    private string currentString = "";
    private bool exitOnNextInput = false;
    private bool skipLine = false;

    private void Start()
    {
        StartCoroutine(TypeDialogue());
    }

    private void Update()
    {
        if (isTyping && Input.anyKeyDown)
        {
            skipLine = true;
        }

        if (exitOnNextInput && Input.anyKeyDown)
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator TypeDialogue()
    {
        textComponent.text = "";
        foreach (string line in dialogue)
        {
            currentString = line;

            foreach (char c in line)
            {
                isTyping = true;
                textComponent.text += c;
                currentStringIndex++;
                if (skipLine)
                {
                    skipLine = false;
                    textComponent.text += currentString[currentStringIndex..];
                    break;
                }
                yield return new WaitForSeconds(delay);
            }

            currentStringIndex = 0;
            isTyping = false;
            textComponent.text += "\n";
            yield return new WaitForSeconds(timeBetweenLines);
        }

        exitOnNextInput = true;
    }
}

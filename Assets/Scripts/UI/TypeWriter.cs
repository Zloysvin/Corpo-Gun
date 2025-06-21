using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    [SerializeField] private int CharactersPerSecond;
    [SerializeField] private EventReference typingSound;
    [SerializeField] private TMP_Text textBox;
    public event Action OnTypeWriterCleared;
    public event Action<int> OnLineFinished;
    public event Action OnTypeWriterFinished;
    public bool PlaySound = false;
    private Coroutine typewriter;
    private WaitForSeconds delay;
    private int currentStringIndex = 0;
    private int currentLineIndex = 0;
    private string currentString = "";
    private bool skipLine = false;
    private List<string> currentTexts = new List<string>();

    private void Awake()
    {
        delay = new WaitForSeconds(1f / CharactersPerSecond);
    }

    public void StartTypeWriter(List<string> texts, bool shouldClear, Action onFinshed = null)
    {
        ClearTypeWriter();
        currentTexts = texts;
        var num = TypeWriteCoroutine(shouldClear, onFinshed);
        typewriter = StartCoroutine(num);
    }

    private IEnumerator TypeWriteCoroutine(bool shouldClear, Action onFinished = null)
    {
        textBox.text = "";
        foreach (string line in currentTexts)
        {
            currentString = line;

            foreach (char c in line)
            {
                textBox.text += c;
                currentStringIndex++;
                if (skipLine)
                {
                    textBox.text += currentString[currentStringIndex..];
                    break;
                }
                // Skip the space if the next character is also a space
                if (!(c == ' ' && textBox.text.Length > currentStringIndex + 1 && currentString[currentStringIndex + 1] == ' '))
                    yield return delay;
            }

            currentStringIndex = 0;
            if (shouldClear)
            {
                ClearTypeWriter();
            }
            else
            {
                currentLineIndex++;
                textBox.text += "\n";
                OnLineFinished?.Invoke(currentLineIndex);
            }

            yield return delay;
        }

        OnTypeWriterFinished?.Invoke();
    }

    public void ClearTypeWriter()
    {
        StopAllCoroutines();
        textBox.text = string.Empty;
        currentTexts.Clear();
        currentString = string.Empty;
        currentStringIndex = 0;
        skipLine = false;
        OnTypeWriterCleared?.Invoke();
        currentLineIndex = 0;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    [SerializeField] private TMP_Text textBox;
    public event Action OnTypeWriterCleared;
    public event Action<int> OnLineFinished;
    public event Action OnTypeWriterFinished;
    public bool PlaySound = false;
    private Coroutine typewriter;
    private int currentStringIndex = 0;
    private int currentLineIndex = 0;
    private string currentString = "";
    private bool skipLine = false;
    private List<string> currentTexts = new List<string>();
    private EventInstance typingSound;
    private bool isTyping = false;

    private void Awake()
    {
        typingSound = RuntimeManager.CreateInstance(FMODEvents.Instance.consoleTypingSound);
        typingSound.setParameterByName("Volume", 0.5f);
    }

    public void StartTypeWriter(List<string> texts, bool shouldClearOnNewLine, bool clearCurrent, int charactersPerSecond, float delayBetweenLines = -1, Action onFinshed = null)
    {
        if (clearCurrent)
            SkipAll();

        WaitForSeconds delay = new WaitForSeconds(1f / charactersPerSecond);
        WaitForSeconds delayBetweenLinesWait = new(delayBetweenLines);
        if (delayBetweenLines < 0)
            delayBetweenLinesWait = delay;
        
        currentTexts = texts;
        var num = TypeWriteCoroutine(shouldClearOnNewLine, delay, delayBetweenLinesWait, onFinshed);
        typewriter = StartCoroutine(num);
    }

    IEnumerator TypingSoundLoop()
    {
        while (isTyping)
        {
            PLAYBACK_STATE playbackState;
            typingSound.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                typingSound.setPitch(UnityEngine.Random.Range(0.7f, 0.9f));
                typingSound.start();
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f, 0.08f));
        }
    }

    private IEnumerator TypeWriteCoroutine(bool shouldClear, WaitForSeconds delay, WaitForSeconds delayBetweenLines, Action onFinished = null)
    {
        isTyping = true;
        StartCoroutine(TypingSoundLoop());
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
                if (!(c == ' ' && currentStringIndex + 1 < currentString.Length && currentString[currentStringIndex + 1] == ' '))
                {
                    yield return delay;
                }
            }

            currentStringIndex = 0;

            yield return delayBetweenLines;

            if (shouldClear)
                textBox.text = string.Empty;
            else
                textBox.text += "\n";

            currentLineIndex++;
            OnLineFinished?.Invoke(currentLineIndex);

        }

        isTyping = false;
        OnTypeWriterFinished?.Invoke();
        onFinished?.Invoke();
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    public void SkipAll()
    {
        if (!isTyping)
            return;

        textBox.text = string.Empty;

        StopAllCoroutines();
        foreach (string line in currentTexts)
        {
            textBox.text += line + "\n";
        }
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
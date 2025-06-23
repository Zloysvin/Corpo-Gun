using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

// Controls the events and wavyness of the level

public class LevelManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup levelCanvasGroup;
    [SerializeField] private TypeWriter typeWriter;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private GameObject levelRoot;
    [SerializeField] private Camera tempCam;
    [SerializeField] private SkinnedMeshRenderer elevatorMesh;
    private EventInstance bgm;

    private int numberOfEnemies = 0;

    public void Start()
    {
        Debug.Log("LevelManager Start");
        numberOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        bgm = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.gameBGM, true);
        bgm.start();
        bgm.setParameterByName("MX Param", 0);
        levelRoot.SetActive(false);
        StartCoroutine(LevelStart());
    }

    public void OnEnemyKilled()
    {
        numberOfEnemies--;
        if (numberOfEnemies <= 0)
        {
            GameManager.Instance.CurrentGameState = GameState.Extraction;
        }
    }

    public void StartExtraction()
    {
        // Show extraction UI or logic here
        GameManager.Instance.CurrentGameState = GameState.Extraction;
    }

    public void EndLevel()
    {
        StartCoroutine(LevelEndRoutine());
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
            StartCoroutine(ElevatorEvent(0f, 100f, GameState.Playing));
        });
        yield return null;
    }

    IEnumerator LevelEndRoutine()
    {
        GameManager.Instance.CurrentGameState = GameState.Cutscene;
        StartCoroutine(HUD.Instance.FadeGroup(levelCanvasGroup, 0f, 1f, fadeDuration));
        yield return StartCoroutine(ElevatorEvent(100f, 0f, GameState.Cutscene));
        bgm.stop(STOP_MODE.ALLOWFADEOUT);
        bgm.release();
        levelRoot.SetActive(false);
        tempCam.gameObject.SetActive(true);
        typeWriter.StartTypeWriter(new List<string>
        {
            "Mission complete.",
            "Returning to base.",
            "Agent " + GameManager.Instance.agentName + ", you are dismissed."
        }, false, true, () =>
        {
            GameManager.Instance.LoadMainMenu();
        });
    }

    private IEnumerator ElevatorEvent(float start, float end, GameState nextState)
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.elevatorSound, elevatorMesh.transform.position, false);
        float duration = 3.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            elevatorMesh.SetBlendShapeWeight(0, Mathf.Lerp(start, end, t));
            yield return null;
        }

        elevatorMesh.SetBlendShapeWeight(0, end);
        GameManager.Instance.CurrentGameState = nextState;
    }
}

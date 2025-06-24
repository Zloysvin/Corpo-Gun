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
    private EventInstance bgm;
    private Player player;
    private int numberOfEnemies = 0;

    public void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>();

        ElevatorExtraction[] elevatorsInLevel = FindObjectsByType<ElevatorExtraction>(FindObjectsSortMode.None);
        foreach (var elevator in elevatorsInLevel)
            elevator.SetLevelManager(this);
        
        ElevatorExtraction startElevator = elevatorsInLevel[Random.Range(0, elevatorsInLevel.Length)];
        startElevator.SetPlayerPosition(player, false);
        
        numberOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        bgm = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.gameBGM, true);
        bgm.start();
        bgm.setParameterByName("MX Param", 0);
        StartCoroutine(LevelStart(startElevator));
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

    public void EndLevel(ElevatorExtraction elevator)
    {
        StartCoroutine(LevelEndRoutine(elevator));
    }

    IEnumerator LevelStart(ElevatorExtraction startingElevator)
    {
        yield return new WaitForSeconds(3f);
        typeWriter.StartTypeWriter(new List<string>
        {
            "Initializing system...",
            "Booting weapon systems...",
            "Testing suit integrity...",
            "All systems nominal...",
            "Agent " + GameManager.Instance.agentName + " is ready for deployment.",
        }, false, true, 40, onFinshed: () =>
        {
            StartCoroutine(HUD.Instance.FadeGroup(levelCanvasGroup, 1f, 0f, fadeDuration));
            HUD.Instance.ShowHUD();
            StartCoroutine(startingElevator.ElevatorEvent(0f, 100f, GameState.Playing));
        });
        yield return null;
    }

    IEnumerator LevelEndRoutine(ElevatorExtraction elevator)
    {
        GameManager.Instance.CurrentGameState = GameState.Cutscene;
        StartCoroutine(HUD.Instance.FadeGroup(levelCanvasGroup, 0f, 1f, fadeDuration));
        elevator.SetPlayerPosition(player, true);
        yield return StartCoroutine(elevator.ElevatorEvent(100f, 0f, GameState.Cutscene));
        bgm.stop(STOP_MODE.ALLOWFADEOUT);
        bgm.release();
        typeWriter.StartTypeWriter(new List<string>
        {
            "Mission complete.",
            "Returning to base.",
            "Agent " + GameManager.Instance.agentName + ", you are dismissed."
        }, false, true, 40, onFinshed: () =>
        {
            GameManager.Instance.LoadMainMenu();
        });
    }

    IEnumerator LevelFailed()
    {
        GameManager.Instance.CurrentGameState = GameState.Cutscene;
        yield return StartCoroutine(HUD.Instance.FadeGroup(levelCanvasGroup, 0f, 1f, fadeDuration));
        bgm.stop(STOP_MODE.ALLOWFADEOUT);
        bgm.release();
        typeWriter.StartTypeWriter(new List<string>
        {
            "          .                                                      .",
            "        .n                   .                 .                  n.",
            "  .   .dP                  dP                   9b                 9b.    .",
            " 4    qXb         .       dX                     Xb       .        dXp     t",
            "dX.    9Xb      .dXb    __                         __    dXb.     dXP     .Xb",
            "9XXb._       _.dXXXXb dXXXXbo.                 .odXXXXb dXXXXb._       _.dXXP",
            " 9XXXXXXXXXXXXXXXXXXXVXXXXXXXXOo.           .oOXXXXXXXXVXXXXXXXXXXXXXXXXXXXP",
            "  `9XXXXXXXXXXXXXXXXXXXXX'~   ~`OOO8b   d8OOO'~   ~`XXXXXXXXXXXXXXXXXXXXXP'",
            "    `9XXXXXXXXXXXP' `9XX'   GAME   `98v8P'   OVER   `XXP' `9XXXXXXXXXXXP'",
            "        ~~~~~~~       9X.          .db|db.          .XP       ~~~~~~~",
            "                        )b.  .dbo.dP'`v'`9b.odb.  .dX(",
            "                      ,dXXXXXXXXXXXb     dXXXXXXXXXXXb.",
            "                     dXXXXXXXXXXXP'   .   `9XXXXXXXXXXXb",
            "                    dXXXXXXXXXXXXb   d|b   dXXXXXXXXXXXXb",
            "                    9XXb'   `XXXXXb.dX|Xb.dXXXXX'   `dXXP",
            "                     `'      9XXXXXX(   )XXXXXXP      `'",
            "                              XXXX X.`v'.X XXXX",
            "                              XP^X'`b   d'`X^XX",
            "                              X. 9  `   '  P )X",
            "                              `b  `       '  d'",
            "                               `             '"
        }, false, true, 200, onFinshed: () =>
        {
            GameManager.Instance.LoadMainMenu();
        });
    }
}

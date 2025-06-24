using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Extraction,
    Paused,
    Cutscene,
}

public class Level
{
    public Level(string id, string requiredId, bool isUnlocked, bool isComplete, int difficulty, List<string> description, int buildId, bool underDevelopment)
    {
        this.id = id;
        this.requiredId = requiredId;
        this.isUnlocked = isUnlocked;
        this.isComplete = isComplete;
        this.difficulty = difficulty;
        this.description = description;
        this.buildId = buildId;
        this.underDevelopment = underDevelopment;
    }

    public string id;
    public string requiredId;
    public bool isUnlocked;
    public bool isComplete;
    public int difficulty;
    public List<string> description;
    public int buildId;
    public bool underDevelopment;

    public string GetMenuString()
    {
        return id + "      Status: " + (underDevelopment ? "In Development      " : (isUnlocked ? "Active              " : "Inactive            ")) + "| Level Difficulty: " + difficulty;
    }
}

public class GameManager : MonoBehaviour
{
    // ------------------ SCENE / GAMESTATE MANAGEMENT ------------------ //

    private float volume = 1f;
    private float sfxVolume = 1f;
    public bool exitedLevel = false;
    public GameState CurrentGameState = GameState.Menu;
    public Level CurrentLevel;
    private static GameManager _instance;

    public Dictionary<string, Level> levelDirectory = new Dictionary<string, Level>
    {
        { "G7225", new Level("G7225", null, true, false, 1, null, 1, false)},
        { "H4993", new Level("H4993", "G7225", false, false, 2, null, 2, false)},
        { "Z1173", new Level("Z1173", "H4993", false, false, 3, null, 3, true)},
        { "P9901", new Level("P9901", "Z1173", false, false, 4, null, 4, true)},
    };

    public static GameManager Instance
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

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnLevelWon()
    {
        foreach (var lvl in levelDirectory.Values)
        {
            if (lvl.requiredId == CurrentLevel.id)
            {
                lvl.isUnlocked = true;
            }

            CurrentLevel.isComplete = true;
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(CurrentLevel.buildId);
        CurrentGameState = GameState.Cutscene;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        CurrentGameState = GameState.Menu;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            // bootScreen.StartGame();
        }
    }

    public bool IsGameInPlay()
    {
        if (CurrentGameState == GameState.Playing || CurrentGameState == GameState.Extraction)
        {
            return true;
        }
        return false;
    }

    public bool IsCutscene()
    {
        return CurrentGameState == GameState.Cutscene;
    }

    public float GetVolume()
    {
        return volume;
    }

    public void SetVolume(float value)
    {
        volume = Mathf.Clamp01(value);
        AudioManager.Instance.OnVolumeChanged(volume);
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        AudioManager.Instance.OnSFXVolumeChanged(sfxVolume);
    }
}

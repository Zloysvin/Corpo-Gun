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

public class GameManager : MonoBehaviour
{
    // ------------------ SCENE / GAMESTATE MANAGEMENT ------------------ //



    private float volume = 1f;
    public string agentName = "Agent";
    public int difficulty = 1;

    public GameState CurrentGameState = GameState.Menu;

    private static GameManager _instance;

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

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
}

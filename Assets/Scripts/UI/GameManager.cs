using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ------------------ SCENE / GAMESTATE MANAGEMENT ------------------ //

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
        Debug.Log("Loading game...");
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
}

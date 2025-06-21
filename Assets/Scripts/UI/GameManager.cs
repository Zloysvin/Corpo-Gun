using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Menu bootScreen;

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
        bootScreen.PlayMenuStart();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

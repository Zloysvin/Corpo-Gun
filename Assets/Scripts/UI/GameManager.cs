using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Bootscreen bootScreen;

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

        Instance = this;
    }

    private void Start()
    {
        bootScreen.gameObject.SetActive(true);
        bootScreen.Init();
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

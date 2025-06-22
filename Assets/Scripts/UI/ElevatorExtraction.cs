using UnityEngine;

public class ElevatorExtraction : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    private bool isExtractionZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered extraction zone");
            isExtractionZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isExtractionZone = false;
        }
    }

    void Update()
    {
        if (isExtractionZone && GameManager.Instance.CurrentGameState == GameState.Extraction)
        {
            levelManager.EndLevel();
        }
    }
}

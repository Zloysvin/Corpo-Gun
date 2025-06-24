using System.Collections;
using UnityEngine;

public class ElevatorExtraction : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer elevatorMesh;
    [SerializeField] private Transform spawnPoint;
    private LevelManager levelManager;
    private bool isExtractionZone = false;
    private bool hasExtracted = false;

    public bool IsPlayerInExtractionZone()
    {
        return isExtractionZone;
    }

    public void SetLevelManager(LevelManager manager)
    {
        levelManager = manager;
    }

    public void SetPlayerPosition(Player player, bool animate)
    {
        if (animate)
        {
            player.Teleport(spawnPoint.position);
        }
        else
        {
            player.Teleport(spawnPoint.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            isExtractionZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            isExtractionZone = false;
        }
    }

    void Update()
    {
        if (isExtractionZone && GameManager.Instance.CurrentGameState == GameState.Extraction && !hasExtracted)
        {
            levelManager.EndLevel(this);
            hasExtracted = true;
        }
    }

    public IEnumerator ElevatorEvent(float start, float end, GameState stateAtEnd)
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
        GameManager.Instance.CurrentGameState = stateAtEnd;
    }
}

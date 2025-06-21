using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
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

    public void PlayOneShot(EventReference soundReference, Vector3 position)
    {
        RuntimeManager.PlayOneShot(soundReference, position);
    }

    public void PlayOneShot(EventReference soundReference)
    {
        RuntimeManager.PlayOneShot(soundReference);
    }

    public void PlaySoundLoop(EventReference soundReference, Vector3 position)
    {
        RuntimeManager.PlayOneShot(soundReference, position);
    }

    public void StopSoundLoop(EventReference soundReference)
    {
        // Stop
    }
}

using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> _activeInstances = new List<EventInstance>();

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

    public EventInstance CreateEventInstance(EventReference soundReference)
    {
        EventInstance instance = RuntimeManager.CreateInstance(soundReference);
        _activeInstances.Add(instance);
        return instance;
    }

    private void CleanUp()
    {
        foreach (var instance in _activeInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}

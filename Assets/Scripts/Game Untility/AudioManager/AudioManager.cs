using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> _activeInstances = new List<EventInstance>();
    private EventInstance bgmInstance;

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

    public void RemoveInstance(EventInstance instance)
    {
        if (_activeInstances.Contains(instance))
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
            _activeInstances.Remove(instance);
        }
    }

    public void StartMusicCore(string musicName)
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            bgmInstance.release();
        }
        bgmInstance = RuntimeManager.CreateInstance(musicName);
        bgmInstance.start();
    }

    private void OnDestroy()
    {
        bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bgmInstance.release();
        CleanUp();
    }
}

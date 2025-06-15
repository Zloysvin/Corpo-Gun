using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public abstract class EffectBase : ScriptableObject
{
    public abstract void CreateInstance(Vector3 hitPoint, Vector3 hitNormal);
}

public abstract class Effect<TInstance> : EffectBase where TInstance : EffectInstanceBase
{
    public TInstance prefab;

    private ObjectPool<TInstance> effectPool;

    public override void CreateInstance(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (effectPool == null)
        {
            effectPool = new ObjectPool<TInstance>(
                createFunc: () => Instantiate(prefab),
                actionOnGet: instance => instance.gameObject.SetActive(true),
                actionOnRelease: instance => instance.gameObject.SetActive(false),
                actionOnDestroy: instance => Destroy(instance.gameObject),
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        // THIS IS BEAUTIFUL (˵ ͜ʖ ˵)
        TInstance instance = effectPool.Get();
        var typedInstance = instance as EffectInstanceBase;
        if (typedInstance != null)
        {
            var initializeMethod = instance.GetType().GetMethod("InitializeEffectData");
            if (initializeMethod != null)
            {
                initializeMethod.Invoke(instance, new object[] { this });
                typedInstance.PlayEffect(hitPoint, hitNormal);
            }
            else
            {
                Debug.LogError($"InitializeEffectData method not found in {instance.GetType().Name}");
            }
        }
    }
}

[CreateAssetMenu(fileName = "Audio Effect", menuName = "Impact System/Effects/Audio Effect", order = 2)]
public class PlayAudioEffect : Effect<AudioEffectInstance>
{
    public List<AudioClip> audioClips = new();
    public Vector2 volumeRange = new(0f, 1f);
    public float soundOffset = 1f;
}

[CreateAssetMenu(menuName = "Impact System/Effects/Spawn Object Effect", fileName = "Spawn Object Effect", order = 0)]
public class PlayParticleEffect : Effect<ParticleEffectInstance>
{
    public float probability = 1f;
    public bool randomizeRotation;
    public Vector3 randomizedRotationMultiplier = Vector3.zero;
}

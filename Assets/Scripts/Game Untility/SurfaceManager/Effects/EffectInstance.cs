using UnityEngine;

public interface IEffectInstance
{
    void InitializeEffectData(EffectBase effectData);
    void PlayEffect(Vector3 hitPoint, Vector3 hitNormal);
}

public abstract class EffectInstanceBase : MonoBehaviour, IEffectInstance
{
    public abstract void InitializeEffectData(EffectBase effectData);

    public abstract void PlayEffect(Vector3 hitPoint, Vector3 hitNormal);
}

public abstract class EffectInstance<TEffect> : EffectInstanceBase where TEffect : EffectBase
{
    protected TEffect effectData;

    public override void InitializeEffectData(EffectBase effect)
    {
        effectData = effect as TEffect;

        if (effectData == null)
        {
            Debug.LogError($"Effect data was not of expected type {typeof(TEffect).Name}");
        }
    }
}
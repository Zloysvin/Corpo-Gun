using UnityEngine;

public abstract class EffectInstanceBase : MonoBehaviour
{
    public abstract void PlayEffect(Vector3 hitPoint, Vector3 hitNormal);
}

public abstract class EffectInstance<TEffect> : EffectInstanceBase
{
    protected TEffect effectData;

    public override abstract void PlayEffect(Vector3 hitPoint, Vector3 hitNormal);

    public void InitializeEffectData(TEffect effect)
    {
        effectData = effect;
    }
}

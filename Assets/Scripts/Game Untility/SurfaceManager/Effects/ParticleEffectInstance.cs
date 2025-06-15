using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffectInstance : EffectInstance<PlayParticleEffect>
{
    private PlayParticleEffect playParticleEffect;

    public override void PlayEffect(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (playParticleEffect == null)
            playParticleEffect = effectData;

        if (playParticleEffect.probability > Random.value)
        {
            Quaternion baseRotation = Quaternion.LookRotation(hitNormal);

            if (playParticleEffect.randomizeRotation)
            {
                Vector3 offset = new Vector3(
                    Random.Range(0, 180 * playParticleEffect.randomizedRotationMultiplier.x),
                    Random.Range(0, 180 * playParticleEffect.randomizedRotationMultiplier.y),
                    Random.Range(0, 180 * playParticleEffect.randomizedRotationMultiplier.z)
                );

                baseRotation *= Quaternion.Euler(offset);
            }

            transform.SetPositionAndRotation(hitPoint + hitNormal * 0.001f, baseRotation);
        }
    }
}

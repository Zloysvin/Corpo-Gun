using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffectInstance : EffectInstance<PlayParticleEffect>
{
    public override void PlayEffect(Vector3 hitPoint, Vector3 hitNormal)
    {
        PlayParticleEffect data = effectData;
        if (data == null)
        {
            Debug.LogError("Effect data is not of type PlayAudioEffect");
            return;
        }

        if (data.probability > Random.value)
        {
            Quaternion baseRotation = Quaternion.LookRotation(hitNormal);

            if (data.randomizeRotation)
            {
                Vector3 offset = new Vector3(
                    Random.Range(0, 180 * data.randomizedRotationMultiplier.x),
                    Random.Range(0, 180 * data.randomizedRotationMultiplier.y),
                    Random.Range(0, 180 * data.randomizedRotationMultiplier.z)
                );

                baseRotation *= Quaternion.Euler(offset);
            }

            transform.SetPositionAndRotation(hitPoint + hitNormal * 0.001f, baseRotation);
        }
    }
}

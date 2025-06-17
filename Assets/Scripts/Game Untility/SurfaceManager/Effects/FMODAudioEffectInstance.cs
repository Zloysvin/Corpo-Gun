using UnityEngine;
using FMODUnity;

public class FMODAudioEffectInstance : EffectInstance<PlayFMODAudioEffect>
{
    public override void PlayEffect(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (data == null)
        {
            Debug.LogError("Effect data is not of type PlayFMODAudioEffect");
            return;
        }

        if (data.soundReferences.Count == 0)
            return;

        EventReference clip = data.soundReferences[Random.Range(0, data.soundReferences.Count)];
        Vector3 position = hitPoint + hitNormal * data.soundOffset;

        AudioManager.Instance.PlayOneShot(clip, position);

        // StartCoroutine(DisposeAfter(clip.length));
    }
}

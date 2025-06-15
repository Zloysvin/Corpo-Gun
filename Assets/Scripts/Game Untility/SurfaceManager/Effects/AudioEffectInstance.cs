using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEffectInstance : EffectInstance<PlayAudioEffect>
{    
    public override void PlayEffect(Vector3 hitPoint, Vector3 hitNormal)
    {
        PlayAudioEffect data = effectData;

        if (data == null)
        {
            Debug.LogError("Effect data is not of type PlayAudioEffect");
            return;
        }

        if (data.audioClips.Count == 0)
            return;

        AudioClip clip = data.audioClips[Random.Range(0, data.audioClips.Count)];
        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.transform.position = hitPoint;
        audioSource.PlayOneShot(clip, data.soundOffset * Random.Range(data.volumeRange.x, data.volumeRange.y));

        StartCoroutine(DisposeAfter(clip.length));
    }

    private IEnumerator DisposeAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        effectData.ReleaseInstance(this);
    }
}
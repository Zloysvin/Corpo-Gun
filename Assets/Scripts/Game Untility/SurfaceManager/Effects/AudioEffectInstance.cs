using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEffectInstance : EffectInstance<PlayAudioEffect>
{
    private PlayAudioEffect playAudioEffect;
    
    public override void PlayEffect(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (playAudioEffect == null)
            playAudioEffect = effectData;

        Debug.Log(playAudioEffect);

        AudioClip clip = playAudioEffect.audioClips[Random.Range(0, playAudioEffect.audioClips.Count)];
        AudioSource audioSource = GetComponent<AudioSource>();

        audioSource.transform.position = hitPoint;
        audioSource.PlayOneShot(clip, playAudioEffect.soundOffset * Random.Range(playAudioEffect.volumeRange.x, playAudioEffect.volumeRange.y));

        StartCoroutine(DisposeAfter(clip.length));
    }

    private IEnumerator DisposeAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
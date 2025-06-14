using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Effect", menuName = "Impact System/Effects/Audio Effect", order = 2)]
public class PlayAudioEffect : Effect
{
    public AudioSource audioSourcePrefab;
    public List<AudioClip> audioClips = new();
    public Vector2 volumeRange = new(0f, 1f);
    public float soundOffset = 1f;

    public override void PlayEffect(Vector3 hitPoint, Vector3 hitNormal, int defaultPoolSize, System.Action<IEnumerator> coroutineRunner)
    {
        AudioClip clip = audioClips[Random.Range(0, audioClips.Count)];
        ObjectPool pool = ObjectPool.CreateInstance(audioSourcePrefab.GetComponent<PoolableObject>(), defaultPoolSize);
        AudioSource audioSource = pool.GetObject().GetComponent<AudioSource>();

        audioSource.transform.position = hitPoint;
        audioSource.PlayOneShot(clip, soundOffset * Random.Range(volumeRange.x, volumeRange.y));

        IEnumerator CoroutineWrapper()
        {
            yield return new WaitForSeconds(clip.length);
            audioSource.gameObject.SetActive(false);
        }

        coroutineRunner?.Invoke(CoroutineWrapper());
    }
}

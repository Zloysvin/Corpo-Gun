using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Impact System/Effects/Spawn Object Effect", fileName = "Spawn Object Effect", order = 0)]
public class SpawnObjectEffect : Effect
{
    public GameObject prefab;
    public float probability = 1f;
    public bool randomizeRotation;
    public Vector3 randomizedRotationMultiplier = Vector3.zero;

    public override void PlayEffect(Vector3 hitPoint, Vector3 hitNormal, int defaultPoolSize, System.Action<IEnumerator> coroutineRunner = null)
    {
        if (probability > Random.value)
        {
            ObjectPool pool = ObjectPool.CreateInstance(prefab.GetComponent<PoolableObject>(), defaultPoolSize);
            Quaternion baseRotation = Quaternion.LookRotation(hitNormal);

            if (randomizeRotation)
            {
                Vector3 offset = new Vector3(
                    Random.Range(0, 180 * randomizedRotationMultiplier.x),
                    Random.Range(0, 180 * randomizedRotationMultiplier.y),
                    Random.Range(0, 180 * randomizedRotationMultiplier.z)
                );

                baseRotation *= Quaternion.Euler(offset);
            }

            pool.GetObject(hitPoint + hitNormal * 0.001f, baseRotation);
        }
    }
}

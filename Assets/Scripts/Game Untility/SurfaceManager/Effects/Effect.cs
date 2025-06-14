using System.Collections;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public abstract void PlayEffect(Vector3 hitPoint, Vector3 hitNormal, int defaultPoolSize, System.Action<IEnumerator> coroutineRunner);
}

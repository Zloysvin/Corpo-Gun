using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "FMOD Audio Effect", menuName = "Impact System/Effects/FMOD Audio Effect", order = 1)]
public class PlayFMODAudioEffect : Effect
{
    public override void PlayEffect(Vector3 hitPoint, Vector3 hitNormal, int defaultPoolSize, Action<IEnumerator> coroutineRunner)
    {
        throw new NotImplementedException();
    }
}

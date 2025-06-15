using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Impact System/Surface Definition", fileName = "Surface Definition")]
public class Surface : ScriptableObject
{
    [System.Serializable] public class SurfaceImpactTypeEffect
    {
        public ImpactType impactType;
        public List<EffectBase> effects = new();
    }

    public List<SurfaceImpactTypeEffect> impactTypeEffects = new List<SurfaceImpactTypeEffect>();
}

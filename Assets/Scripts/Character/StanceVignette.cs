using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StanceVignette : MonoBehaviour
{
    [SerializeField] private float min = 0.1f;
    [SerializeField] private float max = 0.4f;
    [SerializeField] private float responseTime = 4f;

    private VolumeProfile _profile;
    private Vignette _vignette;
    public void Initialize(VolumeProfile profile)
    {
        _profile = profile;

        if (!_profile.TryGet(out _vignette))
        {
            _vignette = profile.Add<Vignette>();
        }

        _vignette.intensity.Override(min);
    }

    public void UpdateVignette(float deltaTime, Stance stance)
    {
        var targetIntensity = stance is Stance.Stand ? min : max;
        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, targetIntensity, 1f - Mathf.Exp(-responseTime * deltaTime));
    }
}

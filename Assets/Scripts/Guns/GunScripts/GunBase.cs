using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public abstract class GunBase : MonoBehaviour
{
    public GunType type;
    [SerializeField] private ImpactType impactType;
    [SerializeField] private string id;
    [SerializeField] private Vector3 spawnPoint;
    [SerializeField] private ParticleSystem fireParticleSystem;
    [SerializeField] private ShootConfigurationScriptableObject shootConfig;
    [SerializeField] private TrailConfigScriptableObject trailConfig;
    [SerializeField] private Transform trailStart;

    private Transform raycastOrigin;
    private float lastShootTime;
    private ObjectPool<TrailRenderer> trailPool;

    public virtual void Initialize(Transform raycastOrigin)
    {
        gameObject.SetActive(false);
        lastShootTime = 0f;
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        this.raycastOrigin = raycastOrigin;

        transform.localPosition = spawnPoint;
    }

    public virtual void SwitchToWeapon()
    {
        gameObject.SetActive(true);
        // Some sort of animation or logic to switch to this weapon
    }

    public virtual void SwitchOffWeapon()
    {
        if (fireParticleSystem != null)
        {
            fireParticleSystem.Stop();
        }
        gameObject.SetActive(false);

        // Some sort of animation or logic to switch off this weapon
    }

    public virtual void Shoot()
    {
        if (Time.time > shootConfig.FireRate + lastShootTime)
        {
            lastShootTime = Time.time;
            fireParticleSystem.Play();
            Vector3 shootDirection = raycastOrigin.transform.forward + new Vector3(
                Random.Range(-shootConfig.Spread.x, shootConfig.Spread.x),
                Random.Range(-shootConfig.Spread.y, shootConfig.Spread.y),
                Random.Range(-shootConfig.Spread.z, shootConfig.Spread.z)
            );

            shootDirection.Normalize();

            if (Physics.Raycast(raycastOrigin.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, shootConfig.HitMask))
            {
                StartCoroutine(PlayTrail(trailStart.position, hit.point, hit));
            }
            else
            {
                StartCoroutine(PlayTrail(trailStart.position, raycastOrigin.transform.position + (shootDirection * trailConfig.missDistance), new RaycastHit()));
            }
        }
    }

    protected IEnumerator PlayTrail(Vector3 start, Vector3 end, RaycastHit hit)
    {
        TrailRenderer trail = trailPool.Get();
        trail.gameObject.SetActive(true);
        trail.transform.position = start;
        yield return null;

        trail.enabled = true;
        trail.emitting = true;

        float distance = Vector3.Distance(start, end);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;

            yield return null;
        }

        trail.transform.position = end;

        if (hit.collider != null)
        {
            // SurfaceManager.Instance.HandleImpact(hit.transform.gameObject, end, hit.normal, impactType, 0);
        }
        else
        {
            trail.transform.rotation = Quaternion.LookRotation((end - start).normalized, Vector3.up);
        }

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        trail.emitting = false;
        trail.enabled = false;
        trail.gameObject.SetActive(false);
        trailPool.Release(trail);
    }

    protected TrailRenderer CreateTrail()
    {
        TrailRenderer trail = new GameObject("Trail").AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.color;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertedxDistance;

        trail.enabled = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
}
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField] private GunType startingGun;
    [SerializeField] private Transform gunRaycastOrigin;
    [SerializeField] private List<GameObject> gunPrefabs;
    private Dictionary<GunType, GameObject> guns;

    [Space]
    [Header("Runtime filled")]
    public GunBase activeGun;

    void Start()
    {
        guns = new Dictionary<GunType, GameObject>();
        
        foreach (GameObject gunPrefab in gunPrefabs)
        {
            GameObject instance = Instantiate(gunPrefab, transform);
            if (!instance.TryGetComponent<GunBase>(out GunBase gunBase))
            {
                Debug.LogError($"Gun prefab {gunPrefab.name} does not have a GunBase component.");
                Destroy(instance);
                continue;
            }

            gunBase.Initialize(gunRaycastOrigin);
            guns[gunBase.type] = instance;
        }

        SelectWeapon(startingGun);
    }
    
    private void SelectWeapon(GunType gunType)
    {
        // Use a better way to find the gun some sort of player equipment manager
        
        if (!guns[gunType].TryGetComponent<GunBase>(out var gun))
        {
            Debug.LogError($"Gun of type {gun} not found in the list.");
            return;
        }

        activeGun = gun;
        gun.SwitchToWeapon();
    }
}

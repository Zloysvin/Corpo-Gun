using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour
{
    [SerializeField] private GunType gun;
    [SerializeField] private Transform gunParent;
    [SerializeField] private List<GunScriptableObject> guns;
    // [SerializeField] private PlayerIK inverseKinematics;

    [Space]
    [Header("Runtime filled")]
    public GunScriptableObject activeGun;

    private void Start()
    {
        // Use a better way to find the gun some sort of player equipment manager
        GunScriptableObject gun = guns.Find(g => g.type == this.gun);

        if (gun == null)
        {
            Debug.LogError($"Gun of type {this.gun} not found in the list.");
            return;
        }

        activeGun = gun;
        gun.Spawn(gunParent, this);
    }
}

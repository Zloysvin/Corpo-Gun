using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerGunSelector gunSelector;

    void Update()
    {
        // Todo improve
        if (Mouse.current.leftButton.isPressed && gunSelector.activeGun != null)
        {
            gunSelector.activeGun.Shoot();
        }
    }
}

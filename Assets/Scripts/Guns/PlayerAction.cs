using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerGunSelector gunSelector;

    void Update()
    {
        if (!GameManager.Instance.IsGameInPlay()) return;

        if (Mouse.current.leftButton.isPressed && gunSelector.activeGun != null)
        {
            gunSelector.activeGun.Shoot();
        }
    }
}

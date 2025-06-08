using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private PlayerCamera _playerCamera;

    private PlayerInputActions _inputActions;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        _playerCharacter.Initialize();
        _playerCamera.Initialize(_playerCharacter.GetCameraTarget());
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    void Update()
    {
        var input = _inputActions.Gameplay;

        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        _playerCamera.UpdateRotation(cameraInput);

        var characterInput = new CharacterInput 
        { 
            Rotation = _playerCamera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>()
        };
        _playerCharacter.UpdateInput(characterInput);
    }

    void LateUpdate()
    {
        _playerCamera.UpdatePosition(_playerCharacter.GetCameraTarget());
    }

}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;
    [SerializeField] private StanceVignette stanceVignette;
    [SerializeField] private Volume volume;

    // Probably rework where this should go
    [Header("Input Settings")]
    [SerializeField] private bool toggleSprint = false;
    [SerializeField] private bool toggleCrouch = false;


    private PlayerInputActions _inputActions;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());

        cameraSpring.Initialize();
        cameraLean.Initialize();
        stanceVignette.Initialize(volume.profile);
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    void Update()
    {
        var input = _inputActions.Gameplay;
        var deltaTime = Time.deltaTime;

        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        var characterInput = new CharacterInput
        {
            Rotation    = playerCamera.transform.rotation,
            Move        = input.Move.ReadValue<Vector2>(),
            Jump        = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Crouch      = toggleCrouch ? input.Crouch.WasPressedThisFrame() ? InputType.ToggleOn : InputType.ToggleOff : input.Crouch.IsPressed() ? InputType.Held : InputType.Off,
            Sprint      = toggleSprint ? input.Sprint.WasPressedThisFrame() ? InputType.ToggleOn : InputType.ToggleOff : input.Sprint.IsPressed() ? InputType.Held : InputType.Off
        };

        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);

    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;
        var cameraTarget = playerCharacter.GetCameraTarget();
        var state = playerCharacter.GetState();

        playerCamera.UpdatePosition(cameraTarget);
        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean(deltaTime, state.Acceleration, cameraTarget.up);

        stanceVignette.UpdateVignette(deltaTime, state.Stance);
    }

    void Teleport(Vector3 position)
    {
        playerCharacter.SetPosition(position);
    }

}

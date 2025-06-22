using FMOD.Studio;
using FMODUnity;
using KinematicCharacterController;
using Unity.VisualScripting;
using UnityEngine;

public enum InputType
{
    ToggleOn, ToggleOff, Held, Off
}

public enum Stance
{
    Stand, Crouch
}

public struct CharacterState
{
    public Stance Stance;
    public bool Grounded;
    public Vector3 Velocity;
    // Not raw acceleration, excludes gravity and other forces
    // User movement driven acceleration
    public Vector3 Acceleration;
}

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool JumpSustain;
    public InputType Crouch;
    public InputType Sprint;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    // Override settings
    [Header("Character Settings")]
    [SerializeField] private bool allowJump = true;
    [SerializeField] private bool allowHoldJump = true;
    [SerializeField] private bool allowCrouch = true;
    [Tooltip("Allows you to queue a jump before you hit the ground transitioning quickly into the next jump (like b-hopping).")]
    [SerializeField] private bool allowCoyoteBefore = true;
    [Tooltip("Allows a split second jump after you leave the ground. Useful for platformers")]
    [SerializeField] private bool allowCoyoteAfter = true;


    [Header("Character Components")]
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform cameraTarget;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 6.5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float walkResponse = 25f;
    [SerializeField] private float crouchResponse = 20f;

    [Space]

    [SerializeField] private float coyoteTimeBefore = 0.2f;
    [SerializeField] private float coyoteTimeAfter = 0.2f;
    [SerializeField] private float airSpeed = 6.5f;
    [SerializeField] private float airAcceleration = 6.5f;
    [SerializeField] private float jumpSpeed = 11f;
    [Range(0f, 1f)][SerializeField] private float jumpSustainGravity = 0.6f;
    [SerializeField] private float gravity = -50f;

    [Space]

    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float crouchHeightResponse = 4f;
    [Range(0f, 1f)][SerializeField] private float standCameraTargetHeight = 0.9f;
    [Range(0f, 1f)][SerializeField] private float crouchCameraTargetHeight = 0.7f;

    private CharacterState _state;
    private CharacterState _lastState;
    private CharacterState _tempState;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedCrouch;
    private bool _requestedJumpSustain;
    private bool _requestedSprint;

    private Collider[] _uncrouchOverlapResults;

    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;

    private EventInstance footStepSound;
    private Vector3 _lastFootStepPosition;
    private float footStepDistance = 1.9f;

    public void Initialize()
    {
        _state.Stance = Stance.Stand;
        _lastState = _state;

        _uncrouchOverlapResults = new Collider[8];
        motor.CharacterController = this;

        footStepSound = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.playerFootstepSound, false);
    }

    public void UpdateInput(CharacterInput input)
    {
        _lastState = _state;

        _requestedRotation = input.Rotation;

        // Convert to 3D space and clamp to prevent diagonal movement speed increase
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);

        // Oreint movement to look direction
        _requestedMovement = input.Rotation * _requestedMovement;

        var wasRequestingJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && !wasRequestingJump)
        {
            _timeSinceJumpRequest = 0f;
        }

        _requestedJumpSustain = allowHoldJump && input.JumpSustain;
        _requestedCrouch = allowCrouch && input.Crouch switch
        {
            InputType.ToggleOn => !_requestedCrouch,
            InputType.ToggleOff => _requestedCrouch,
            InputType.Held => true,
            InputType.Off => false,
            _ => _requestedCrouch
        };

        _requestedSprint = input.Sprint switch
        {
            InputType.ToggleOn => !_requestedSprint,
            InputType.ToggleOff => _requestedSprint,
            InputType.Held => true,
            InputType.Off => false,
            _ => _requestedSprint
        };
    }

    public void UpdateBody(float deltaTime)
    {
        var currentHeight = motor.Capsule.height;
        var cameraTargetHeight = currentHeight * (_state.Stance == Stance.Stand ? standCameraTargetHeight : crouchCameraTargetHeight);

        cameraTarget.localPosition = Vector3.Lerp(
            a: cameraTarget.localPosition,
            b: new Vector3(0f, cameraTargetHeight, 0f),
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime) // Remain framerate independent
        );
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        _state.Acceleration = Vector3.zero;

        // Grounded movement
        if (motor.GroundingStatus.IsStableOnGround)
        {
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;

            var groundedMovement = motor.GetDirectionTangentToSurface(_requestedMovement, motor.GroundingStatus.GroundNormal) * _requestedMovement.magnitude;

            // Calculate speed and response based on Stance
            var speed = _state.Stance is Stance.Stand ? _requestedSprint ? sprintSpeed : walkSpeed : crouchSpeed;
            var response = _state.Stance is Stance.Stand ? walkResponse : crouchResponse;

            // Apply response to the current velocity
            var targetVelocity = groundedMovement * speed;
            var moveVelocity = Vector3.Lerp(
                a: currentVelocity,
                b: targetVelocity,
                t: 1f - Mathf.Exp(-response * deltaTime) // Remain framerate independent
            );

            _state.Acceleration = (moveVelocity - currentVelocity) / deltaTime;
            currentVelocity = moveVelocity;

            float movedDistance = Vector3.Distance(transform.position, _lastFootStepPosition);

            if (movedDistance >= footStepDistance)
            {
                footStepSound.start();
                _lastFootStepPosition = transform.position;
            }
        }
        // Air movement
        else
        {
            _timeSinceUngrounded += deltaTime;

            if (_requestedMovement.sqrMagnitude > 0f)
            {
                // Calculate air movement based on the requested movement same as regular movement but using the Up
                var planarMovement = Vector3.ProjectOnPlane(_requestedMovement, motor.CharacterUp) * _requestedMovement.magnitude;
                var currentPlanarVelocity = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

                var movementForce = planarMovement * airAcceleration * deltaTime;

                var targetPlanarVelocity = currentPlanarVelocity + movementForce;
                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);
                movementForce = targetPlanarVelocity - currentPlanarVelocity;

                // ------------------- EXPERIMENTAL ------------------- //
                // Attempting to make jumping near steep slopes more stable
                if (motor.GroundingStatus.IsStableOnGround)
                {
                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                    {
                        var obstructionNormal = Vector3.Cross(motor.CharacterUp, Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal)).normalized;
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                    }
                }
                // currentVelocity += targetPlanarVelocity - currentPlanarVelocity;
                // ---------------------------------------------------- //

                currentVelocity += movementForce;
            }

            // Hold action results in larger jumps
            // Only edit gravity if the vertical speed is moving up
            // Otherwise fall at normal gravity
            var effectiveGravity = gravity;
            var verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (_requestedJumpSustain && verticalSpeed > 0f)
            {
                effectiveGravity *= jumpSustainGravity;
            }

            currentVelocity += deltaTime * effectiveGravity * motor.CharacterUp;
        }

        if (_requestedJump && allowJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTimeAfter && allowCoyoteAfter && !_ungroundedDueToJump;

            if (_state.Stance is Stance.Crouch)
            {
                _requestedJump = false;
                _requestedCrouch = false;
            }
            else if (grounded || canCoyoteJump)
            {
                _requestedJump = false;

                // Unstick player off ground
                motor.ForceUnground(time: 0f);
                _ungroundedDueToJump = true;

                // Get your current vertical speed based on your current "up" direction (allows for diagonal)
                // Don't slow down if you are already moving up at the same speed
                // Of course we can always limit when you can jump and prevent double jumping etc. it shouldn't be an issue
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);

                // Add enough vertical speed to reach the target vertical speed
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
            else
            {
                // Defer the jump request, jump can occur early or late
                _timeSinceJumpRequest += deltaTime;
                _requestedJump = allowCoyoteBefore && _timeSinceJumpRequest < coyoteTimeBefore;
            }
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        // Update the character's rotation to facce requested camera rotation
        // Project a vector pointing in the same direction that the player is looking to a flat plane
        var forward = Vector3.ProjectOnPlane(_requestedRotation * Vector3.forward, motor.CharacterUp);

        if (forward != Vector3.zero)
        {
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
        }
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        if (_requestedCrouch && _state.Stance is Stance.Stand)
        {
            _state.Stance = Stance.Crouch;
            motor.SetCapsuleDimensions(motor.Capsule.radius, crouchHeight, crouchHeight * 0.5f);
        }

        _tempState = _state;
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        if (!_requestedCrouch && _state.Stance is not Stance.Stand)
        {
            motor.SetCapsuleDimensions(motor.Capsule.radius, standHeight, standHeight * 0.5f);

            if (motor.CharacterOverlap(motor.TransientPosition, motor.TransientRotation, _uncrouchOverlapResults, motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
            {
                _requestedCrouch = true;
                motor.SetCapsuleDimensions(motor.Capsule.radius, crouchHeight, crouchHeight * 0.5f);
            }
            else
            {
                _state.Stance = Stance.Stand;
            }
        }

        _state.Grounded = motor.GroundingStatus.IsStableOnGround;
        _state.Velocity = motor.Velocity;

        _lastState = _tempState;
    }


    public bool IsColliderValidForCollisions(Collider coll) { return true; }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void PostGroundingUpdate(float deltaTime) { }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public Transform GetCameraTarget() => cameraTarget;
    public CharacterState GetState() => _state;
    public CharacterState GetLastState() => _lastState;

    public void SetPosition(Vector3 position, bool killVelocity = true)
    {
        motor.SetPosition(position);

        if (killVelocity)
        {
            motor.BaseVelocity = Vector3.zero;
        }
    }

    public Vector3 GetPosition()
    {
        return motor.TransientPosition;
    }
}

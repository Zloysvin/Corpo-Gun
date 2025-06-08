using KinematicCharacterController;
using UnityEngine;

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Move;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float walkSpeed = 20f;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;

    public void Initialize()
    {
        Debug.Log("Initializing PlayerCharacter");
        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.Rotation;

        // Convert to 3D space and clamp to prevent diagonal movement speed increase
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);

        // Oreint movement to look direction
        _requestedMovement = input.Rotation * _requestedMovement;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        var groundedMovement = motor.GetDirectionTangentToSurface(_requestedMovement, motor.GroundingStatus.GroundNormal) * _requestedMovement.magnitude; 

        currentVelocity = groundedMovement * walkSpeed;
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

    public void AfterCharacterUpdate(float deltaTime) { }

    public void BeforeCharacterUpdate(float deltaTime) { }

    public bool IsColliderValidForCollisions(Collider coll) { return true; }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void PostGroundingUpdate(float deltaTime) { }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public Transform GetCameraTarget() => cameraTarget;
}

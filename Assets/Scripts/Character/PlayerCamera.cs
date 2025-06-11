using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;

    private float _yaw;
    private float _pitch;

    public void Initialize(Transform target)
    {
        transform.position = target.position;

        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;
        
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    public void UpdateRotation(CameraInput input)
    {
        _yaw += input.Look.x * sensitivity;
        _pitch -= input.Look.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }
}

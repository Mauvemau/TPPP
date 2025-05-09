using System;
using UnityEngine;

/// <summary>
/// Controls the behaviour of the camera and it's position
/// </summary>
public class MyCamera : MonoBehaviour {
    [SerializeField] 
    private Transform target;
    [Header("Position")]
    [SerializeField]
    private Vector3 offset;
    [Header("Sensitivity")]
    [SerializeField] 
    private float horizontalSpeed = 1f;
    [SerializeField] 
    private float verticalSpeed = 1f;
    [SerializeField] 
    private float rotationSmoothness = 0.1f;
    [Header("Configuration")]
    [SerializeField]
    private float minClampAngle = -70f;
    [SerializeField]
    private float maxClampAngle = 80f;
    [SerializeField]
    public LayerMask collisionLayer;
    [SerializeField]
    private float minDistance = 1f;
    [SerializeField]
    private float maxDistance = 7f;
    
    private Vector2 _requestedRotationVelocity;
    private Vector2 _smoothedRotation; // We declare these here so that we don't do it in every late update loop
    private float _pitch = 0;
    private float _yaw = 0;

    public Vector3 GetCameraForward() {
        return transform.forward;
    }

    public Vector3 GetCameraRight() {
        return transform.right;
    }
    
    public void RequestRotation(Vector2 rotationDirection) {
        _requestedRotationVelocity = rotationDirection;
    }

    private Vector3 HandleCameraCollision(Vector3 desiredPosition) {
        Vector3 direction = (desiredPosition - target.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(target.position, direction, out hit, maxDistance, collisionLayer)) {
            float distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            return target.position + direction * distance;
        }
        
        return desiredPosition; // If no collision is detected we just return it as it is.
    }

    private Vector3 HandleCameraMovement() {
        _smoothedRotation = Vector2.Lerp(_smoothedRotation, _requestedRotationVelocity, rotationSmoothness);

        _yaw += _smoothedRotation.x * horizontalSpeed * Time.deltaTime;
        _pitch -= _smoothedRotation.y * verticalSpeed * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, minClampAngle, maxClampAngle);

        Quaternion desiredRotation = Quaternion.Euler(_pitch, _yaw, 0);
        return target.position + desiredRotation * offset;
    }

    private void LateUpdate() {
        if (!target) return;

        Vector3 desiredPosition = HandleCameraMovement();

        Vector3 finalPosition = HandleCameraCollision(desiredPosition);

        transform.position = finalPosition;
        transform.LookAt(target);
    }

    private void Awake() {
        if (target == null) {
            Debug.LogError("The camera needs to be assigned a target!");
        }
        if (horizontalSpeed == 0) {
            Debug.LogWarning("The horizontal speed for the camera is set to 0, the camera will not rotate horizontally!");
        }
        if (verticalSpeed == 0) {
            Debug.LogWarning("The vertical speed for the camera is set to 0, the camera will not rotate vertically!");
        }
        if (collisionLayer.value == 0) {
            Debug.LogWarning("Collision layer for the camera is not configured.");
        }
    }
}

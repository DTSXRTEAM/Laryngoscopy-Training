using UnityEngine;

public class SmoothGimbalCamera : MonoBehaviour
{
    public Transform target;   // XR Camera
    public float positionSmooth = 5f;
    public float rotationSmooth = 5f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            target.position,
            ref velocity,
            1f / positionSmooth
        );

        // Smooth rotation
        Quaternion targetRot = Quaternion.Slerp(
            transform.rotation,
            target.rotation,
            Time.deltaTime * rotationSmooth
        );

        transform.rotation = targetRot;
    }

}

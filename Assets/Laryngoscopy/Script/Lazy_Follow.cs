using UnityEngine;

public class Lazy_Follow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, 0.5f);

    [Header("Movement")]
    public float followSpeed = 6f;

    [Header("Angle Delay System")]
    public float allowedAngle = 20f;
    public float delaySeconds = 5f;

    [Header("Pin Settings")]
    public bool isPinned = false;
    public bool followRotationWhilePinned = true;

    float timer = 0f;
    bool waiting = false;

    Vector3 delayedPosition;
    Quaternion delayedRotation;

    void Start()
    {
        if (target == null && Camera.main != null)
            target = Camera.main.transform;

        delayedPosition = transform.position;
        delayedRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Desired position
        Vector3 newPos = target.position + target.TransformVector(offset);
        newPos.y = transform.position.y;

        // Desired rotation
        Quaternion newRot = Quaternion.Euler(0f, target.eulerAngles.y, 0f);

        //--------------------------------------------------
        // PINNED
        //--------------------------------------------------
        if (isPinned)
        {
            if (followRotationWhilePinned)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    newRot,
                    Time.deltaTime * followSpeed);
            }

            // Position stays fixed
            return;
        }

        //--------------------------------------------------
        // NORMAL LAZY FOLLOW
        //--------------------------------------------------

        float angle = Vector3.Angle(transform.forward, target.forward);

        // Camera inside allowed angle
        if (angle < allowedAngle)
        {
            waiting = false;
            timer = 0f;

            transform.position = Vector3.Lerp(
                transform.position,
                delayedPosition,
                Time.deltaTime * followSpeed);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                delayedRotation,
                Time.deltaTime * followSpeed);

            return;
        }

        // Camera outside allowed angle
        if (!waiting)
        {
            waiting = true;
            timer = 0f;
        }

        timer += Time.deltaTime;

        if (timer >= delaySeconds)
        {
            delayedPosition = newPos;
            delayedRotation = newRot;

            waiting = false;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            delayedPosition,
            Time.deltaTime * followSpeed);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            delayedRotation,
            Time.deltaTime * followSpeed);
    }

    //====================================================
    // PUSH PIN
    //====================================================

    public void PinPanel()
    {
        isPinned = true;
    }

    public void UnPinPanel()
    {
        isPinned = false;

        // Resume follow from current target
        delayedPosition = target.position + target.TransformVector(offset);
        delayedPosition.y = transform.position.y;

        delayedRotation = Quaternion.Euler(0f, target.eulerAngles.y, 0f);

        waiting = false;
        timer = 0f;
    }

    public void TogglePin()
    {
        if (isPinned)
        {
            UnPinPanel();
        }
        else
        {
            PinPanel();
        }
    }
}
using UnityEngine;
using Unity.XR.CoreUtils;

public class QRTrackingManager : MonoBehaviour
{
    public GameObject qrPrefab;

    private XROrigin xrOrigin;

    private void Start()
    {
        xrOrigin = FindObjectOfType<XROrigin>();

        // Example fake spawn for testing
        SpawnQRObject(
            new Vector3(0, 0, 2),
            Quaternion.identity
        );
    }

    public void SpawnQRObject(Vector3 localPos, Quaternion localRot)
    {
        Vector3 worldPos =
            xrOrigin.transform.TransformPoint(localPos);

        Quaternion worldRot =
            xrOrigin.transform.rotation * localRot;

        Instantiate(
            qrPrefab,
            worldPos,
            worldRot
        );
    }
}
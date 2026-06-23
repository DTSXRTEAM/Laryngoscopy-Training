using System.Collections;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class QR_Manager : MonoBehaviour
{
    [SerializeField]
    private MRUK _mrukInstance;

    [SerializeField]
    private GameObject _qrPrefab;

    [Header("Animation Controller")]
    [SerializeField]
    private HeadingAnimationController animationController;

    private bool hasSpawned = false;

    // Runtime spawned manikin/prefab
    private GameObject spawnedObject;

    private void OnEnable()
    {
        if (_mrukInstance == null)
        {
            Debug.Log("MRUK Missing");
            return;
        }

        _mrukInstance.SceneSettings
            .TrackableAdded
            .AddListener(OnTrackableAdded);
    }

    private void OnDisable()
    {
        if (_mrukInstance != null)
        {
            _mrukInstance.SceneSettings
                .TrackableAdded
                .RemoveListener(OnTrackableAdded);
        }
    }

    private void OnTrackableAdded(MRUKTrackable trackable)
    {
        // Already spawned
        if (hasSpawned)
            return;

        // Only QR codes
        if (trackable.TrackableType !=
            OVRAnchor.TrackableType.QRCode)
        {
            return;
        }

        hasSpawned = true;

        // Spawn prefab at QR
        spawnedObject =
            Instantiate(
                _qrPrefab,
                trackable.transform
            );

        // Get runtime animator
        Animator runtimeAnimator =
            spawnedObject.GetComponentInChildren<Animator>();

        // Assign runtime animator
        if (animationController != null &&
            runtimeAnimator != null)
        {
            animationController.SetPrefabAnimator(
                runtimeAnimator
            );
        }

        StartCoroutine(
            DetachAndDisableTracking(
                spawnedObject
            )
        );

        Debug.Log("QR Spawned Correctly");
    }

    IEnumerator DetachAndDisableTracking(
        GameObject spawnedObject)
    {
        yield return null;

        // Keep world position
        spawnedObject.transform.SetParent(
            null,
            true
        );

        // Disable QR tracking
        var config =
            _mrukInstance.SceneSettings
            .TrackerConfiguration;

        config.QRCodeTrackingEnabled = false;

        _mrukInstance.SceneSettings
            .TrackerConfiguration = config;

        Debug.Log(
            "Tracking Disabled And Object Fixed"
        );
    }

    // Call this from ExitTraining()
    public void ResetQR()
    {
        hasSpawned = false;

        // Remove spawned manikin
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }

        // Enable QR scanning again
        var config =
            _mrukInstance.SceneSettings
            .TrackerConfiguration;

        config.QRCodeTrackingEnabled = true;

        _mrukInstance.SceneSettings
            .TrackerConfiguration = config;

        Debug.Log("QR Reset Complete");
    }
}
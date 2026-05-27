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

    private void OnTrackableAdded(
        MRUKTrackable trackable)
    {
        // ONLY ONE SPAWN
        if (hasSpawned)
        {
            return;
        }

        // ONLY QR
        if (trackable.TrackableType !=
            OVRAnchor.TrackableType.QRCode)
        {
            return;
        }

        hasSpawned = true;

        // SPAWN WITH QR PARENT
        GameObject spawnedObject =
            Instantiate(
                _qrPrefab,
                trackable.transform
            );

        // GET RUNTIME ANIMATOR
        Animator runtimeAnimator =
            spawnedObject
            .GetComponentInChildren<Animator>();

        // ASSIGN ANIMATOR
        if (animationController != null &&
            runtimeAnimator != null)
        {
            animationController
                .SetPrefabAnimator(
                    runtimeAnimator
                );
        }

        // DETACH AFTER 1 FRAME
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
        // WAIT FOR MRUK ALIGNMENT
        yield return null;

        // KEEP WORLD POSITION
        spawnedObject.transform.SetParent(
            null,
            true
        );

        // DISABLE QR TRACKING
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
}
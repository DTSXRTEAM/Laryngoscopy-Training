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
        if (trackable.TrackableType !=
            OVRAnchor.TrackableType.QRCode)
        {
            return;
        }

        // CORRECT MRUK SPAWN
        GameObject spawnedObject =
            Instantiate(
                _qrPrefab,
                trackable.transform
            );

        // GET RUNTIME ANIMATOR
        Animator runtimeAnimator =
            spawnedObject
            .GetComponentInChildren<Animator>();

        // ASSIGN RUNTIME ANIMATOR
        if (animationController != null &&
            runtimeAnimator != null)
        {
            animationController
                .SetPrefabAnimator(
                    runtimeAnimator
                );
        }

        Debug.Log("QR Spawned");
    }
}
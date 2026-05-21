using Meta.XR.MRUtilityKit;
using UnityEngine;

public class QR_Manager : MonoBehaviour
{
    [SerializeField] private MRUK _mrukInstance;

    [SerializeField] private GameObject humanPrefab;

    // Assign chest point from prefab
    [SerializeField] private Transform chestPointPrefab;

    private void OnEnable()
    {
        _mrukInstance.SceneSettings.TrackableAdded.AddListener(OnTrackableAdded);
    }

    private void OnDisable()
    {
        _mrukInstance.SceneSettings.TrackableAdded.RemoveListener(OnTrackableAdded);
    }

    private void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode)
            return;

        // Spawn human
        GameObject human = Instantiate(humanPrefab);

        // Find chest point inside spawned object
        Transform chest = human.transform.Find("ChestPoint");

        // Calculate offset
        Vector3 offset = human.transform.position - chest.position;

        // Move human so chest aligns with QR
        human.transform.position =
            trackable.transform.position + offset;

        // Optional rotation
        human.transform.rotation = trackable.transform.rotation;

        Debug.Log("Human chest aligned to QR");
    }
}
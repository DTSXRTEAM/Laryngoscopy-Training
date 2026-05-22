using Meta.XR.MRUtilityKit;
using System.Linq;
using UnityEngine;
public class QR_Manager : MonoBehaviour

{
    [SerializeField]
    private MRUK _mrukInstance;
    [SerializeField]
    private GameObject _qrPrefab;

    private static QR_Manager s_instance;

    public static bool TrackingEnabled
    {
        get => s_instance &&
               s_instance._mrukInstance &&
               s_instance._mrukInstance.SceneSettings.TrackerConfiguration.QRCodeTrackingEnabled;

        set
        {
            if (!s_instance || !s_instance._mrukInstance)
            {
                return;
            }

            var config = s_instance._mrukInstance.SceneSettings.TrackerConfiguration;
            config.QRCodeTrackingEnabled = value;
            s_instance._mrukInstance.SceneSettings.TrackerConfiguration = config;
        }
    }

    private void OnEnable()
    {
        s_instance = this;

        if (!_mrukInstance)
        {
            Debug.Log($"{nameof(QR_Manager)} requires an MRUK object in the scene!");
            return;
        }

        _mrukInstance.SceneSettings.TrackableAdded.AddListener(OnTrackableAdded);
        _mrukInstance.SceneSettings.TrackableRemoved.AddListener(OnTrackableRemoved);
    }

    private void OnDisable()
    {
        if (_mrukInstance)
        {
            _mrukInstance.SceneSettings.TrackableAdded.RemoveListener(OnTrackableAdded);
            _mrukInstance.SceneSettings.TrackableRemoved.RemoveListener(OnTrackableRemoved);
        }

        if (s_instance == this)
        {
            s_instance = null;
        }
    }

    private void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode)
        {
            return;
        }
        var instance = Instantiate(_qrPrefab,trackable.transform);

        Debug.Log("<<< QRCode detected! >>>" + nameof(OnTrackableAdded));

        Debug.Log($"<<< QRCode ID: {trackable.name}, Position: {trackable.transform.position}, Rotation: {trackable.transform.rotation} >>>");

        Debug.Log("<<< 2D Bounding Box points: " + trackable.PlaneRect + " >>>");

        Debug.Log("<<< 2D Polygon points: " + trackable.PlaneBoundary2D + " >>>");

        if (trackable.MarkerPayloadString is { } str)
        {
            Debug.Log("<<< Payload is a string: " + str + " >>>");
        }
        else if (trackable.MarkerPayloadBytes is { } bytes)
        {
            Debug.Log($"Binary(data=[{string.Join(", ", bytes.Take(16).Select(b => $"{b:x02}"))}]" +
                      $"{(bytes.Length > 16 ? "..." : "")})");
        }
        else
        {
            Debug.Log("<<<< No payload >>>>");
        }
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode)
        {
            return;
        }

        Debug.Log("<<< QRCode removed >>>");
    }
}

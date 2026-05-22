using UnityEngine;

public class OriginSetter : MonoBehaviour
{
    public GameObject xrOrigin;

    [Header("UI")]
    public GameObject startPanel;     // Set Origin Panel
    public GameObject mainPanel;      // Your actual UI

    void Start()
    {
        // First show only Set Origin panel
        startPanel.SetActive(true);

        // Hide main UI initially
        mainPanel.SetActive(false);
    }

    public void SetOrigin()
    {
        // Reset XR Origin
        xrOrigin.transform.position = Vector3.zero;
        xrOrigin.transform.rotation = Quaternion.identity;

        // Hide start panel
        startPanel.SetActive(false);

        // Show actual UI
        mainPanel.SetActive(true);
    }
}
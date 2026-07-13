using UnityEngine;

public class ThreeDNextButton : MonoBehaviour
{
    private UIPanelSequence uiPanelSequence;

    private void Awake()
    {
        uiPanelSequence = FindFirstObjectByType<UIPanelSequence>();

        // If using an older Unity version:
        // uiPanelSequence = FindObjectOfType<UIPanelSequence>();
    }

    public void OnButtonPressed()
    {
        if (uiPanelSequence != null)
        {
            uiPanelSequence.NextPage();
        }
        else
        {
            Debug.LogError("UIPanelSequence not found in the scene.");
        }
    }
}
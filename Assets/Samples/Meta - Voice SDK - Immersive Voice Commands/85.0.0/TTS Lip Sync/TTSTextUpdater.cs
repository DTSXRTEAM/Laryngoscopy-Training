using UnityEngine;
using UnityEngine.UI;
using Meta.WitAi.TTS.Utilities;

public class TTSTextUpdater : MonoBehaviour
{
    public TTSSpeaker speaker;

    // Your UI text field
    public Text textField;

    void OnEnable()
    {
        if (speaker != null)
        {
            speaker.Events.OnTextPlaybackStart.AddListener(UpdateText);
        }
    }

    void OnDisable()
    {
        if (speaker != null)
        {
            speaker.Events.OnTextPlaybackStart.RemoveListener(UpdateText);
        }
    }

    void UpdateText(string text)
    {
        // This text is exactly what TTSSpeaker is speaking
        textField.text = text;
    }
}
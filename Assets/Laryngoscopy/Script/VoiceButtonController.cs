using Meta.WitAi;
using Meta.WitAi.Json;
using Oculus.Voice;
using UnityEngine;
using UnityEngine.UI;

public class VoiceButtonController : MonoBehaviour
{
    public AppVoiceExperience voiceExperience;
    public UIPanelSequence uiPanelSequence;

    private void Start()
    {
        if (voiceExperience == null)
        {
            Debug.LogError("Voice Experience Missing");
            return;
        }

        if (uiPanelSequence == null)
        {
            Debug.LogError("UIPanelSequence Missing");
            return;
        }

        voiceExperience.VoiceEvents.OnResponse.AddListener(OnResponse);

        Invoke(nameof(StartListening), 2f);
    }

    void StartListening()
    {
        if (!voiceExperience.Active)
        {
            voiceExperience.Activate();
        }
    }

    private void OnResponse(WitResponseNode response)
    {
        string spokenText = response["text"];

        if (string.IsNullOrEmpty(spokenText))
        {
            RestartListening();
            return;
        }

        spokenText = spokenText.ToLower();

        Debug.Log("Voice Command : " + spokenText);

        // NEXT
        if (spokenText.Contains("next step"))
        {
            Debug.Log("Next Page");

            uiPanelSequence.NextPage();
        }

        // BACK
        else if (spokenText.Contains("go back"))
        {
            Debug.Log("Previous Page");

            uiPanelSequence.PreviousPage();
        }

        RestartListening();
    }

    void RestartListening()
    {
        Invoke(nameof(StartListening), 0.5f);
    }

    private void OnDestroy()
    {
        if (voiceExperience != null)
        {
            voiceExperience.VoiceEvents.OnResponse.RemoveListener(OnResponse);
        }
    }
}
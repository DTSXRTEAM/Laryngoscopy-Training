using Meta.WitAi;
using Meta.WitAi.Json;
using Oculus.Voice;
using UnityEngine;
using UnityEngine.UI;

public class VoiceButtonController : MonoBehaviour
{
    public AppVoiceExperience voiceExperience;

    public Button button1;

    void Start()
    {
        voiceExperience.VoiceEvents.OnResponse.AddListener(OnResponse);

        voiceExperience.ActivateImmediately();
    }

    void OnResponse(WitResponseNode response)
    {
        Debug.Log(response.ToString());

        if (response["intents"].Count > 0)
        {
            string intent = response["intents"][0]["name"];

            Debug.Log("Detected Intent : " + intent);

            if (intent == "selectbutton")
            {
                Debug.Log("BUTTON CLICKED");

                button1.onClick.Invoke();
            }
        }
        else
        {
            Debug.Log("NO INTENT DETECTED");
        }
    }
}
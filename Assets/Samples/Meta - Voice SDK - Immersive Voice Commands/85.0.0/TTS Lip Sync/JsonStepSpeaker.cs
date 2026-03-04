using Meta.WitAi.TTS.Utilities;
using Meta.WitAi.TTS.UX;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JsonStepSpeaker : MonoBehaviour
{
    public TTSSpeaker speaker;
    public TTSSpeakerInput ui;     // <--- reference to the above script
    public TextAsset jsonFile;

    private List<string> steps = new List<string>();
    private int currentStep = 0;

    void Start()
    {
        LoadSteps();
    }

    void LoadSteps()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON file missing");
            return;
        }

        StepContainer container = JsonUtility.FromJson<StepContainer>(jsonFile.text);
        if (container?.steps == null || container.steps.Length == 0)
        {
            Debug.LogWarning("No steps found in JSON");
            return;
        }

        for (int i = 0; i < container.steps.Length; i++)
        {
            steps.Add(container.steps[i].text);
        }
    }

    // Call this from your UI button (e.g., "Next" or "Speak")
    public void SpeakNext()
    {
        if (currentStep >= steps.Count)
        {
            Debug.Log("All steps completed");
            return;
        }

        string line = steps[currentStep];

        // Update the same Input Field so the user sees what will be spoken
        if (ui != null)
        {
            ui.SetInputText(line);
        }

        // Speak the step
        if (speaker != null)
        {
            speaker.Speak(line);
        }

        currentStep++;
    }

    public void ResetSteps()
    {
        currentStep = 0;

        // Optionally also clear the input field
        if (ui != null)
            ui.SetInputText(string.Empty);
    }
}

[Serializable]
public class StepContainer
{
    public Step[] steps;
}

[Serializable]
public class Step
{
    public string text;
}

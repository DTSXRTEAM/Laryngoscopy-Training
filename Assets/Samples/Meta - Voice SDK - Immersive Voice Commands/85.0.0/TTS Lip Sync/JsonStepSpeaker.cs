using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.WitAi.TTS.Utilities;

public class JsonStepSpeaker : MonoBehaviour
{
    public TTSSpeaker speaker;
    public TextAsset jsonFile;

    public float firstSentenceDelay = 3f;   // Delay before first sentence

    private List<string> steps = new List<string>();
    private int currentStep = 0;

    void Start()
    {
        LoadSteps();
        StartCoroutine(SpeakFirstSentence());
    }

    IEnumerator SpeakFirstSentence()
    {
        yield return new WaitForSeconds(firstSentenceDelay);
        SpeakNext();
    }

    void LoadSteps()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON file missing");
            return;
        }

        StepContainer container = JsonUtility.FromJson<StepContainer>(jsonFile.text);

        for (int i = 0; i < container.steps.Length; i++)
        {
            steps.Add(container.steps[i].text);
        }
    }

    public void SpeakNext()
    {
        if (currentStep >= steps.Count)
        {
            Debug.Log("All steps completed");
            return;
        }

        speaker.Speak(steps[currentStep]);
        currentStep++;
    }

    public void ResetSteps()
    {
        currentStep = 0;
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
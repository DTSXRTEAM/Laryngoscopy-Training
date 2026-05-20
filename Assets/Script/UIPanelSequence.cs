using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Meta.WitAi.TTS.Utilities;

public class UIPanelSequence : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text headingText;
    public TMP_Text contentText;
    public TMP_Text nextButtonText;

    [Header("TTS")]
    public TTSSpeaker speaker;

    [Header("JSON")]
    public TextAsset jsonFile;

    [Serializable]
    public class StepData
    {
        public string heading;
        public string content;
        public string buttonText;
        public string tts;
    }

    [Serializable]
    public class StepContainer
    {
        public StepData[] steps;
    }

    private List<StepData> steps = new List<StepData>();

    private int currentIndex = 0;

    void Start()
    {
        LoadJson();

        if (steps.Count > 0)
        {
            ShowStep();
        }
    }

    void LoadJson()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON File Missing");
            return;
        }

        try
        {
            StepContainer container =
                JsonUtility.FromJson<StepContainer>(jsonFile.text);

            if (container == null || container.steps == null)
            {
                Debug.LogError("JSON Parsing Failed");
                return;
            }

            steps.Clear();

            foreach (StepData step in container.steps)
            {
                steps.Add(step);
            }

            Debug.Log("JSON Loaded Successfully");
            Debug.Log("Total Steps : " + steps.Count);
        }
        catch (Exception e)
        {
            Debug.LogError("JSON Error : " + e.Message);
        }
    }

    public void NextPage()
    {
        if (currentIndex < steps.Count - 1)
        {
            currentIndex++;
            ShowStep();
        }
    }

    public void PreviousPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowStep();
        }
    }

    void ShowStep()
    {
        if (steps.Count == 0)
            return;

        headingText.text = steps[currentIndex].heading;
        contentText.text = steps[currentIndex].content;
        nextButtonText.text = steps[currentIndex].buttonText;

        Debug.Log("Showing Step : " + currentIndex);

        if (speaker != null)
        {
            speaker.Stop();

            if (!string.IsNullOrEmpty(steps[currentIndex].tts))
            {
                speaker.Speak(steps[currentIndex].tts);
            }
        }
    }
}
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

    [System.Serializable]
    public class StepData
    {
        public string heading;

        [TextArea(3, 5)]
        public string content;

        public string buttonText;

        public string tts;
    }

    [System.Serializable]
    public class StepContainer
    {
        public StepData[] steps;
    }

    private List<StepData> steps = new List<StepData>();

    private int currentIndex = 0;

    void Start()
    {
        LoadJson();
        ShowStep();
    }

    void LoadJson()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON File Missing");
            return;
        }

        StepContainer container =
            JsonUtility.FromJson<StepContainer>(jsonFile.text);

        steps.Clear();

        for (int i = 0; i < container.steps.Length; i++)
        {
            steps.Add(container.steps[i]);
        }
    }

    // NEXT BUTTON
    public void NextPage()
    {
        if (currentIndex < steps.Count - 1)
        {
            currentIndex++;
            ShowStep();
        }
    }

    // BACK BUTTON
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
        headingText.text = steps[currentIndex].heading;
        contentText.text = steps[currentIndex].content;
        nextButtonText.text = steps[currentIndex].buttonText;

        // STOP PREVIOUS AUDIO
        speaker.Stop();

        // PLAY CURRENT STEP AUDIO
        speaker.Speak(steps[currentIndex].tts);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Meta.WitAi.TTS.Utilities;

public class UIPanelSequence : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text headingText;
    public TMP_Text contentText;
    public TMP_Text nextButtonText;

    [Header("Checklist")]
    public Transform checklistParent;
    public GameObject checkboxPrefab;

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

        public string[] checkList;
    }

    [Serializable]
    public class StepContainer
    {
        public StepData[] steps;
    }

    private List<StepData> steps = new List<StepData>();

    int currentIndex = 0;

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
        StepContainer container =
            JsonUtility.FromJson<StepContainer>(jsonFile.text);

        steps.Clear();

        foreach (StepData step in container.steps)
        {
            steps.Add(step);
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
        StepData step = steps[currentIndex];

        headingText.text = step.heading;
        contentText.text = step.content;
        nextButtonText.text = step.buttonText;

        GenerateChecklist(step);

        if (speaker != null)
        {
            speaker.Stop();

            if (!string.IsNullOrEmpty(step.tts))
            {
                speaker.Speak(step.tts);
            }
        }
    }

    void GenerateChecklist(StepData step)
    {
        // CLEAR OLD CHECKBOXES
        for (int i = 0; i < checklistParent.childCount; i++)
        {
            Destroy(checklistParent.GetChild(i).gameObject);
        }

        // CREATE NEW CHECKBOXES
        if (step.checkList == null)
            return;

        for (int i = 0; i < step.checkList.Length; i++)
        {
            GameObject obj =
                Instantiate(checkboxPrefab, checklistParent);

            TMP_Text txt =
                obj.GetComponentInChildren<TMP_Text>();

            txt.text = step.checkList[i];
        }
    }
}
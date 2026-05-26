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
    public Transform checklistParent2;

    public GameObject checkboxPrefab;
    public GameObject checkboxPrefab2;

    [Header("Animation")]
    public HeadingAnimationController animationController;

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

        // UI TEXT
        headingText.text = step.heading;
        contentText.text = step.content;
        nextButtonText.text = step.buttonText;

        // PLAY ANIMATION BASED ON HEADING
        if (animationController != null)
        {
            animationController.PlayAnimation(step.heading);
        }

        // GENERATE CHECKLIST
        GenerateChecklist(step.checkList);

        // TTS
        if (speaker != null)
        {
            speaker.Stop();

            if (!string.IsNullOrEmpty(step.tts))
            {
                speaker.Speak(step.tts);
            }
        }
    }

    void GenerateChecklist(string[] list)
    {
        // CLEAR FIRST PARENT
        for (int i = 0; i < checklistParent.childCount; i++)
        {
            Destroy(checklistParent.GetChild(i).gameObject);
        }

        // CLEAR SECOND PARENT
        for (int i = 0; i < checklistParent2.childCount; i++)
        {
            Destroy(checklistParent2.GetChild(i).gameObject);
        }

        if (list == null)
            return;

        for (int i = 0; i < list.Length; i++)
        {
            Transform targetParent;
            GameObject targetPrefab;

            // FIRST 4 ITEMS
            if (i < 4)
            {
                targetParent = checklistParent;
                targetPrefab = checkboxPrefab;
            }
            // REMAINING ITEMS
            else
            {
                targetParent = checklistParent2;
                targetPrefab = checkboxPrefab2;
            }

            GameObject obj =
                Instantiate(targetPrefab, targetParent);

            TMP_Text txt =
                obj.GetComponentInChildren<TMP_Text>();

            txt.text = list[i];
        }
    }
}
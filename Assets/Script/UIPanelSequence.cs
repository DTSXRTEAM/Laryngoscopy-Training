using System;
using System.Collections;
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
    public TMP_Text backButtonText;

    [Header("Models")]
    public ModelManager modelManager;

    [Header("Buttons")]
    public Button nextButton;
    public Button backButton;
    public Button replayButton;

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
        public string backButtonText;
        public string tts;
        public string[] checkList;
        public bool showNextButton = true;
        public bool showBackButton = true;
        public ModelData[] models;
    }

    [Serializable]
    public class StepContainer
    {
        public StepData[] steps;
    }

    private List<StepData> steps = new List<StepData>();
    private int currentIndex = 0;
    private int totalChecklistItems;
    private string currentTTS = "";

    void Start()
    {
        LoadJson();
        if (steps.Count > 0)
        {
            ShowStep(true);
        }
    }

    void LoadJson()
    {
        StepContainer container = JsonUtility.FromJson<StepContainer>(jsonFile.text);
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
            ShowStep(true);
        }
    }

    public void PreviousPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            StepData step = steps[currentIndex];
            ShowStep(false);

            if (animationController != null)
            {
                animationController.SetAnimationToLastFrame(step.heading);
            }
        }
    }

    void ShowStep(bool playAnimation = true)
    {
        StepData step = steps[currentIndex];

        headingText.text = step.heading;
        contentText.text = step.content;
        nextButtonText.text = step.buttonText;

        if (backButtonText != null)
        {
            backButtonText.text = string.IsNullOrEmpty(step.backButtonText) ? "Back" : step.backButtonText;
        }

        if (nextButton != null)
            nextButton.gameObject.SetActive(step.showNextButton);

        if (backButton != null)
            backButton.gameObject.SetActive(step.showBackButton);

        if (animationController != null && playAnimation)
        {
            animationController.PlayAnimation(step.heading);
        }

        GenerateChecklist(step.checkList);

        currentTTS = step.tts;

        PlayTTS(currentTTS);

        if (replayButton != null)
        {
            replayButton.interactable = !string.IsNullOrEmpty(currentTTS);
        }

        if (modelManager != null && step.models != null)
        {
            modelManager.ShowModels(step.models);
        }
    }

    private void PlayTTS(string text)
    {
        if (speaker == null || string.IsNullOrEmpty(text)) return;

        // Stop any ongoing playback before speaking again
        speaker.Stop();
        // Use SpeakQueued to ensure smooth playback
        speaker.SpeakQueued(text);
    }

    public void ReplayTTS()
    {
        StepData step = steps[currentIndex];
        if (animationController != null)
        {
            animationController.PlayAnimation(step.heading);
        }
        PlayTTS(currentTTS);
    }

    void GenerateChecklist(string[] list)
    {
        for (int i = checklistParent.childCount - 1; i >= 0; i--)
        {
            Destroy(checklistParent.GetChild(i).gameObject);
        }

        for (int i = checklistParent2.childCount - 1; i >= 0; i--)
        {
            Destroy(checklistParent2.GetChild(i).gameObject);
        }

        if (list == null || list.Length == 0)
        {
            checklistParent.gameObject.SetActive(false);
            checklistParent2.gameObject.SetActive(false);
            nextButton.interactable = true;
            return;
        }

        totalChecklistItems = list.Length;

        if (list.Length <= 2)
        {
            checklistParent.gameObject.SetActive(true);
            checklistParent2.gameObject.SetActive(false);

            for (int i = 0; i < list.Length; i++)
            {
                GameObject obj = Instantiate(checkboxPrefab, checklistParent);
                TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = list[i];

                Toggle toggle = obj.GetComponentInChildren<Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = false;
                    toggle.onValueChanged.AddListener(delegate { CheckChecklistCompleted(); });
                }
            }
        }
        else
        {
            checklistParent.gameObject.SetActive(false);
            checklistParent2.gameObject.SetActive(true);

            for (int i = 0; i < list.Length; i++)
            {
                GameObject obj = Instantiate(checkboxPrefab2, checklistParent2);
                TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = list[i];

                Toggle toggle = obj.GetComponentInChildren<Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = false;
                    toggle.onValueChanged.AddListener(delegate { CheckChecklistCompleted(); });
                }
            }
        }

        nextButton.interactable = false;
        CheckChecklistCompleted();
    }

    private void CheckChecklistCompleted()
    {
        int checkedItems = 0;

        Toggle[] toggles1 = checklistParent.GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles1)
        {
            if (toggle.isOn) checkedItems++;
        }

        Toggle[] toggles2 = checklistParent2.GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles2)
        {
            if (toggle.isOn) checkedItems++;
        }

        Debug.Log("Checked: " + checkedItems + " / " + totalChecklistItems);
        nextButton.interactable = checkedItems >= totalChecklistItems;
    }
}

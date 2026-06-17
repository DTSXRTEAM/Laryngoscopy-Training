using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Meta.WitAi.TTS.Utilities;
using UnityEngine.Video;

public class UIPanelSequence : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text headingText;
    public TMP_Text contentText;
    public TMP_Text nextButtonText;
    public TMP_Text backButtonText;

    [Header("Alternate UI for Step 3")]
    public TMP_Text contentText2;             // extra text transform
    public Transform checklistParent3;        // extra checklist parent
    public GameObject checkboxPrefab3;        // prefab for step 3 checklist

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

    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Serializable]
    public class StepData
    {
        public int index;
        public string heading;
        public string content;
        public string buttonText;
        public string backButtonText;
        public string tts;
        public string[] checkList;
        public bool showNextButton = true;
        public bool showBackButton = true;
        public int nextIndex = -1;
        public int backIndex = -1;
        public ModelData[] models;

        // flags for alternate UI
        public bool useAlternateContent = false;
        public bool useAlternateChecklist = false;

        public string videoName;
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
        StepData step = steps[currentIndex];
        if (step.nextIndex != -1 && step.nextIndex < steps.Count)
        {
            currentIndex = step.nextIndex;
            ShowStep(true);
        }
    }

    public void PreviousPage()
    {
        StepData step = steps[currentIndex];
        if (step.backIndex != -1 && step.backIndex >= 0)
        {
            currentIndex = step.backIndex;
            StepData previousStep = steps[currentIndex];
            ShowStep(false);

            if (animationController != null)
            {
                if (!string.IsNullOrEmpty(step.backButtonText) &&
                    step.backButtonText.Equals("Incorrect", StringComparison.OrdinalIgnoreCase))
                {
                    animationController.PlayAnimation(previousStep.index);
                }
                else
                {
                    animationController.SetAnimationToLastFrame(previousStep.index);
                }
            }
        }
    }

    void ShowStep(bool playAnimation = true)
    {
        StepData step = steps[currentIndex];

        headingText.text = step.heading;

        // use alternate content for index 3
        if (step.useAlternateContent && contentText2 != null)
        {
            contentText.gameObject.SetActive(false);
            contentText2.gameObject.SetActive(true);
            contentText2.text = step.content;
        }
        else
        {
            contentText.gameObject.SetActive(true);
            if (contentText2 != null) contentText2.gameObject.SetActive(false);
            contentText.text = step.content;
        }

        nextButtonText.text = step.buttonText;
        if (backButtonText != null)
            backButtonText.text = string.IsNullOrEmpty(step.backButtonText) ? "Back" : step.backButtonText;

        if (nextButton != null)
            nextButton.gameObject.SetActive(step.showNextButton);
        if (backButton != null)
            backButton.gameObject.SetActive(step.showBackButton);

        if (animationController != null && playAnimation)
            animationController.PlayAnimation(step.index);

        GenerateChecklist(step);

        currentTTS = step.tts;
        PlayTTS(currentTTS);

        if (replayButton != null)
            replayButton.interactable = !string.IsNullOrEmpty(currentTTS);

        if (modelManager != null && step.models != null)
            modelManager.ShowModels(step.models);

        PlayVideo(step.videoName);
    }

    private void PlayTTS(string text)
    {
        if (speaker == null || string.IsNullOrEmpty(text)) return;
        speaker.Stop();
        speaker.SpeakQueued(text);
    }

    public void ReplayTTS()
    {
        StepData step = steps[currentIndex];
        if (animationController != null)
            animationController.PlayAnimation(step.index);
        PlayTTS(currentTTS);
    }

    void GenerateChecklist(StepData step)
    {
        // clear all parents
        foreach (Transform child in checklistParent) Destroy(child.gameObject);
        foreach (Transform child in checklistParent2) Destroy(child.gameObject);
        if (checklistParent3 != null)
            foreach (Transform child in checklistParent3) Destroy(child.gameObject);

        if (step.checkList == null || step.checkList.Length == 0)
        {
            checklistParent.gameObject.SetActive(false);
            checklistParent2.gameObject.SetActive(false);
            if (checklistParent3 != null) checklistParent3.gameObject.SetActive(false);
            nextButton.interactable = true;
            return;
        }

        totalChecklistItems = step.checkList.Length;
        nextButton.interactable = false;

        // alternate checklist for index 3
        if (step.useAlternateChecklist && checklistParent3 != null)
        {
            checklistParent.gameObject.SetActive(false);
            checklistParent2.gameObject.SetActive(false);
            checklistParent3.gameObject.SetActive(true);

            foreach (string item in step.checkList)
            {
                GameObject obj = Instantiate(checkboxPrefab3, checklistParent3);
                TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = item;

                Toggle toggle = obj.GetComponentInChildren<Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = false;
                    toggle.onValueChanged.AddListener(delegate { CheckChecklistCompleted(); });
                }
            }
        }
        else if (step.checkList.Length <= 2)
        {
            checklistParent.gameObject.SetActive(true);
            checklistParent2.gameObject.SetActive(false);
            if (checklistParent3 != null) checklistParent3.gameObject.SetActive(false);

            foreach (string item in step.checkList)
            {
                GameObject obj = Instantiate(checkboxPrefab, checklistParent);
                TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = item;

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
            if (checklistParent3 != null) checklistParent3.gameObject.SetActive(false);

            foreach (string item in step.checkList)
            {
                GameObject obj = Instantiate(checkboxPrefab2, checklistParent2);
                TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = item;

                Toggle toggle = obj.GetComponentInChildren<Toggle>();
                if (toggle != null)
                {
                    toggle.isOn = false;
                    toggle.onValueChanged.AddListener(delegate { CheckChecklistCompleted(); });
                }
            }
        }
    }

    private void CheckChecklistCompleted()
    {
        int checkedItems = 0;

        Toggle[] toggles1 = checklistParent.GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles1) if (toggle.isOn) checkedItems++;

        Toggle[] toggles2 = checklistParent2.GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles2) if (toggle.isOn) checkedItems++;

        if (checklistParent3 != null)
        {
            Toggle[] toggles3 = checklistParent3.GetComponentsInChildren<Toggle>(true);
            foreach (Toggle toggle in toggles3) if (toggle.isOn) checkedItems++;
        }

        nextButton.interactable = (checkedItems >= totalChecklistItems);
    }

    private void PlayVideo(string videoName)
{
    if (videoPlayer == null || string.IsNullOrEmpty(videoName))
        return;

    string path = System.IO.Path.Combine(
        Application.streamingAssetsPath,
        "Videos",
        videoName
    );

    videoPlayer.Stop();
    videoPlayer.url = path;
    videoPlayer.Prepare();

    videoPlayer.prepareCompleted += OnVideoPrepared;
}

private void OnVideoPrepared(VideoPlayer vp)
{
    vp.prepareCompleted -= OnVideoPrepared;
    vp.Play();
}
}

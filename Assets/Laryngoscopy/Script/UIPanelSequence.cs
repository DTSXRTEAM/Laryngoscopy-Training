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
    public TMP_Text contentText2;
    public Transform checklistParent3;
    public GameObject checkboxPrefab3;

    [Header("Models")]
    public ModelManager modelManager;

    [Header("Buttons")]
    public Button nextButton;
    public Button backButton;
    public Button replayButton;

    public Button correctButton;
    public Button incorrectButton;

    public TMP_Text correctButtonText;
    public TMP_Text incorrectButtonText;

    [Header("Exit Button")]
    public Button exitButton;
    public TMP_Text exitButtonText;

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
    public TMP_Text videoHeadingText;   // NEW: video panel heading

    [Header("Image Panel")]
    public TMP_Text imageHeadingText;   // NEW: image panel heading

    [Header("Introduction Back")]
    public GameObject introductionBackPrefab;

    public QR_Manager qrManager;
    public TriggerManager triggerManager;

    private Dictionary<int, bool[]> checklistStates = new Dictionary<int, bool[]>();

    [Header("Button Audio")]
    public AudioSource buttonAudioSource;

    [Header("Auscultation Colliders")]
    public Collider auscultationCollider1;
    public Collider auscultationCollider2;



    [Serializable]
    public class StepAudioData
    {
        public int stepIndex;
        public AudioSource audioSource;
    }

    public StepAudioData[] stepAudios;

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

        public bool useAlternateContent = false;
        public bool useAlternateChecklist = false;

        public string videoName;
        public TriggerManager triggerManager;
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
        PlayButtonSound();

        StepData step = steps[currentIndex];

        if (step.index == 24 || step.index == 30)
        {
            ExitTraining();
            return;
        }

        if (step.nextIndex != -1)
        {
            currentIndex = step.nextIndex;
            ShowStep(true);
        }
    }
    private void PlayButtonSound()
    {
        if (buttonAudioSource != null)
        {
            buttonAudioSource.Play();
        }
    }
    public void StartTraining()
    {
        introductionBackPrefab.SetActive(false);
        gameObject.SetActive(true);
        currentIndex = 0;
        ShowStep(true);
    }

    public void RestartTraining()
    {
        currentIndex = 0;
        ShowStep(false);
    }

    public void ExitTraining()
    {
        // Reset to first step
        currentIndex = 0;

        // Remove all saved checklist states
        checklistStates.Clear();

        if (triggerManager != null)
            triggerManager.ResetAllObjects();

        if (qrManager != null)
            qrManager.ResetQR();

        if (animationController != null)
            animationController.ResetAllAnimations();

        if (speaker != null)
            speaker.Stop();

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = null;
        }

        // Now recreate the first page
        ShowStep(false);

        if (introductionBackPrefab != null)
            introductionBackPrefab.SetActive(true);

        gameObject.SetActive(false);

        Debug.Log("Training Reset Complete");
    }

    public void PreviousPage()
    {
        PlayButtonSound();

        if (currentIndex == 0)
        {
            if (introductionBackPrefab != null)
            {
                introductionBackPrefab.SetActive(true);
                gameObject.SetActive(false);
            }
            return;
        }

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

    private void HandleStepAudio(int stepIndex)
    {
        foreach (StepAudioData item in stepAudios)
        {
            if (item.audioSource != null) item.audioSource.Stop();
        }
        foreach (StepAudioData item in stepAudios)
        {
            if (item.stepIndex == stepIndex && item.audioSource != null)
            {
                item.audioSource.loop = true;
                if (!item.audioSource.isPlaying) item.audioSource.Play();
                break;
            }
        }
    }

    void ShowStep(bool playAnimation = true)
    {
        StepData step = steps[currentIndex];

        //=========================================
        // Enable / Disable Auscultation Colliders
        //=========================================
        bool enableColliders = (step.index == 21 || step.index == 27);

        if (auscultationCollider1 != null)
            auscultationCollider1.enabled = enableColliders;

        if (auscultationCollider2 != null)
            auscultationCollider2.enabled = enableColliders;

        //==========================
        // Heading
        //==========================
        headingText.text = step.heading;

        if (videoHeadingText != null)
            videoHeadingText.text = step.heading;

        if (imageHeadingText != null)
            imageHeadingText.text = step.heading;

        //==========================
        // Content
        //==========================
        if (step.useAlternateContent && contentText2 != null)
        {
            contentText.gameObject.SetActive(false);
            contentText2.gameObject.SetActive(true);
            contentText2.text = step.content;
        }
        else
        {
            contentText.gameObject.SetActive(true);

            if (contentText2 != null)
                contentText2.gameObject.SetActive(false);

            contentText.text = step.content;
        }

        //==========================
        // Step Type
        //==========================
        bool isResultStep =
            step.buttonText.Equals("Correct", StringComparison.OrdinalIgnoreCase) &&
            step.backButtonText.Equals("Incorrect", StringComparison.OrdinalIgnoreCase);

        bool isExitStep =
            step.buttonText.Equals("Exit", StringComparison.OrdinalIgnoreCase);

        //==========================
        // Hide All Buttons
        //==========================
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        if (backButton != null) backButton.gameObject.SetActive(false);
        if (correctButton != null) correctButton.gameObject.SetActive(false);
        if (incorrectButton != null) incorrectButton.gameObject.SetActive(false);
        if (exitButton != null) exitButton.gameObject.SetActive(false);

        //==========================
        // EXIT BUTTON
        //==========================
        if (isExitStep)
        {
            if (exitButton != null)
            {
                exitButton.gameObject.SetActive(true);
                exitButton.interactable = true;
            }

            if (exitButtonText != null)
                exitButtonText.text = step.buttonText;
        }
        //==========================
        // CORRECT / INCORRECT
        //==========================
        else if (isResultStep)
        {
            if (correctButton != null)
            {
                correctButton.gameObject.SetActive(true);
                correctButton.interactable = true;
            }

            if (incorrectButton != null)
            {
                incorrectButton.gameObject.SetActive(true);

                // Disable Incorrect only on Step 28
                incorrectButton.interactable = (step.index != 28);
            }

            if (correctButtonText != null)
                correctButtonText.text = step.buttonText;

            if (incorrectButtonText != null)
                incorrectButtonText.text = step.backButtonText;
        }
        //==========================
        // NORMAL BUTTONS
        //==========================
        else
        {
            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(step.showNextButton);
                nextButton.interactable = true;
            }

            if (backButton != null)
            {
                backButton.gameObject.SetActive(step.showBackButton);
                backButton.interactable = true;
            }

            if (nextButtonText != null)
                nextButtonText.text = step.buttonText;

            if (backButtonText != null)
                backButtonText.text = string.IsNullOrEmpty(step.backButtonText)
                    ? "Back"
                    : step.backButtonText;
        }

        //==========================
        // Animation
        //==========================
        if (animationController != null && playAnimation)
            animationController.PlayAnimation(step.index);

        //==========================
        // Checklist
        //==========================
        GenerateChecklist(step);

        //==========================
        // TTS
        //==========================
        currentTTS = step.tts;
        PlayTTS(currentTTS);

        if (replayButton != null)
            replayButton.interactable = !string.IsNullOrEmpty(currentTTS);

        //==========================
        // Models
        //==========================
        if (modelManager != null && step.models != null)
            modelManager.ShowModels(step.models);

        //==========================
        // Video
        //==========================
        PlayVideo(step.videoName);

        //==========================
        // Audio
        //==========================
        HandleStepAudio(step.index);
    }
    private void PlayTTS(string text)
    {
        if (speaker == null || string.IsNullOrEmpty(text)) return;
        speaker.Stop();
        speaker.SpeakQueued(text);
    }

    public void ReplayTTS()
    {
        PlayButtonSound();

        // Reset all trigger objects (restore materials & stop audio)
        if (triggerManager != null)
            triggerManager.ResetAllObjects();

        StepData step = steps[currentIndex];

        if (animationController != null)
            animationController.PlayAnimation(step.index);

        PlayTTS(currentTTS);
    }

    void GenerateChecklist(StepData step)
    {
        foreach (Transform child in checklistParent)
            Destroy(child.gameObject);

        foreach (Transform child in checklistParent2)
            Destroy(child.gameObject);

        if (checklistParent3 != null)
        {
            foreach (Transform child in checklistParent3)
                Destroy(child.gameObject);
        }

        if (step.checkList == null || step.checkList.Length == 0)
        {
            checklistParent.gameObject.SetActive(false);
            checklistParent2.gameObject.SetActive(false);

            if (checklistParent3 != null)
                checklistParent3.gameObject.SetActive(false);

            nextButton.interactable = true;
            return;
        }

        totalChecklistItems = step.checkList.Length;

        Transform parent;
        GameObject prefab;

        if (step.useAlternateChecklist && checklistParent3 != null)
        {
            checklistParent.gameObject.SetActive(false);
            checklistParent2.gameObject.SetActive(false);
            checklistParent3.gameObject.SetActive(true);

            parent = checklistParent3;
            prefab = checkboxPrefab3;
        }
        else if (step.checkList.Length <= 2)
        {
            checklistParent.gameObject.SetActive(true);
            checklistParent2.gameObject.SetActive(false);

            if (checklistParent3 != null)
                checklistParent3.gameObject.SetActive(false);

            parent = checklistParent;
            prefab = checkboxPrefab;
        }
        else
        {
            checklistParent.gameObject.SetActive(false);
            checklistParent2.gameObject.SetActive(true);

            if (checklistParent3 != null)
                checklistParent3.gameObject.SetActive(false);

            parent = checklistParent2;
            prefab = checkboxPrefab2;
        }

        bool[] savedStates = null;

        if (checklistStates.ContainsKey(step.index))
            savedStates = checklistStates[step.index];

        for (int i = 0; i < step.checkList.Length; i++)
        {
            GameObject obj = Instantiate(prefab, parent);

            TMP_Text txt = obj.GetComponentInChildren<TMP_Text>();
            if (txt != null)
                txt.text = step.checkList[i];

            Toggle toggle = obj.GetComponentInChildren<Toggle>();

            if (toggle != null)
            {
                int toggleIndex = i;

                toggle.onValueChanged.RemoveAllListeners();

                if (savedStates != null && toggleIndex < savedStates.Length)
                    toggle.isOn = savedStates[toggleIndex];
                else
                    toggle.isOn = false;

                toggle.onValueChanged.AddListener((value) =>
                {
                    SaveChecklistState();
                    CheckChecklistCompleted();
                });
            }
        }

        CheckChecklistCompleted();
    }
    private void CheckChecklistCompleted()
    {
        int checkedItems = 0;

        Toggle[] toggles = null;

        if (checklistParent3 != null && checklistParent3.gameObject.activeSelf)
            toggles = checklistParent3.GetComponentsInChildren<Toggle>(true);
        else if (checklistParent.gameObject.activeSelf)
            toggles = checklistParent.GetComponentsInChildren<Toggle>(true);
        else
            toggles = checklistParent2.GetComponentsInChildren<Toggle>(true);

        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
                checkedItems++;
        }

        nextButton.interactable = (checkedItems == totalChecklistItems);
    }

    private void SaveChecklistState()
    {
        StepData step = steps[currentIndex];

        if (step.checkList == null || step.checkList.Length == 0)
            return;

        Toggle[] toggles = null;

        if (checklistParent3 != null && checklistParent3.gameObject.activeSelf)
            toggles = checklistParent3.GetComponentsInChildren<Toggle>(true);
        else if (checklistParent.gameObject.activeSelf)
            toggles = checklistParent.GetComponentsInChildren<Toggle>(true);
        else
            toggles = checklistParent2.GetComponentsInChildren<Toggle>(true);

        bool[] states = new bool[toggles.Length];

        for (int i = 0; i < toggles.Length; i++)
            states[i] = toggles[i].isOn;

        checklistStates[step.index] = states;
    }

    private void PlayVideo(string videoName)
    {
        if (videoPlayer == null || string.IsNullOrEmpty(videoName)) return;
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Videos", videoName);
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

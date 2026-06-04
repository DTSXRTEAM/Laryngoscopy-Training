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

    [Header("Models")]
    public ModelManager modelManager;

    [Header("Buttons")]
    public Button nextButton;
    public Button backButton;

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

    int currentIndex = 0;
    private int checkedCount;
    private int totalChecklistItems;

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

        // BUTTON VISIBILITY
        if (nextButton != null)
            nextButton.gameObject.SetActive(step.showNextButton);

        if (backButton != null)
            backButton.gameObject.SetActive(step.showBackButton);

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

        //if (modelManager != null)
        //{
        //    modelManager.ShowModels(step.models);
        //}

        if (modelManager != null && step.models != null)
        {
            modelManager.ShowModels(step.models);
        }
    }

    //void GenerateChecklist(string[] list)
    //{
    //    // CLEAR FIRST PARENT
    //    for (int i = 0; i < checklistParent.childCount; i++)
    //    {
    //        Destroy(checklistParent.GetChild(i).gameObject);
    //    }

    //    // CLEAR SECOND PARENT
    //    for (int i = 0; i < checklistParent2.childCount; i++)
    //    {
    //        Destroy(checklistParent2.GetChild(i).gameObject);
    //    }

    //    if (list == null)
    //        return;

    //    for (int i = 0; i < list.Length; i++)
    //    {
    //        Transform targetParent;
    //        GameObject targetPrefab;

    //        // FIRST 4 ITEMS
    //        if (i < 4)
    //        {
    //            targetParent = checklistParent;
    //            targetPrefab = checkboxPrefab;
    //        }
    //        // REMAINING ITEMS
    //        else
    //        {
    //            targetParent = checklistParent2;
    //            targetPrefab = checkboxPrefab2;
    //        }

    //        GameObject obj =
    //            Instantiate(targetPrefab, targetParent);

    //        TMP_Text txt =
    //            obj.GetComponentInChildren<TMP_Text>();

    //        txt.text = list[i];
    //    }
    //}

 void GenerateChecklist(string[] list)
{
    // Clear Parent 1
    for (int i = checklistParent.childCount - 1; i >= 0; i--)
    {
        Destroy(checklistParent.GetChild(i).gameObject);
    }

    // Clear Parent 2
    for (int i = checklistParent2.childCount - 1; i >= 0; i--)
    {
        Destroy(checklistParent2.GetChild(i).gameObject);
    }

    checkedCount = 0;

    if (list == null || list.Length == 0)
    {
        checklistParent.gameObject.SetActive(false);
        checklistParent2.gameObject.SetActive(false);

        nextButton.interactable = true;
        return;
    }

    totalChecklistItems = list.Length;

    // Show checklist 1 for small lists
    if (list.Length <= 2)
    {
        checklistParent.gameObject.SetActive(true);
        checklistParent2.gameObject.SetActive(false);

        for (int i = 0; i < list.Length; i++)
        {
            GameObject obj =
                Instantiate(
                    checkboxPrefab,
                    checklistParent);

            TMP_Text txt =
                obj.GetComponentInChildren<TMP_Text>();

            if (txt != null)
                txt.text = list[i];

            Toggle toggle =
                obj.GetComponentInChildren<Toggle>();

            if (toggle != null)
            {
                toggle.isOn = false;

                toggle.onValueChanged.AddListener(
                    delegate
                    {
                        CheckChecklistCompleted();
                    });
            }
        }
    }
    // Show checklist 2 for larger lists
    else
    {
        checklistParent.gameObject.SetActive(false);
        checklistParent2.gameObject.SetActive(true);

        for (int i = 0; i < list.Length; i++)
        {
            GameObject obj =
                Instantiate(
                    checkboxPrefab2,
                    checklistParent2);

            TMP_Text txt =
                obj.GetComponentInChildren<TMP_Text>();

            if (txt != null)
                txt.text = list[i];

            Toggle toggle =
                obj.GetComponentInChildren<Toggle>();

            if (toggle != null)
            {
                toggle.isOn = false;

                toggle.onValueChanged.AddListener(
                    delegate
                    {
                        CheckChecklistCompleted();
                    });
            }
        }
    }

    nextButton.interactable = false;
    CheckChecklistCompleted();
}

private void CheckChecklistCompleted()
{
    int checkedItems = 0;

    Toggle[] toggles1 =
        checklistParent.GetComponentsInChildren<Toggle>(true);

    foreach (Toggle toggle in toggles1)
    {
        if (toggle.isOn)
            checkedItems++;
    }

    Toggle[] toggles2 =
        checklistParent2.GetComponentsInChildren<Toggle>(true);

    foreach (Toggle toggle in toggles2)
    {
        if (toggle.isOn)
            checkedItems++;
    }

    Debug.Log(
        "Checked: " +
        checkedItems +
        " / " +
        totalChecklistItems);

    nextButton.interactable =
        checkedItems >= totalChecklistItems;
}
}
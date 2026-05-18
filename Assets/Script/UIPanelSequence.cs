using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIPanelSequence : MonoBehaviour
{
    [System.Serializable]
    public class UIPage
    {
        public string heading;
        [TextArea(3, 5)]
        public string content;
        public string buttonName;
    }

    public TMP_Text headingText;
    public TMP_Text contentText;
    public TMP_Text buttonText;

    public List<UIPage> pages = new List<UIPage>();

    int currentIndex = 0;

    void Start()
    {
        ShowPage();
    }

    public void NextPage()
    {
        currentIndex++;

        if (currentIndex >= pages.Count)
        {
            currentIndex = 0;
        }

        ShowPage();
    }

    void ShowPage()
    {
        headingText.text = pages[currentIndex].heading;
        contentText.text = pages[currentIndex].content;
        buttonText.text = pages[currentIndex].buttonName;
    }
}
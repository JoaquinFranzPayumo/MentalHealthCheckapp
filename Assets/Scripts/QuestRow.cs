using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class QuestRow : MonoBehaviour
{
    public Button completeButton;
    public TMP_Text questLabel;

    private Image buttonImg;
    private Color ogColor;
    private Action<bool> completeCall;
    private bool isComplete;

    private void Awake()
    {
        if (completeButton != null)
        {
            buttonImg = completeButton.GetComponent<Image>();

            if (buttonImg != null)
            {
                ogColor = buttonImg.color;
            }
        }
    }

    public void Setup(QuestDef quest, Action<bool> onComplete)
    {
         completeCall = onComplete;
        isComplete = false;

        questLabel.text = quest.QuestName + " [+" + quest.Reward + " 0]";

        if (buttonImg != null)
        {
            buttonImg.color = ogColor;
        }

        completeButton.interactable = true;
        completeButton.onClick.RemoveAllListeners();
        completeButton.onClick.AddListener(CompleteQuest);
    }

    private void CompleteQuest()
    {
        if (isComplete)
        {
            return;
        }

        isComplete = true;
        completeButton.interactable = false;

        if(buttonImg != null)
        {
            buttonImg.color = new Color( 0.55f, 0.8f, 0.55f, 1f);
        }

        questLabel.text = "/" +questLabel.text;
        completeCall?.Invoke(true);
    }
}

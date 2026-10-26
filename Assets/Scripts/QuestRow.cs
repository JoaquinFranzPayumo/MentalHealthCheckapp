using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class QuestRow : MonoBehaviour
{
    [Header("Existing completion control")]
    public Button completeButton;
    public TMP_Text questLabel;

    [Header("Optional details control")]
    [Tooltip("Optional button to open the quest suggestion panel. If left empty, clicking the quest label's GameObject will not select the quest.")]
    public Button detailsButton;

    private Image buttonImg;
    private Color originalButtonColor;
    private Action<bool> completeCall;
    private Action selectCall;
    private bool isComplete;

    private void Awake()
    {
        if (completeButton != null)
        {
            buttonImg = completeButton.GetComponent<Image>();
            if (buttonImg != null)
                originalButtonColor = buttonImg.color;
        }
    }

    public void Setup(QuestDef quest, Action<bool> onComplete, Action onSelect = null)
    {
        completeCall = onComplete;
        selectCall = onSelect;
        isComplete = false;

        if (questLabel != null)
        {
            questLabel.text = quest.QuestName + " [+" + quest.Reward + " XP]";
        }

        if (buttonImg != null)
            buttonImg.color = originalButtonColor;

        if (completeButton != null)
        {
            completeButton.interactable = true;
            completeButton.onClick.RemoveListener(CompleteQuest);
            completeButton.onClick.AddListener(CompleteQuest);
        }

        if (detailsButton != null)
        {
            detailsButton.onClick.RemoveListener(SelectDetails);
            detailsButton.onClick.AddListener(SelectDetails);
        }
    }

    private void SelectDetails()
    {
        selectCall?.Invoke();
    }

    private void CompleteQuest()
    {
        if (isComplete)
            return;

        isComplete = true;

        if (completeButton != null)
            completeButton.interactable = false;

        if (buttonImg != null)
            buttonImg.color = new Color(0.55f, 0.8f, 0.55f, 1f);

        if (questLabel != null)
            questLabel.text = "✓ " + questLabel.text;

        completeCall?.Invoke(true);
    }

    private void OnDestroy()
    {
        if (completeButton != null)
            completeButton.onClick.RemoveListener(CompleteQuest);

        if (detailsButton != null)
            detailsButton.onClick.RemoveListener(SelectDetails);
    }
}

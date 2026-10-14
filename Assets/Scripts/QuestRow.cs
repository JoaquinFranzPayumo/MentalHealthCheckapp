using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class QuestRow : MonoBehaviour
{
    public Toggle toggle;
    public TMP_Text questName;
    public TMP_Text reward;

    public void Setup(QuestDef quest, Action<bool> onChanged)
    {
        toggle.onValueChanged.RemoveAllListeners();

        questName.text = quest.QuestName;
        reward.text = "0 " + quest.Reward;

        toggle.onValueChanged.AddListener(
            isOn => onChanged(isOn));
    }

    public void MarkComplete()
    {
        toggle.interactable = false;
    }
}

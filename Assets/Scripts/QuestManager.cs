using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [Header("Available Quests")]
    public List<QuestDef> allQuests = new List<QuestDef>();

    [Header("Existing Quest Rows")]
    public QuestRow[] questRow;

    [Header("UI")]
    public TMP_Text scoreTxT;
    public Button resetButton;

    private int totalPoints;
    private int completed;

    private readonly List<QuestDef> currentOBJ = new List<QuestDef>();

    void Start()
    {
        resetButton.onClick.AddListener(GenerateNewQuest);
    }

    public void GenerateNewQuest()
    {
        List<QuestDef> pool = new List<QuestDef>(allQuests);

        for (int i = pool.Count -1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            QuestDef temp = pool[i];
            pool[i] = pool[j];
            pool[j] = temp;
        }

        int count = Mathf.Min(questRow.Length, pool.Count);

        currentOBJ.Clear();
        completed = 0;

        for (int i = 0; i < count; i++)
        {
            QuestDef quest = pool[i];
            currentOBJ.Add(quest);

            int index = i;

            questRow[i].Setup(quest, isComplete =>
            {
                if (isComplete) CompleteQuest(index);

            });
        }

        for (int i = 0; i < questRow.Length; i++)
        {
            questRow[i].gameObject.SetActive(i < count);
        }

        resetButton.interactable = count > 0 && completed == count;
    }

    private void CompleteQuest(int index)
    {
        if (index < 0 || index >= currentOBJ.Count)
        {
            return;
        }

        totalPoints += currentOBJ[index].Reward;

        completed++;

        UpdateScore();

        resetButton.interactable = completed == currentOBJ.Count && currentOBJ.Count > 0;
    }

    private void UpdateScore()
    {
        scoreTxT.text = "Total Score: " + totalPoints;
    }
}

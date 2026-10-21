
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

    private int totalPoints = 0;
    private int completed = 0;

    private readonly List<QuestDef> currentOBJ =
        new List<QuestDef>();

    private readonly HashSet<int> completedIndices =
        new HashSet<int>();

    void Start()
    {
        if (resetButton == null || scoreTxT == null)
        {
            Debug.LogError(
                "QuestManager is missing a UI reference!");
            enabled = false;
            return;
        }

        resetButton.onClick.AddListener(GenerateNewQuest);

        UpdateScore();
        GenerateNewQuest();
    }

    public void GenerateNewQuest()
    {
        if (allQuests == null || questRow == null)
        {
            Debug.LogError("Quest list or quest rows are missing!");
            return;
        }

        List<QuestDef> pool = new List<QuestDef>(allQuests);

        // Shuffle quests randomly.
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            QuestDef temp = pool[i];
            pool[i] = pool[j];
            pool[j] = temp;
        }

        int count = Mathf.Min(questRow.Length, pool.Count);

        currentOBJ.Clear();
        completedIndices.Clear();
        completed = 0;

        for (int i = 0; i < count; i++)
        {
            if (questRow[i] == null)
            {
                Debug.LogError("A QuestRow reference is missing!");
                continue;
            }

            QuestDef quest = pool[i];
            currentOBJ.Add(quest);

            int index = i;

            questRow[i].Setup(quest, isComplete =>
            {
                if (isComplete)
                    CompleteQuest(index);
            });
        }

        for (int i = 0; i < questRow.Length; i++)
        {
            if (questRow[i] != null)
                questRow[i].gameObject.SetActive(i < count);
        }

        resetButton.interactable =
            count > 0 && completed == count;

        UpdateScore();
    }

    private void CompleteQuest(int index)
    {
        if (index < 0 || index >= currentOBJ.Count)
            return;

        if (!completedIndices.Add(index))
            return;

        totalPoints += currentOBJ[index].Reward;
        completed++;

        UpdateScore();

        resetButton.interactable =
            completed == currentOBJ.Count &&
            currentOBJ.Count > 0;
    }

    private void UpdateScore()
    {
        scoreTxT.text = "Total Score: " + totalPoints;
    }

    void OnDestroy()
    {
        if (resetButton != null)
            resetButton.onClick.RemoveListener(GenerateNewQuest);
    }
}

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

    [Header("Existing UI")]
    public TMP_Text scoreTxT;
    public Button resetButton;

    [Header("Quest Response UI")]
    public GameObject responsePanel;
    public TMP_Text responseTitleText;
    public TMP_Text responseDescriptionText;
    public TMP_Text responseSuggestionText;

    private int totalPoints = 0;
    private int completed = 0;

    private readonly List<QuestDef> currentOBJ = new List<QuestDef>();
    private readonly HashSet<int> completedIndices = new HashSet<int>();

    private void Start()
    {
        if (resetButton == null || scoreTxT == null)
        {
            Debug.LogError("QuestManager is missing the score text or reset button reference.");
            enabled = false;
            return;
        }

        if (responsePanel != null)
            responsePanel.SetActive(false);

        resetButton.onClick.AddListener(GenerateNewQuest);
        UpdateScore();
        GenerateNewQuest();
    }

    public void GenerateNewQuest()
    {
        if (allQuests == null || questRow == null)
        {
            Debug.LogError("QuestManager: Quest list or quest rows are missing.");
            return;
        }

        List<QuestDef> pool = new List<QuestDef>(allQuests);

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

        if (responsePanel != null)
            responsePanel.SetActive(false);

        for (int i = 0; i < count; i++)
        {
            if (questRow[i] == null)
            {
                Debug.LogError("QuestManager: A QuestRow reference is missing.");
                continue;
            }

            QuestDef quest = pool[i];
            currentOBJ.Add(quest);
            int index = i;

            questRow[i].Setup(
                quest,
                isComplete =>
                {
                    if (isComplete)
                        CompleteQuest(index);
                },
                () => ShowQuestDetails(index)
            );
        }

        for (int i = 0; i < questRow.Length; i++)
        {
            if (questRow[i] != null)
                questRow[i].gameObject.SetActive(i < count);
        }

        resetButton.interactable = count > 0 && completed == count;
        UpdateScore();
    }

    private void ShowQuestDetails(int index)
    {
        if (index < 0 || index >= currentOBJ.Count)
            return;

        QuestDef quest = currentOBJ[index];

        if (responsePanel == null || responseTitleText == null ||
            responseDescriptionText == null || responseSuggestionText == null)
        {
            Debug.LogWarning("Quest details UI is not fully assigned in the Inspector.");
            return;
        }

        responsePanel.SetActive(true);
        responseTitleText.text = quest.QuestName;
        responseDescriptionText.text = string.IsNullOrWhiteSpace(quest.Description)
            ? "Give this activity a try at your own pace."
            : quest.Description;
        responseSuggestionText.text =
            (string.IsNullOrWhiteSpace(quest.Suggestion)
                ? "Try starting with a small, manageable step."
                : quest.Suggestion)
            + "\n\nReward: " + quest.Reward + " XP";
    }

    private void CompleteQuest(int index)
    {
        if (index < 0 || index >= currentOBJ.Count || !completedIndices.Add(index))
            return;

        totalPoints += currentOBJ[index].Reward;
        completed++;

        UpdateScore();

        resetButton.interactable =
            completed == currentOBJ.Count && currentOBJ.Count > 0;
    }

    private void UpdateScore()
    {
        if (scoreTxT != null)
            scoreTxT.text = "Total Score: " + totalPoints;
    }

    private void OnDestroy()
    {
        if (resetButton != null)
            resetButton.onClick.RemoveListener(GenerateNewQuest);
    }
}

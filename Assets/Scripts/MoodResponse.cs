using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoodResponse : MonoBehaviour
{
    [Header("Mood Buttons (ordered: Great, Good, Okay, Low, Difficult)")]
    public Button[] moodButtons;

    [Header("Response Panel")]
    public GameObject responsePanel;
    public TMP_Text responseText;
    public TMP_Text activitiesText;

    [Header("Appearance")]
    public Color selectedColor = new Color(0.94f, 0.88f, 0.77f, 1f);

    private Color[] originalColors;
    private int selectedMood = -1;

    private readonly string[] moodNames =
    {
        "Great", "Good", "Okay", "Low", "Difficult"
    };

    private readonly string[] responses =
    {
        "It's great that you're feeling good. Take a moment to appreciate what's going well.",
        "It's nice that things are going fairly well. Keep doing what feels right for you.",
        "It's okay to have an ordinary day. You can take things one step at a time.",
        "Some days feel harder than others. Be gentle with yourself and take things at your own pace.",
        "You don't have to figure everything out right now. If you need support, consider reaching out to someone you trust."
    };

    private readonly string[][] activities =
    {
        new[] { "Try a hobby you enjoy", "Share a happy moment with a friend", "Spend some time outdoors" },
        new[] { "Listen to favorite music", "Take a relaxing walk", "Do something creative" },
        new[] { "Take a short break", "Watch a favorite show or movie", "Listen to calming music" },
        new[] { "Take a gentle walk if you feel up to it", "Watch a comforting movie", "Message someone you trust" },
        new[] { "Spend a little time somewhere comfortable", "Choose one small, comforting activity", "Reach out to someone supportive" }
    };

    private void Start()
    {
        if (moodButtons == null || moodButtons.Length != moodNames.Length ||
            responsePanel == null || responseText == null || activitiesText == null)
        {
            Debug.LogError("MoodResponse: Assign five mood buttons and all response UI references in the Inspector.");
            enabled = false;
            return;
        }

        originalColors = new Color[moodButtons.Length];

        for (int i = 0; i < moodButtons.Length; i++)
        {
            if (moodButtons[i] == null)
            {
                Debug.LogError("MoodResponse: One or more mood button references are missing.");
                enabled = false;
                return;
            }

            Image image = moodButtons[i].GetComponent<Image>();
            if (image != null)
                originalColors[i] = image.color;

            int index = i;
            moodButtons[i].onClick.AddListener(() => SelectMood(index));
        }

        responsePanel.SetActive(false);
    }

    public void SelectMood(int index)
    {
        if (index < 0 || index >= moodNames.Length)
            return;

        selectedMood = index;

        for (int i = 0; i < moodButtons.Length; i++)
        {
            Image image = moodButtons[i].GetComponent<Image>();
            if (image != null)
                image.color = i == selectedMood ? selectedColor : originalColors[i];
        }

        responsePanel.SetActive(true);
        responseText.text = moodNames[index] + "\n\n" + responses[index];

        string formattedActivities = "Activities you could try:\n";
        for (int i = 0; i < activities[index].Length; i++)
            formattedActivities += "• " + activities[index][i] + (i < activities[index].Length - 1 ? "\n" : "");

        activitiesText.text = formattedActivities;
    }

    private void OnDestroy()
    {
        if (moodButtons == null)
            return;

        foreach (Button button in moodButtons)
        {
            if (button != null)
                button.onClick.RemoveAllListeners();
        }
    }
}

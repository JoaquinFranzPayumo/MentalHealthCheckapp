using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class JournalEntry
{
    public string moodIcon;
    public string moodName;
    public string content;
    public string date;
}

[Serializable]
public class JournalSaveData
{
    public List<JournalEntry> entries = new List<JournalEntry>();
}

public class JournalManager : MonoBehaviour
{
    [Header("Journal Input")]
    public TMP_InputField journalInput;
    public Button saveButton;

    [Header("Mood Buttons (five buttons)")]
    public Button[] moodButtons;
    public string[] moodIcons = { "🌧️", "🌥️", "😐", "🙁", "😞" };
    public string[] moodNames = { "Difficult", "Low", "Okay", "Good", "Great" };

    [Header("Mood Response UI")]
    public GameObject responsePanel;
    public TMP_Text responseText;
    public TMP_Text promptText;

    [Header("Previous Entries")]
    public Transform entryContainer;
    public JournalEntryView entryPrefab;

    [Header("Appearance")]
    public Color selectedColor = new Color(0.94f, 0.88f, 0.77f, 1f);

    private int selectedMood = -1;
    private readonly List<JournalEntry> entries = new List<JournalEntry>();
    private readonly List<JournalEntryView> entryViews = new List<JournalEntryView>();
    private Color[] originalColors;

    private readonly string[] responses =
    {
        "You can write as much or as little as you want. Be gentle with yourself.",
        "It's okay to give yourself space to express what you're feeling.",
        "You don't need a special reason to write. Start wherever you like.",
        "Take a moment to notice something that went well today.",
        "Take a moment to appreciate something that made today feel good."
    };

    private readonly string[] prompts =
    {
        "What would you like to express, or what support might help?",
        "What has felt difficult, and what might help right now?",
        "What's been on your mind today?",
        "What was one positive moment today?",
        "What made today feel good?"
    };

    private string SavePath => Path.Combine(Application.persistentDataPath, "journal.json");

    private void Start()
    {
        if (journalInput == null || saveButton == null || entryContainer == null ||
            entryPrefab == null || moodButtons == null || moodIcons == null ||
            moodNames == null || moodButtons.Length != moodIcons.Length ||
            moodButtons.Length != moodNames.Length || moodButtons.Length != 5)
        {
            Debug.LogError("JournalManager: Assign all references and exactly five mood buttons, icons, and names.");
            enabled = false;
            return;
        }

        originalColors = new Color[moodButtons.Length];

        for (int i = 0; i < moodButtons.Length; i++)
        {
            int index = i;

            if (moodButtons[i] == null)
            {
                Debug.LogError("JournalManager: A mood button reference is missing.");
                enabled = false;
                return;
            }

            Image image = moodButtons[i].GetComponent<Image>();
            if (image != null)
                originalColors[i] = image.color;

            moodButtons[i].onClick.AddListener(() => SelectMood(index));
        }

        saveButton.onClick.AddListener(SaveEntry);
        journalInput.onValueChanged.AddListener(OnJournalTextChanged);

        if (responsePanel != null)
            responsePanel.SetActive(false);

        LoadEntries();
        UpdateSaveButton();
    }

    private void SelectMood(int index)
    {
        if (index < 0 || index >= moodButtons.Length)
            return;

        selectedMood = index;

        for (int i = 0; i < moodButtons.Length; i++)
        {
            Image image = moodButtons[i].GetComponent<Image>();
            if (image != null)
                image.color = i == selectedMood ? selectedColor : originalColors[i];
        }

        if (responsePanel != null)
            responsePanel.SetActive(true);

        if (responseText != null)
            responseText.text = moodNames[index] + "\n\n" + responses[index];

        if (promptText != null)
            promptText.text = "Writing prompt: " + prompts[index];

        UpdateSaveButton();
    }

    private void OnJournalTextChanged(string unused)
    {
        UpdateSaveButton();
    }

    private void UpdateSaveButton()
    {
        if (saveButton == null || journalInput == null)
            return;

        bool hasText = !string.IsNullOrWhiteSpace(journalInput.text);
        saveButton.interactable = hasText && selectedMood >= 0;
    }

    private void SaveEntry()
    {
        if (selectedMood < 0 || string.IsNullOrWhiteSpace(journalInput.text))
            return;

        JournalEntry entry = new JournalEntry
        {
            moodIcon = moodIcons[selectedMood],
            moodName = moodNames[selectedMood],
            content = journalInput.text.Trim(),
            date = DateTime.Now.ToString("ddd, MMM dd")
        };

        entries.Insert(0, entry);

        bool saved = SaveToFile();
        DisplayEntry(entry);

        journalInput.text = "";
        selectedMood = -1;

        for (int i = 0; i < moodButtons.Length; i++)
        {
            Image image = moodButtons[i].GetComponent<Image>();
            if (image != null)
                image.color = originalColors[i];
        }

        if (responsePanel != null)
            responsePanel.SetActive(false);

        UpdateSaveButton();

        Debug.Log(saved ? "Journal entry saved." : "Journal entry displayed, but file saving failed.");
    }

    private void DisplayEntry(JournalEntry entry)
    {
        JournalEntryView view = Instantiate(entryPrefab, entryContainer);
        view.Setup(entry.moodIcon, entry.content, entry.date);
        entryViews.Add(view);
    }

    private bool SaveToFile()
    {
        JournalSaveData data = new JournalSaveData { entries = entries };
        string json = JsonUtility.ToJson(data, true);

        try
        {
            File.WriteAllText(SavePath, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Could not save journal: " + e.Message);
            return false;
        }
    }

    private void LoadEntries()
    {
        if (!File.Exists(SavePath))
            return;

        try
        {
            string json = File.ReadAllText(SavePath);
            JournalSaveData data = JsonUtility.FromJson<JournalSaveData>(json);

            if (data == null || data.entries == null)
                return;

            // Copy loaded entries into the live list before displaying them.
            entries.Clear();
            entries.AddRange(data.entries);

            foreach (JournalEntry entry in entries)
                DisplayEntry(entry);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not load journal: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        if (saveButton != null)
            saveButton.onClick.RemoveListener(SaveEntry);

        if (journalInput != null)
            journalInput.onValueChanged.RemoveListener(OnJournalTextChanged);

        if (moodButtons != null)
        {
            foreach (Button button in moodButtons)
            {
                if (button != null)
                    button.onClick.RemoveAllListeners();
            }
        }
    }
}

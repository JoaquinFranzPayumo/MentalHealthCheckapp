using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[Serializable]
public class JournalEntry
{
    public string moodIcon;
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
    [Header("Mood Buttons")]
    public TMP_InputField journalInput;
    public Button saveButton;

    [Header("Mood Buttons")]
    public Button[] moodButtons;
    public string[] moodIcons = { "🌧️", "🌥️", "🌤️", "🌤️", "☀️" };

    [Header("Previous Entries")]
    public Transform entryContainer;
    public JournalEntryView entryPrefab;

    [Header("Appearance")]
    public Color selectedColor = new Color(0.94f, 0.88f, 0.77f);

    private int selectedMood = -1;

    private readonly List<JournalEntry> entries = new List<JournalEntry>();
    private readonly List<JournalEntryView> entryViews = new List<JournalEntryView>();
    private Color[] originalColors;

    private string SavePath => Path.Combine(Application.persistentDataPath, "journal.json");

    void Start()
    {
        if (journalInput == null || saveButton == null ||
            entryContainer == null || entryPrefab == null ||
            moodButtons == null || moodIcons == null ||
            moodButtons.Length != moodIcons.Length)
        {
            Debug.LogError("JournalManager is missing some references" + "or the mood button and icon counts differ");
            enabled = false;
            return;
        }

        originalColors = new Color[moodButtons.Length];

        for (int i = 0; i < moodButtons.Length; i++)
        {
            int index = i;
            
            if (moodButtons[i] == null)
            {
                Debug.LogError("A mood button is missing!");
                enabled = false;
                return;
            }

            Image image = moodButtons[i].GetComponent<Image>();

            if (image != null)
            {
                originalColors[i] = image.color;
            }

            moodButtons[i].onClick.AddListener(() => SelectMood(index));
        }

        saveButton.onClick.AddListener(SaveEntry);
        journalInput.onValueChanged.AddListener(_ => UpdateSaveButton());

        LoadEntries();
        UpdateSaveButton();
    }

    void SelectMood(int index)
    {
        selectedMood = index;

        for (int i = 0; i < moodButtons.Length; i++)
        {
            Image image = moodButtons[i].GetComponent<Image>();

            if (image != null)
            {
                image.color = i == selectedMood ? selectedColor : originalColors[i];
            }
        }
        UpdateSaveButton();
    }

    void UpdateSaveButton()
    {
        bool hasText = !string.IsNullOrWhiteSpace(journalInput.text);
        saveButton.interactable = hasText && selectedMood >= 0; 
    }

    void SaveEntry()
    {
        if (selectedMood < 0 || string.IsNullOrWhiteSpace(journalInput.text))
        {
            return;

        }

        JournalEntry entry = new JournalEntry
        {
            moodIcon = moodIcons[selectedMood],
            content = journalInput.text.Trim(),
            date = DateTime.Now.ToString("ddd, MMM dd")
        };

        entries.Insert(0, entry);

        SaveToFile();
        DisplayEntry(entry);

        journalInput.text = "";
        selectedMood = -1;

        for (int i = 0;i < moodButtons.Length;i++)
        {
            Image image = moodButtons[i].GetComponent<Image>();

            if (image != null)
            {
                image.color = originalColors[i];
            }
        }

        UpdateSaveButton();

        Debug.Log("Journal Entry Saved!");
    }

    void DisplayEntry(JournalEntry entry)
    {
        JournalEntryView view = Instantiate(entryPrefab, entryContainer);

        view.Setup(entry.moodIcon, entry.content, entry.date);

        entryViews.Insert(0, view);
    }

    void SaveToFile()
    {
        JournalSaveData data = new JournalSaveData { entries = entries };

        string json = JsonUtility.ToJson(data, true);

        try
        {
            File.WriteAllText(SavePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not save journal: " + e.Message);
        }
    }

    void LoadEntries()
    {
        if (!File.Exists(SavePath))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            JournalSaveData data = JsonUtility.FromJson<JournalSaveData>(json);

            if (data == null || data.entries == null)
            {
                return;
            }

            foreach (JournalEntry entry in entries) DisplayEntry(entry);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not load journal: " + e.Message);
        }
    }

    void OnDestroy()
    {
        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(SaveEntry);
        }
        if (moodButtons != null)
        {
            foreach (Button button in moodButtons)
            {
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                }
            }
        }
    }
}

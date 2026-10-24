using UnityEngine;
using TMPro;

public class JournalEntryView : MonoBehaviour
{
    public TMP_Text Mood;
    public TMP_Text Entry;
    public TMP_Text Date;

    public void Setup(string moodIcon, string content, string date)
    {
        Mood.text = moodIcon;
        Entry.text = content;
        Date.text = date;
    }
}

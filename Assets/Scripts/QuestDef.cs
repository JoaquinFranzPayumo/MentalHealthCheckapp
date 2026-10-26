using System;
using UnityEngine;

[Serializable]
public class QuestDef
{
    public string QuestName;
    public int Reward;

    [TextArea(2, 4)]
    public string Description;

    [TextArea(2, 4)]
    public string Suggestion;
}

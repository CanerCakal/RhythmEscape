using System;
using UnityEngine;

public enum QuestType
{
    Distance,
    CorrectRhythmMoves,
    ObstaclesPassed
}

[Serializable]
public class Quest
{
    [Header("Quest Info")]
    public string questName;
    [TextArea] public string description;

    [Header("Quest Goal")]
    public QuestType questType;
    public int targetAmount;
    public int currentAmount;
    public int rewardScore;

    [Header("Quest State")]
    public bool isCompleted;

    public Quest(string questName, string description, QuestType questType, int targetAmount, int rewardScore)
    {
        this.questName = questName;
        this.description = description;
        this.questType = questType;
        this.targetAmount = targetAmount;
        this.rewardScore = rewardScore;

        currentAmount = 0;
        isCompleted = false;
    }

    public void AddProgress(int amount)
    {
        if (isCompleted)
        {
            return;
        }

        currentAmount += amount;

        if (currentAmount >= targetAmount)
        {
            currentAmount = targetAmount;
            isCompleted = true;
        }
    }

    public string GetProgressText()
    {
        return currentAmount + " / " + targetAmount;
    }
}
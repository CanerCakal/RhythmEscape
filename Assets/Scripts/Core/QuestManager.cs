using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Quest Settings")]
    [SerializeField] private List<Quest> quests = new List<Quest>();

    [Header("Next Quest Settings")]
    [SerializeField] private float nextQuestDelay = 1.5f;

    private int currentQuestIndex = 0;
    private Quest currentQuest;

    private float questStartZ;
    private int lastDistanceProgress;

    public event Action<Quest> OnQuestUpdated;
    public event Action<Quest> OnQuestCompleted;
    public event Action OnAllQuestsCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        PlayerMovement.OnCorrectRhythmMove += HandleCorrectRhythmMove;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnObstaclePassed += HandleObstaclePassed;
        }
    }

    private void OnDisable()
    {
        PlayerMovement.OnCorrectRhythmMove -= HandleCorrectRhythmMove;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnObstaclePassed -= HandleObstaclePassed;
        }
    }

    private void Start()
    {
        CreateDefaultQuestsIfNeeded();
        StartQuest(0);
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)
            {
                return;
            }
        }

        TrackDistanceQuest();
    }

    private void CreateDefaultQuestsIfNeeded()
    {
        if (quests.Count > 0)
        {
            return;
        }

        quests.Add(new Quest(
            "İlk Ritim",
            "5 kez doğru ritimde hareket et.",
            QuestType.CorrectRhythmMoves,
            5
        ));

        quests.Add(new Quest(
            "Yola Devam",
            "150 metre ilerle.",
            QuestType.Distance,
            150
        ));

        quests.Add(new Quest(
            "Engelleri Aş",
            "5 engeli başarıyla geç.",
            QuestType.ObstaclesPassed,
            5
        ));
    }

    private void StartQuest(int questIndex)
    {
        if (questIndex >= quests.Count)
        {
            currentQuest = null;
            OnAllQuestsCompleted?.Invoke();
            Debug.Log("Tüm görevler tamamlandı.");
            return;
        }

        currentQuestIndex = questIndex;
        currentQuest = quests[currentQuestIndex];

        currentQuest.currentAmount = 0;
        currentQuest.isCompleted = false;

        ResetDistanceTracking();

        Debug.Log("Yeni görev başladı: " + currentQuest.questName);

        OnQuestUpdated?.Invoke(currentQuest);
    }

    private void ResetDistanceTracking()
    {
        if (player != null)
        {
            questStartZ = player.position.z;
        }

        lastDistanceProgress = 0;
    }

    private void TrackDistanceQuest()
    {
        if (currentQuest == null)
        {
            return;
        }

        if (currentQuest.isCompleted)
        {
            return;
        }

        if (currentQuest.questType != QuestType.Distance)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        int currentDistance = Mathf.FloorToInt(player.position.z - questStartZ);

        if (currentDistance <= lastDistanceProgress)
        {
            return;
        }

        int progressDifference = currentDistance - lastDistanceProgress;
        lastDistanceProgress = currentDistance;

        AddProgressToCurrentQuest(progressDifference);
    }

    private void HandleCorrectRhythmMove()
    {
        if (!CanProgressQuest())
        {
            return;
        }

        if (currentQuest.questType != QuestType.CorrectRhythmMoves)
        {
            return;
        }

        AddProgressToCurrentQuest(1);
    }

    private void HandleObstaclePassed()
    {
        if (!CanProgressQuest())
        {
            return;
        }

        if (currentQuest.questType != QuestType.ObstaclesPassed)
        {
            return;
        }

        AddProgressToCurrentQuest(1);
    }

    private bool CanProgressQuest()
    {
        if (currentQuest == null)
        {
            return false;
        }

        if (currentQuest.isCompleted)
        {
            return false;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return false;
        }

        return true;
    }

    private void AddProgressToCurrentQuest(int amount)
    {
        currentQuest.AddProgress(amount);

        OnQuestUpdated?.Invoke(currentQuest);

        if (currentQuest.isCompleted)
        {
            CompleteCurrentQuest();
        }
    }

    private void CompleteCurrentQuest()
    {
        Debug.Log("Görev tamamlandı: " + currentQuest.questName);

        OnQuestCompleted?.Invoke(currentQuest);

        Invoke(nameof(StartNextQuest), nextQuestDelay);
    }

    private void StartNextQuest()
    {
        StartQuest(currentQuestIndex + 1);
    }

    public Quest GetCurrentQuest()
    {
        return currentQuest;
    }
}
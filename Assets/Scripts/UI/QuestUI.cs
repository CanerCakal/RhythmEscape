using System.Collections;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private TextMeshProUGUI questProgressText;
    [SerializeField] private TextMeshProUGUI questCompletedText;

    [Header("Animation Settings")]
    [SerializeField] private float completedTextDuration = 1.2f;
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchDuration = 0.15f;

    private bool isSubscribed = false;

    private Vector3 titleOriginalScale;
    private Coroutine punchCoroutine;
    private Coroutine completedTextCoroutine;

    private void Awake()
    {
        if (questTitleText != null)
        {
            titleOriginalScale = questTitleText.transform.localScale;
        }

        if (questCompletedText != null)
        {
            questCompletedText.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        TrySubscribeToQuestManager();
    }

    private void OnEnable()
    {
        TrySubscribeToQuestManager();
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null && isSubscribed)
        {
            QuestManager.Instance.OnQuestUpdated -= UpdateQuestUI;
            QuestManager.Instance.OnQuestCompleted -= ShowCompletedText;
            QuestManager.Instance.OnAllQuestsCompleted -= ShowAllQuestsCompleted;
            isSubscribed = false;
        }
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribeToQuestManager();
        }
    }

    private void TrySubscribeToQuestManager()
    {
        if (isSubscribed)
        {
            return;
        }

        if (QuestManager.Instance == null)
        {
            return;
        }

        QuestManager.Instance.OnQuestUpdated += UpdateQuestUI;
        QuestManager.Instance.OnQuestCompleted += ShowCompletedText;
        QuestManager.Instance.OnAllQuestsCompleted += ShowAllQuestsCompleted;

        isSubscribed = true;

        Quest currentQuest = QuestManager.Instance.GetCurrentQuest();

        if (currentQuest != null)
        {
            UpdateQuestUI(currentQuest);
        }

        Debug.Log("QuestUI QuestManager'a bağlandı.");
    }

    private void UpdateQuestUI(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        if (questTitleText != null)
        {
            questTitleText.text = quest.questName;
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.text = quest.description;
        }

        if (questProgressText != null)
        {
            questProgressText.text = quest.GetProgressText();
        }

        PlayTitlePunch();
    }

    private void ShowCompletedText(Quest quest)
    {
        if (questCompletedText == null)
        {
            return;
        }

        if (completedTextCoroutine != null)
        {
            StopCoroutine(completedTextCoroutine);
        }

        completedTextCoroutine = StartCoroutine(CompletedTextRoutine());
    }

    private IEnumerator CompletedTextRoutine()
    {
        questCompletedText.gameObject.SetActive(true);
        questCompletedText.text = "GÖREV TAMAMLANDI!";

        float timer = 0f;

        while (timer < completedTextDuration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        questCompletedText.gameObject.SetActive(false);
    }

    private void ShowAllQuestsCompleted()
    {
        if (questTitleText != null)
        {
            questTitleText.text = "Tüm Görevler Tamamlandı!";
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.text = "Harika ilerledin.";
        }

        if (questProgressText != null)
        {
            questProgressText.text = "";
        }
    }

    private void PlayTitlePunch()
    {
        if (questTitleText == null)
        {
            return;
        }

        if (punchCoroutine != null)
        {
            StopCoroutine(punchCoroutine);
        }

        punchCoroutine = StartCoroutine(TitlePunchRoutine());
    }

    private IEnumerator TitlePunchRoutine()
    {
        questTitleText.transform.localScale = titleOriginalScale * punchScale;

        float timer = 0f;

        while (timer < punchDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / punchDuration;

            questTitleText.transform.localScale = Vector3.Lerp(
                titleOriginalScale * punchScale,
                titleOriginalScale,
                t
            );

            yield return null;
        }

        questTitleText.transform.localScale = titleOriginalScale;
    }
}
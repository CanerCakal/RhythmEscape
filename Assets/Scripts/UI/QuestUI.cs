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

    [Header("Quest Complete Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip questCompletedClip;

    [Header("Panel Animation")]
    [SerializeField] private RectTransform questPanel;
    [SerializeField] private float panelPunchScale = 1.08f;
    [SerializeField] private float panelPunchDuration = 0.18f;

    private bool isSubscribed = false;

    private Vector3 titleOriginalScale;
    private Coroutine punchCoroutine;
    private Coroutine completedTextCoroutine;
    private Vector3 panelOriginalScale;
    private Coroutine panelPunchCoroutine;

    private void Awake()
    {
        if (questTitleText != null)
        {
            titleOriginalScale = questTitleText.transform.localScale;
        }

        if (questPanel != null)
        {
            panelOriginalScale = questPanel.localScale;
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
        PlayQuestCompletedSound();
        PlayPanelPunch();

        if (questCompletedText == null)
        {
            return;
        }

        if (completedTextCoroutine != null)
        {
            StopCoroutine(completedTextCoroutine);
        }

        completedTextCoroutine = StartCoroutine(CompletedTextRoutine(quest));
    }

    private IEnumerator CompletedTextRoutine(Quest quest)
    {
        questCompletedText.gameObject.SetActive(true);
        questCompletedText.text = "GÖREV TAMAMLANDI!\n+" + quest.rewardScore + " SCORE";

        Vector3 originalScale = Vector3.one;
        Vector3 startScale = Vector3.one * 1.25f;

        questCompletedText.transform.localScale = startScale;

        float timer = 0f;

        while (timer < completedTextDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / completedTextDuration;

            questCompletedText.transform.localScale = Vector3.Lerp(
                startScale,
                originalScale,
                t
            );

            Color currentColor = questCompletedText.color;
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            questCompletedText.color = currentColor;

            yield return null;
        }

        Color resetColor = questCompletedText.color;
        resetColor.a = 1f;
        questCompletedText.color = resetColor;

        questCompletedText.transform.localScale = originalScale;
        questCompletedText.gameObject.SetActive(false);
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
    private void PlayQuestCompletedSound()
    {
        if (audioSource == null)
        {
            return;
        }

        if (questCompletedClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(questCompletedClip);
    }
    private void PlayPanelPunch()
    {
        if (questPanel == null)
        {
            return;
        }

        if (panelPunchCoroutine != null)
        {
            StopCoroutine(panelPunchCoroutine);
        }

        panelPunchCoroutine = StartCoroutine(PanelPunchRoutine());
    }

    private IEnumerator PanelPunchRoutine()
    {
        questPanel.localScale = panelOriginalScale * panelPunchScale;

        float timer = 0f;

        while (timer < panelPunchDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / panelPunchDuration;

            questPanel.localScale = Vector3.Lerp(
                panelOriginalScale * panelPunchScale,
                panelOriginalScale,
                t
            );

            yield return null;
        }

        questPanel.localScale = panelOriginalScale;
    }
}
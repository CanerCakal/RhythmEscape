using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicSelectController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Track List")]
    [SerializeField] private MusicTrackData[] availableTracks;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI trackNameText;
    [SerializeField] private TextMeshProUGUI artistNameText;
    [SerializeField] private TextMeshProUGUI bpmText;
    [SerializeField] private TextMeshProUGUI difficultyText;

    [Header("Preview Audio")]
    [SerializeField] private AudioSource previewAudioSource;

    [Header("Buttons")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button backButton;

    private int currentTrackIndex = 0;

    private void Start()
    {
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(ShowPreviousTrack);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(ShowNextTrack);
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(PlaySelectedTrack);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMainMenu);
        }

        UpdateTrackUI();
    }

    private void OnDisable()
    {
        if (previousButton != null)
        {
            previousButton.onClick.RemoveListener(ShowPreviousTrack);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(ShowNextTrack);
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveListener(PlaySelectedTrack);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(BackToMainMenu);
        }
    }

    private void ShowPreviousTrack()
    {
        if (!HasTracks())
        {
            return;
        }

        currentTrackIndex--;

        if (currentTrackIndex < 0)
        {
            currentTrackIndex = availableTracks.Length - 1;
        }

        UpdateTrackUI();
    }

    private void ShowNextTrack()
    {
        if (!HasTracks())
        {
            return;
        }

        currentTrackIndex++;

        if (currentTrackIndex >= availableTracks.Length)
        {
            currentTrackIndex = 0;
        }

        UpdateTrackUI();
    }

    private void PlaySelectedTrack()
    {
        if (!HasTracks())
        {
            Debug.LogWarning("MusicSelectController: Seçilebilir müzik yok.");
            return;
        }

        MusicTrackData selectedTrack = availableTracks[currentTrackIndex];

        if (selectedTrack == null)
        {
            Debug.LogWarning("MusicSelectController: Seçilen track boş.");
            return;
        }

        SelectedMusicManager.SelectTrack(selectedTrack);

        SceneManager.LoadScene(gameSceneName);
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void UpdateTrackUI()
    {
        if (!HasTracks())
        {
            SetEmptyUI();
            return;
        }

        MusicTrackData track = availableTracks[currentTrackIndex];

        if (track == null)
        {
            SetEmptyUI();
            return;
        }

        if (trackNameText != null)
        {
            trackNameText.text = track.trackName;
        }

        if (artistNameText != null)
        {
            artistNameText.text = track.artistName;
        }

        if (bpmText != null)
        {
            bpmText.text = "BPM: " + track.bpm;
        }

        if (difficultyText != null)
        {
            difficultyText.text = "Level: " + track.difficultyLabel;
        }

        PlayPreview(track);
    }

    private void PlayPreview(MusicTrackData track)
    {
        if (previewAudioSource == null)
        {
            return;
        }

        if (track.audioClip == null)
        {
            previewAudioSource.Stop();
            return;
        }

        previewAudioSource.clip = track.audioClip;
        previewAudioSource.time = 0f;
        previewAudioSource.volume = 0.35f;
        previewAudioSource.Play();
    }

    private void SetEmptyUI()
    {
        if (trackNameText != null)
        {
            trackNameText.text = "No Track";
        }

        if (artistNameText != null)
        {
            artistNameText.text = "-";
        }

        if (bpmText != null)
        {
            bpmText.text = "BPM: -";
        }

        if (difficultyText != null)
        {
            difficultyText.text = "Level: -";
        }
    }

    private bool HasTracks()
    {
        return availableTracks != null && availableTracks.Length > 0;
    }
}
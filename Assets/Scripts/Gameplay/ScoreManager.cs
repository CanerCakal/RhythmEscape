using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int HighScore { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void AddScore(InputTiming.TimingResult result)
    {
        switch (result)
        {
            case InputTiming.TimingResult.Perfect:
                Score += 100;
                Combo++;
                break;

            case InputTiming.TimingResult.Good:
                Score += 50;
                Combo++;
                break;

            case InputTiming.TimingResult.Miss:
                Combo = 0;
                break;
        }

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt("HighScore", HighScore);
        }

        Debug.Log("Score: " + Score + " | Combo: " + Combo);

        UIManager.Instance.UpdateScore(Score);
        UIManager.Instance.UpdateCombo(Combo);
    }

}
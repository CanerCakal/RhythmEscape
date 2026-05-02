using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    private bool isDead = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && !isDead)
        {
            isDead = true;
            GameOver();
        }
    }
    void GameOver()
    {
        Time.timeScale = 0f;
        UIManager.Instance.ShowGameOver(
            ScoreManager.Instance.Score,
            ScoreManager.Instance.HighScore
        );
    }

}

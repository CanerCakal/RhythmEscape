using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Pass Settings")]
    [SerializeField] private float passDistanceBehindPlayer = 1.5f;

    private Transform player;
    private bool hasPassedPlayer = false;

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        CheckIfPassedPlayer();
    }

    private void CheckIfPassedPlayer()
    {
        if (hasPassedPlayer)
        {
            return;
        }

        if (transform.position.z < player.position.z - passDistanceBehindPlayer)
        {
            hasPassedPlayer = true;

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddObstaclePassedScore();
            }

            Destroy(gameObject);
        }
    }
}
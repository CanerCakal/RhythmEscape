using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over!");

            // Şimdilik sahneyi yeniden başlat
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
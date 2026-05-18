using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHazard hazard = other.GetComponent<PlayerHazard>();

        if (hazard == null)
        {
            hazard = other.GetComponentInParent<PlayerHazard>();
        }

        if (hazard == null)
        {
            Debug.Log(
                "Player zararsız objeye temas etti: " +
                other.name +
                " | Tag: " +
                other.tag +
                " | Root: " +
                other.transform.root.name
            );

            return;
        }

        Debug.Log(
            "PlayerHazard temas algılandı: " +
            other.name +
            " | Root: " +
            other.transform.root.name
        );

        if (!hazard.CausesGameOver)
        {
            return;
        }

        if (PlayerInvincibility.Instance != null && PlayerInvincibility.Instance.IsInvincible)
        {
            Debug.Log("Oyuncu dokunulmaz. Engel hasarı engellendi: " + other.name);
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogWarning("GameManager sahnede bulunamadı.");
        }
    }
}
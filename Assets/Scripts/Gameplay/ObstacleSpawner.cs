using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab; // Engel prefabı
    public float laneDistance = 3f;   // Şeritler arası mesafe
    public float spawnZ = 20f;        // Engellerin ne kadar ileride oluşacağı

    void Start()
    {
        // HATA BURADAYDI: BeatManager.Instance eklendi
        if (BeatManager.Instance != null)
        {
            // Her ritim vuruşunda SpawnObstacle fonksiyonunu çağırır
            BeatManager.Instance.OnBeat.AddListener(SpawnObstacle);
        }
        else
        {
            Debug.LogError("Sahnede BeatManager bulunamadı! Lütfen kontrol et.");
        }
    }

    void SpawnObstacle()
    {
        // Rastgele bir şerit seç (0: Sol, 1: Orta, 2: Sağ)
        int randomLane = Random.Range(0, 3);
        float xPos = (randomLane - 1) * laneDistance;

        Vector3 spawnPos = new Vector3(xPos, 0.5f, transform.position.z + spawnZ);

        // Engeli oluştur
        if (obstaclePrefab != null)
        {
            Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        }
    }
}
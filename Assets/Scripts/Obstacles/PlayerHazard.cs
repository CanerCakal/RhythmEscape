using UnityEngine;

public class PlayerHazard : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private bool causesGameOver = true;

    public bool CausesGameOver => causesGameOver;
}
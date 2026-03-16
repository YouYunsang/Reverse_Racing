using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerScoreSystem playerScoreSystem;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text distanceScoreText;
    [SerializeField] private TMP_Text parryScoreText;

    private void Awake()
    {
        if (playerScoreSystem == null)
        {
            playerScoreSystem = FindFirstObjectByType<PlayerScoreSystem>();
        }
    }

    private void Update()
    {
        if (playerScoreSystem == null)
            return;

        if (totalScoreText != null)
        {
            totalScoreText.text = $"SCORE : {playerScoreSystem.TotalScore:N0}";
        }

        if (distanceScoreText != null)
        {
            distanceScoreText.text = $"DIST : {playerScoreSystem.DistanceScore:N0}";
        }

        if (parryScoreText != null)
        {
            parryScoreText.text = $"PARRY : {playerScoreSystem.ParryScore:N0}";
        }
    }
}
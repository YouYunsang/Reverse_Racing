using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerScoreSystem playerScoreSystem;
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Slow Down")]
    [SerializeField] private float forwardSlowDownRate = 8f;
    [SerializeField] private float laneSlowDownRate = 10f;
    [SerializeField] private float stopThreshold = 0.05f;

    private bool isGameOverSequenceStarted = false;
    private bool isStoppingPlayer = false;

    public bool IsGameOverSequenceStarted => isGameOverSequenceStarted;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerScoreSystem == null)
            playerScoreSystem = GetComponent<PlayerScoreSystem>();
    }

    private void Update()
    {
        if (!isStoppingPlayer || playerMovement == null)
            return;

        SlowDownPlayer();
    }

    public void StartGameOverSequence()
    {
        if (isGameOverSequenceStarted)
            return;

        isGameOverSequenceStarted = true;
        isStoppingPlayer = true;
    }

    private void SlowDownPlayer()
    {
        float newForward = Mathf.MoveTowards(
            playerMovement.ForwardMoveSpeed,
            0f,
            forwardSlowDownRate * Time.deltaTime
        );

        float newLane = Mathf.MoveTowards(
            playerMovement.LaneChangeSpeed,
            0f,
            laneSlowDownRate * Time.deltaTime
        );

        playerMovement.SetForwardMoveSpeed(newForward);
        playerMovement.SetLaneChangeSpeed(newLane);

        bool isForwardStopped = newForward <= stopThreshold;
        bool isLaneStopped = newLane <= stopThreshold;

        if (isForwardStopped && isLaneStopped)
        {
            playerMovement.StopAllMovementImmediate();
            FinishGameOver();
        }
    }

    private void FinishGameOver()
    {
        isStoppingPlayer = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        if (gameOverUI != null)
        {
            int totalScore = playerScoreSystem != null ? playerScoreSystem.TotalScore : 0;
            gameOverUI.Show(totalScore);
        }

        Time.timeScale = 0f;
    }
}
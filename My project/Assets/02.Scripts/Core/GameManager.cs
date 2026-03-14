using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState CurrentState { get; private set; } = GameState.Ready;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
    }

    public void GameOver()
    {
        if (CurrentState == GameState.GameOver)
            return;

        CurrentState = GameState.GameOver;
    }

    public bool IsPlaying()
    {
        return CurrentState == GameState.Playing;
    }
}
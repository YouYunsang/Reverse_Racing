using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Texts")]
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text totalScoreText;

    private void Awake()
    {
        Hide();
    }

    public void Show(int totalScore)
    {
        if (root != null)
            root.SetActive(true);
        else
            gameObject.SetActive(true);

        if (gameOverText != null)
            gameOverText.text = "GAME OVER";

        if (totalScoreText != null)
            totalScoreText.text = $"Total Score : {totalScore:N0}";
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
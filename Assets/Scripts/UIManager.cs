using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Text")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text timerText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateUI(
        int score,
        int lives,
        int round,
        float timer)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (livesText != null)
            livesText.text = "Lives: " + lives;

        if (roundText != null)
            roundText.text = "Round: " + round;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timer);
    }
}
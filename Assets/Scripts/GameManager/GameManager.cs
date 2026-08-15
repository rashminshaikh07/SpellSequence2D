using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;

    [Header("Game Settings")]
    public int score = 0;
    public int lives = 3;
    public int round = 1;
    public float timer = 60f;
    public int maxRounds = 5;

    [Header("Pause")]
    public GameObject pausePanel;

    private bool gameOver = false;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        UIManager.Instance.UpdateUI(score, lives, round, timer);
    }

    void Update()
    {
        if (gameOver)
            return;

        if (isPaused)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            GameOver();
        }

        UIManager.Instance.UpdateUI(score, lives, round, timer);
    }

    public void AddScore(int points)
    {
        score += points;

        UIManager.Instance.UpdateUI(
            score,
            lives,
            round,
            timer
        );
    }

    public void LoseLife()
    {
        lives--;

        UIManager.Instance.UpdateUI(
            score,
            lives,
            round,
            timer
        );

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (gameOver)
            return;

        isPaused = true;

        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void GameOver()
    {
        gameOver = true;

        Time.timeScale = 1f;

        SceneManager.LoadScene("LoseScene");
    }

    void WinGame()
    {
        gameOver = true;

        Time.timeScale = 1f;

        SceneManager.LoadScene("WinScene");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("GameScene");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}
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
    private bool timerRunning = false;

    void Awake()
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

    void Start()
    {
        Time.timeScale = 1f;

        isPaused = false;
        timerRunning = false;
        gameOver = false;

        // Hide pause panel when game starts
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI(
                score,
                lives,
                round,
                timer
            );
        }
    }

    void Update()
    {
        if (gameOver)
            return;

        if (isPaused)
            return;

        // Timer does not run until Simon has finished
        if (!timerRunning)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;

            GameOver();

            return;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI(
                score,
                lives,
                round,
                timer
            );
        }
    }

    // =========================================================
    // TIMER
    // =========================================================

    public void StartPlayerTimer()
    {
        if (gameOver)
            return;

        timerRunning = true;

        Debug.Log("PLAYER TIMER STARTED");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI(
                score,
                lives,
                round,
                timer
            );
        }
    }

    public void StopPlayerTimer()
    {
        timerRunning = false;
    }

    // =========================================================
    // SCORE
    // =========================================================

    public void AddScore(int points)
    {
        score += points;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI(
                score,
                lives,
                round,
                timer
            );
        }
    }

    // =========================================================
    // LIVES
    // =========================================================

    public void LoseLife()
    {
        lives--;

        if (lives < 0)
            lives = 0;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI(
                score,
                lives,
                round,
                timer
            );
        }

        Debug.Log("Life lost. Remaining lives: " + lives);

        if (lives <= 0)
        {
            GameOver();
        }
    }

    // =========================================================
    // PAUSE
    // =========================================================

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

    // =========================================================
    // LOSE
    // =========================================================

    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;

        StopPlayerTimer();

        // Reset time before changing scene
        Time.timeScale = 1f;

        Debug.Log("GAME OVER - Loading LoseScene");

        SceneManager.LoadScene("LoseScene");
    }

    // =========================================================
    // WIN
    // =========================================================

    public void WinGame()
    {
        if (gameOver)
            return;

        gameOver = true;

        StopPlayerTimer();

        // Reset time before changing scene
        Time.timeScale = 1f;

        Debug.Log("GAME WON - Loading WinScene");

        SceneManager.LoadScene("WinScene");
    }

    // =========================================================
    // RESTART
    // =========================================================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("GameScene");
    }

    // =========================================================
    // MAIN MENU
    // =========================================================

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int startingRound = 1;
    [SerializeField] private int maxRounds = 5;
    [SerializeField] private float startingTime = 10f;

    [Header("Score")]
    [SerializeField] private int matchScore = 10;
    [SerializeField] private int simonScore = 20;
    [SerializeField] private int roundBonus = 50;

    // Current game values
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int Round { get; private set; }
    public float Timer { get; private set; }

    public bool GameOver { get; private set; }

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

    private void Start()
    {
        StartNewGame();
    }

    private void Update()
    {
        if (GameOver)
            return;

        UpdateTimer();
    }

    // -----------------------------
    // GAME START
    // -----------------------------

    public void StartNewGame()
    {
        Score = 0;
        Lives = startingLives;
        Round = startingRound;
        Timer = startingTime;
        GameOver = false;

        UpdateUI();
    }

    // -----------------------------
    // TIMER
    // -----------------------------

    private void UpdateTimer()
    {
        Timer -= Time.deltaTime;

        if (Timer <= 0f)
        {
            Timer = 0f;
            UpdateUI();
            LoseGame();
            return;
        }

        UpdateUI();
    }

    // -----------------------------
    // SCORE
    // -----------------------------

    public void AddMatchScore()
    {
        AddScore(matchScore);
    }

    public void AddSimonScore()
    {
        AddScore(simonScore);
    }

    public void AddRoundBonus()
    {
        AddScore(roundBonus);
    }

    public void AddScore(int amount)
    {
        Score += amount;
        UpdateUI();
    }

    // -----------------------------
    // LIVES
    // -----------------------------

    public void LoseLife()
    {
        if (GameOver)
            return;

        Lives--;

        if (Lives <= 0)
        {
            Lives = 0;
            UpdateUI();
            LoseGame();
            return;
        }

        UpdateUI();
    }

    // -----------------------------
    // ROUND
    // -----------------------------

    public void CompleteRound()
    {
        if (GameOver)
            return;

        AddRoundBonus();

        if (Round >= maxRounds)
        {
            WinGame();
            return;
        }

        Round++;

        UpdateUI();
    }

    // -----------------------------
    // WIN
    // -----------------------------

    public void WinGame()
    {
        if (GameOver)
            return;

        GameOver = true;

        SceneManager.LoadScene("WinScene");
    }

    // -----------------------------
    // LOSE
    // -----------------------------

    public void LoseGame()
    {
        if (GameOver)
            return;

        GameOver = true;

        SceneManager.LoadScene("LoseScene");
    }

    // -----------------------------
    // RESTART
    // -----------------------------

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("GameScene");
    }

    // -----------------------------
    // MAIN MENU
    // -----------------------------

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    // -----------------------------
    // UI
    // -----------------------------

    private void UpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI(
                Score,
                Lives,
                Round,
                Timer
            );
        }
    }
}
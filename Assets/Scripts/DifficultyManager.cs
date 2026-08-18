using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    // =========================================================
    // DIFFICULTY
    // =========================================================

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [Header("Current Difficulty")]
    public Difficulty currentDifficulty = Difficulty.Easy;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        // Make this object available between scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


    // =========================================================
    // SET EASY
    // =========================================================

    public void SetEasy()
    {
        SetDifficulty(Difficulty.Easy);
    }


    // =========================================================
    // SET MEDIUM
    // =========================================================

    public void SetMedium()
    {
        SetDifficulty(Difficulty.Medium);
    }


    // =========================================================
    // SET HARD
    // =========================================================

    public void SetHard()
    {
        SetDifficulty(Difficulty.Hard);
    }


    // =========================================================
    // SET DIFFICULTY
    // =========================================================

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;

        Debug.Log(
            "DifficultyManager: Difficulty changed to " +
            currentDifficulty
        );
    }


    // =========================================================
    // GET GRID COLUMNS
    // =========================================================

    public int GetColumns()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                return 3;

            case Difficulty.Medium:
                return 4;

            case Difficulty.Hard:
                return 4;
        }

        return 3;
    }


    // =========================================================
    // GET GRID ROWS
    // =========================================================

    public int GetRows()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                return 2;

            case Difficulty.Medium:
                return 3;

            case Difficulty.Hard:
                return 4;
        }

        return 2;
    }


    // =========================================================
    // GET MAXIMUM ROUNDS
    // =========================================================

    public int GetMaximumRounds()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                return 3;

            case Difficulty.Medium:
                return 6;

            case Difficulty.Hard:
                return 8;
        }

        return 3;
    }
}
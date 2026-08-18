using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public Difficulty currentDifficulty;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetEasy()
    {
        currentDifficulty = Difficulty.Easy;
        Debug.Log("Difficulty selected: EASY");
    }

    public void SetMedium()
    {
        currentDifficulty = Difficulty.Medium;
        Debug.Log("Difficulty selected: MEDIUM");
    }

    public void SetHard()
    {
        currentDifficulty = Difficulty.Hard;
        Debug.Log("Difficulty selected: HARD");
    }
}
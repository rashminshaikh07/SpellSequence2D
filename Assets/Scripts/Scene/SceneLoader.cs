using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadHowToPlay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HowToPlay");
    }
    public void LoadLevels()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelManager");
    }
    public void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void LoadSettings()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SettingScene");
    }

    public void LoadWin()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScene");
    }

    public void LoadLose()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoseScene");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
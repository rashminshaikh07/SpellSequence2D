using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public AudioSource buttonClickSource;
    public AudioSource cardFlipSource;

    void Awake()
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

    public void PlayButtonClick()
    {
        buttonClickSource.Play();
    }

    public void PlayCardFlip()
    {
        cardFlipSource.Play();
    }
}
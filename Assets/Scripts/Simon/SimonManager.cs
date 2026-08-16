using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonManager : MonoBehaviour
{
    public static SimonManager Instance;

    [Header("References")]
    public CardManager cardManager;

    [Header("Simon")]
    public float revealTime = 1.2f;
    public float gapBetweenCards = 0.2f;

    [Header("Initial Memory")]
    public float getReadyTime = 2f;
    public float memoryTime = 10f;

    [Header("UI")]
    public GameObject gameMessage;
    public TMPro.TMP_Text gameMessageText;

    private List<int> sequence = new List<int>();

    private int currentSequenceIndex = 0;

    private Cards firstSelectedCard = null;

    private bool playerCanInput = false;
    private bool gameStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (cardManager == null)
            cardManager = FindFirstObjectByType<CardManager>();

        StartCoroutine(StartGame());
    }

    // =========================================================
    // GAME START
    // =========================================================

    private IEnumerator StartGame()
    {
        yield return null;

        if (cardManager == null)
        {
            Debug.LogError("SimonManager: CardManager is missing.");
            yield break;
        }

        cardManager.HideAllCards();

        playerCanInput = false;
        gameStarted = false;

        ShowMessage("Get Ready!");

        yield return new WaitForSeconds(getReadyTime);

        HideMessage();

        // Show all 12 cards for memorization.
        foreach (Cards card in cardManager.GetCards())
            card.Reveal();

        Debug.Log("Initial memory phase started.");

        yield return new WaitForSeconds(memoryTime);

        // Hide all 12 cards.
        cardManager.HideAllCards();

        Debug.Log("Initial memory phase finished.");

        gameStarted = true;

        StartNewRound();
    }

    // =========================================================
    // START ROUND
    // =========================================================

    private void StartNewRound()
    {
        if (!gameStarted)
            return;

        playerCanInput = false;
        firstSelectedCard = null;

        if (GameManagerScript.Instance != null)
            GameManagerScript.Instance.StopPlayerTimer();

        cardManager.HideAllCards();

        // Add exactly ONE new card to the sequence.
        AddNewSequenceCard();

        Debug.Log(
            "ROUND " +
            GameManagerScript.Instance.round +
            " | Sequence: " +
            SequenceText()
        );

        StartCoroutine(ShowSimonSequence());
    }

    private void AddNewSequenceCard()
    {
        List<int> available = new List<int>();

        for (int id = 0; id < 6; id++)
        {
            if (!sequence.Contains(id))
                available.Add(id);
        }

        if (available.Count == 0)
        {
            Debug.Log("All 6 card types are already in the sequence.");
            return;
        }

        int newID = available[Random.Range(0, available.Count)];

        sequence.Add(newID);
    }

    // =========================================================
    // SIMON SHOWS WHOLE SEQUENCE
    // =========================================================

    private IEnumerator ShowSimonSequence()
    {
        playerCanInput = false;

        Debug.Log("Simon showing: " + SequenceText());

        // Show each sequence card.
        foreach (int id in sequence)
        {
            Cards card = FindCardByID(id);

            if (card == null)
                continue;

            card.Reveal();

            yield return new WaitForSeconds(revealTime);

            card.Hide();

            yield return new WaitForSeconds(gapBetweenCards);
        }

        // Make absolutely sure Simon's cards are hidden.
        cardManager.HideAllCards();

        currentSequenceIndex = 0;
        firstSelectedCard = null;

        playerCanInput = true;

        if (GameManagerScript.Instance != null)
            GameManagerScript.Instance.StartPlayerTimer();

        Debug.Log(
            "PLAYER TURN. Expected ID = " +
            sequence[currentSequenceIndex]
        );
    }

    // =========================================================
    // PLAYER CLICKS
    // =========================================================

    public void CardClicked(Cards clickedCard)
    {
        if (!playerCanInput || clickedCard == null)
            return;

        int expectedID = sequence[currentSequenceIndex];

        // -----------------------------------------------------
        // FIRST CARD OF CURRENT PAIR
        // -----------------------------------------------------

        if (firstSelectedCard == null)
        {
            // Wrong sequence.
            if (clickedCard.cardID != expectedID)
            {
                clickedCard.Reveal();

                StartCoroutine(
                    WrongMoveRoutine(clickedCard)
                );

                return;
            }

            // Correct first card.
            firstSelectedCard = clickedCard;
            firstSelectedCard.Reveal();

            Debug.Log(
                "Correct sequence card: " +
                clickedCard.cardID
            );

            return;
        }

        // -----------------------------------------------------
        // SECOND CARD OF PAIR
        // -----------------------------------------------------

        if (clickedCard == firstSelectedCard)
        {
            StartCoroutine(
                WrongMoveRoutine(clickedCard)
            );

            return;
        }

        // Reveal the second card even if it is wrong.
        clickedCard.Reveal();

        // Wrong matching pair.
        if (clickedCard.cardID != firstSelectedCard.cardID)
        {
            StartCoroutine(
                WrongPairRoutine(clickedCard)
            );

            return;
        }

        // -----------------------------------------------------
        // CORRECT PAIR
        // -----------------------------------------------------

        Debug.Log(
            "Correct pair: " +
            clickedCard.cardID
        );

        // Keep BOTH cards face-up.
        // We intentionally do NOT hide them here.

        firstSelectedCard = null;

        currentSequenceIndex++;

        // Entire sequence completed.
        if (currentSequenceIndex >= sequence.Count)
        {
            StartCoroutine(CompleteRound());
            return;
        }

        // Move to the next Simon card.
        Debug.Log(
            "Next expected card ID = " +
            sequence[currentSequenceIndex]
        );
    }

    // =========================================================
    // WRONG FIRST CARD
    // =========================================================

    private IEnumerator WrongMoveRoutine(Cards wrongCard)
    {
        playerCanInput = false;

        Debug.Log(
            "WRONG SEQUENCE. Clicked ID = " +
            wrongCard.cardID +
            " | Expected ID = " +
            sequence[currentSequenceIndex]
        );

        // Leave the wrong card visible briefly.
        yield return new WaitForSeconds(0.6f);

        cardManager.HideAllCards();

        firstSelectedCard = null;

        LoseLifeAndRetry();
    }

    // =========================================================
    // WRONG MATCHING PAIR
    // =========================================================

    private IEnumerator WrongPairRoutine(Cards wrongCard)
    {
        playerCanInput = false;

        Debug.Log(
            "WRONG MATCH. Expected pair ID = " +
            firstSelectedCard.cardID +
            " | Clicked ID = " +
            wrongCard.cardID
        );

        // Both cards remain visible briefly.
        yield return new WaitForSeconds(0.8f);

        cardManager.HideAllCards();

        firstSelectedCard = null;

        LoseLifeAndRetry();
    }

    // =========================================================
    // LOSE LIFE + RESTART SAME SEQUENCE
    // =========================================================

    private void LoseLifeAndRetry()
    {
        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.StopPlayerTimer();
            GameManagerScript.Instance.LoseLife();
        }

        // If lives reached zero, GameManager loads LoseScene.
        if (GameManagerScript.Instance != null &&
            GameManagerScript.Instance.lives <= 0)
        {
            return;
        }

        StartCoroutine(RetrySameSequence());
    }

    private IEnumerator RetrySameSequence()
    {
        yield return new WaitForSeconds(1f);

        cardManager.HideAllCards();

        currentSequenceIndex = 0;
        firstSelectedCard = null;

        // Same sequence again.
        StartCoroutine(ShowSimonSequence());
    }

    // =========================================================
    // ROUND COMPLETED
    // =========================================================

    private IEnumerator CompleteRound()
    {
        playerCanInput = false;

        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.StopPlayerTimer();

            // Score increases according to sequence length.
            GameManagerScript.Instance.AddScore(
                sequence.Count * 100
            );
        }

        Debug.Log(
            "ROUND COMPLETE! Sequence: " +
            SequenceText()
        );

        // Keep the successfully matched pairs visible briefly.
        yield return new WaitForSeconds(0.8f);

        // Now all cards from the completed sequence go back.
        cardManager.HideAllCards();

        if (GameManagerScript.Instance == null)
            yield break;

        // Check whether this was the final round.
        if (
            GameManagerScript.Instance.round >=
            GameManagerScript.Instance.maxRounds
        )
        {
            Debug.Log("FINAL ROUND COMPLETE!");

            GameManagerScript.Instance.WinGame();
            yield break;
        }

        // Next round.
        GameManagerScript.Instance.round++;

        yield return new WaitForSeconds(0.8f);

        StartNewRound();
    }

    // =========================================================
    // FIND CARD
    // =========================================================

    private Cards FindCardByID(int id)
    {
        foreach (Cards card in cardManager.GetCards())
        {
            if (card.cardID == id)
                return card;
        }

        return null;
    }

    // =========================================================
    // DEBUG
    // =========================================================

    private string SequenceText()
    {
        string result = "";

        for (int i = 0; i < sequence.Count; i++)
        {
            result += sequence[i];

            if (i < sequence.Count - 1)
                result += " → ";
        }

        return result;
    }

    // =========================================================
    // MESSAGE
    // =========================================================

    private void ShowMessage(string message)
    {
        if (gameMessage != null)
            gameMessage.SetActive(true);

        if (gameMessageText != null)
            gameMessageText.text = message;
    }

    private void HideMessage()
    {
        if (gameMessage != null)
            gameMessage.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonManager : MonoBehaviour
{
    public static SimonManager Instance;


    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    public CardManager cardManager;


    // =========================================================
    // SIMON SETTINGS
    // =========================================================

    [Header("Simon Settings")]

    // How long each Simon card remains visible.
    public float revealTime = 1.2f;

    // Delay between Simon cards.
    public float gapBetweenCards = 0.2f;


    // =========================================================
    // INITIAL MEMORY
    // =========================================================

    [Header("Initial Memory")]

    // "Get Ready!" duration.
    public float getReadyTime = 2f;

    // Time during which all cards are visible.
    public float memoryTime = 10f;


    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    public GameObject gameMessage;

    public TMPro.TMP_Text gameMessageText;


    // =========================================================
    // GAME VARIABLES
    // =========================================================

    // Simon sequence.
    private List<int> sequence =
        new List<int>();


    // Current position in sequence.
    private int currentSequenceIndex = 0;


    // First selected card.
    private Cards firstSelectedCard = null;


    // Player input allowed?
    private bool playerCanInput = false;


    // Has initial memory finished?
    private bool gameStarted = false;


    // Prevent duplicate Simon coroutines.
    private bool simonSequenceRunning = false;


    // Maximum rounds.
    private int maximumRounds = 3;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        // Find CardManager automatically.
        if (cardManager == null)
        {
            cardManager =
                FindFirstObjectByType<CardManager>();
        }


        if (cardManager == null)
        {
            Debug.LogError(
                "SimonManager: CardManager is missing."
            );

            return;
        }


        // Get maximum rounds from the SAME
        // DifficultyManager used by CardManager.
        SetMaximumRounds();


        StartCoroutine(
            StartGame()
        );
    }


    // =========================================================
    // DIFFICULTY BUTTON FUNCTIONS
    // =========================================================
    //
    // You can directly assign these functions to
    // your Unity UI buttons.
    //
    // Easy Button:
    // SimonManager → SetEasy()
    //
    // Medium Button:
    // SimonManager → SetMedium()
    //
    // Hard Button:
    // SimonManager → SetHard()
    //
    // =========================================================


    public void SetEasy()
    {
        ChangeDifficulty(
            DifficultyManager.Difficulty.Easy
        );
    }


    public void SetMedium()
    {
        ChangeDifficulty(
            DifficultyManager.Difficulty.Medium
        );
    }


    public void SetHard()
    {
        ChangeDifficulty(
            DifficultyManager.Difficulty.Hard
        );
    }


    // =========================================================
    // CHANGE DIFFICULTY
    // =========================================================

    private void ChangeDifficulty(
        DifficultyManager.Difficulty difficulty
    )
    {
        if (DifficultyManager.Instance == null)
        {
            Debug.LogError(
                "SimonManager: DifficultyManager not found!"
            );

            return;
        }


        // =====================================================
        // SET ONE CENTRAL DIFFICULTY
        // =====================================================

        DifficultyManager.Instance.SetDifficulty(
            difficulty
        );


        Debug.Log(
            "SimonManager: Difficulty changed to " +
            difficulty
        );


        // =====================================================
        // UPDATE GRID
        // =====================================================

        if (cardManager == null)
        {
            cardManager =
                FindFirstObjectByType<CardManager>();
        }


        if (cardManager != null)
        {
            // Destroy old Easy cards and create
            // the correct Medium/Hard cards.
            cardManager.RebuildGrid();
        }


        // =====================================================
        // UPDATE ROUND LIMIT
        // =====================================================

        SetMaximumRounds();


        // =====================================================
        // RESET SIMON
        // =====================================================

        ResetSimonGame();


        // =====================================================
        // START THE GAME AGAIN
        // =====================================================

        StartCoroutine(
            StartGame()
        );
    }


    // =========================================================
    // SET MAXIMUM ROUNDS
    // =========================================================

    private void SetMaximumRounds()
    {
        if (DifficultyManager.Instance == null)
        {
            maximumRounds = 3;

            return;
        }


        // Get rounds from central DifficultyManager.
        maximumRounds =
            DifficultyManager.Instance
            .GetMaximumRounds();


        // Safety limit.
        maximumRounds =
            Mathf.Min(
                maximumRounds,
                8
            );


        // Update GameManager.
        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.maxRounds =
                maximumRounds;
        }


        Debug.Log(
            "SimonManager: Maximum rounds = " +
            maximumRounds
        );
    }


    // =========================================================
    // RESET SIMON GAME
    // =========================================================

    private void ResetSimonGame()
    {
        // Stop currently running Simon sequences.
        StopAllCoroutines();


        // Reset game variables.
        sequence.Clear();

        currentSequenceIndex = 0;

        firstSelectedCard = null;

        playerCanInput = false;

        gameStarted = false;

        simonSequenceRunning = false;


        // Stop timer.
        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.StopPlayerTimer();

            GameManagerScript.Instance.round = 1;

            GameManagerScript.Instance.maxRounds =
                maximumRounds;
        }


        // Hide existing cards.
        if (cardManager != null)
        {
            cardManager.HideAllCards();
        }
    }


    // =========================================================
    // START GAME
    // =========================================================

    private IEnumerator StartGame()
    {
        // Give Unity time to finish creating cards.
        yield return null;


        if (cardManager == null)
        {
            Debug.LogError(
                "SimonManager: CardManager is missing."
            );

            yield break;
        }


        // Make sure round limit is correct.
        SetMaximumRounds();


        // Hide cards.
        cardManager.HideAllCards();


        playerCanInput = false;

        gameStarted = false;


        // Show Get Ready.
        ShowMessage(
            "Get Ready!"
        );


        yield return new WaitForSeconds(
            getReadyTime
        );


        HideMessage();


        // =====================================================
        // INITIAL MEMORY PHASE
        // =====================================================

        foreach (
            Cards card
            in cardManager.GetCards()
        )
        {
            if (card != null)
            {
                card.Reveal();
            }
        }


        Debug.Log(
            "Initial memory phase started."
        );


        yield return new WaitForSeconds(
            memoryTime
        );


        // Hide everything.
        cardManager.HideAllCards();


        Debug.Log(
            "Initial memory phase finished."
        );


        gameStarted = true;


        // Start from Round 1.
        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.round = 1;

            GameManagerScript.Instance.maxRounds =
                maximumRounds;
        }


        StartNewRound();
    }


    // =========================================================
    // START NEW ROUND
    // =========================================================

    private void StartNewRound()
    {
        if (!gameStarted)
        {
            return;
        }


        playerCanInput = false;

        firstSelectedCard = null;


        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.StopPlayerTimer();
        }


        // Hide board.
        cardManager.HideAllCards();


        // Add one new Simon card.
        AddNewSequenceCard();


        int currentRound = 1;


        if (GameManagerScript.Instance != null)
        {
            currentRound =
                GameManagerScript.Instance.round;
        }


        Debug.Log(
            "========================================"
        );


        Debug.Log(
            "DIFFICULTY: " +
            DifficultyManager.Instance.currentDifficulty
        );


        Debug.Log(
            "GRID: " +
            cardManager.columns +
            " x " +
            cardManager.rows
        );


        Debug.Log(
            "ROUND: " +
            currentRound +
            " / " +
            maximumRounds
        );


        Debug.Log(
            "SEQUENCE: " +
            SequenceText()
        );


        Debug.Log(
            "========================================"
        );


        StartCoroutine(
            ShowSimonSequence()
        );
    }


    // =========================================================
    // ADD NEW SIMON CARD
    // =========================================================

    private void AddNewSequenceCard()
    {
        List<int> available =
            new List<int>();


        foreach (
            Cards card
            in cardManager.GetCards()
        )
        {
            if (card == null)
            {
                continue;
            }


            // Don't use the same card type twice
            // in Simon's sequence.
            if (!sequence.Contains(card.cardID))
            {
                available.Add(
                    card.cardID
                );
            }
        }


        if (available.Count == 0)
        {
            Debug.LogError(
                "SimonManager: No unused cards available!"
            );

            return;
        }


        int newID =
            available[
                Random.Range(
                    0,
                    available.Count
                )
            ];


        sequence.Add(newID);


        Debug.Log(
            "Simon card added: " +
            newID
        );
    }


    // =========================================================
    // SHOW SIMON SEQUENCE
    // =========================================================

    private IEnumerator ShowSimonSequence()
    {
        if (simonSequenceRunning)
        {
            yield break;
        }


        simonSequenceRunning = true;


        playerCanInput = false;


        // Start hidden.
        cardManager.HideAllCards();


        for (
            int i = 0;
            i < sequence.Count;
            i++
        )
        {
            int id =
                sequence[i];


            Cards card =
                FindCardByID(id);


            if (card == null)
            {
                Debug.LogError(
                    "SimonManager: Could not find card ID " +
                    id
                );

                continue;
            }


            Debug.Log(
                "Simon showing card " +
                (i + 1) +
                "/" +
                sequence.Count +
                " | ID: " +
                id
            );


            // Hide everything before showing
            // current Simon card.
            cardManager.HideAllCards();


            yield return null;


            // Reveal current card.
            card.Reveal();


            yield return new WaitForSeconds(
                revealTime
            );


            // Hide it again.
            card.Hide();


            yield return new WaitForSeconds(
                gapBetweenCards
            );
        }


        // Make sure everything is hidden.
        cardManager.HideAllCards();


        currentSequenceIndex = 0;

        firstSelectedCard = null;


        simonSequenceRunning = false;


        // Player can now enter sequence.
        playerCanInput = true;


        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.StartPlayerTimer();
        }


        Debug.Log(
            "PLAYER TURN"
        );


        Debug.Log(
            "Expected ID: " +
            sequence[currentSequenceIndex]
        );
    }


    // =========================================================
    // PLAYER CLICKS CARD
    // =========================================================

    public void CardClicked(
        Cards clickedCard
    )
    {
        if (
            !playerCanInput ||
            clickedCard == null
        )
        {
            return;
        }


        int expectedID =
            sequence[currentSequenceIndex];


        // =====================================================
        // FIRST CARD
        // =====================================================

        if (firstSelectedCard == null)
        {
            // Wrong Simon card.
            if (
                clickedCard.cardID !=
                expectedID
            )
            {
                clickedCard.Reveal();


                StartCoroutine(
                    WrongMoveRoutine(
                        clickedCard
                    )
                );


                return;
            }


            // Correct Simon card.
            firstSelectedCard =
                clickedCard;


            firstSelectedCard.Reveal();


            Debug.Log(
                "Correct Simon card: " +
                clickedCard.cardID
            );


            return;
        }


        // =====================================================
        // SECOND CARD
        // =====================================================

        // Same physical card cannot be selected twice.
        if (
            clickedCard ==
            firstSelectedCard
        )
        {
            StartCoroutine(
                WrongMoveRoutine(
                    clickedCard
                )
            );


            return;
        }


        // Reveal second card.
        clickedCard.Reveal();


        // =====================================================
        // WRONG PAIR
        // =====================================================

        if (
            clickedCard.cardID !=
            firstSelectedCard.cardID
        )
        {
            StartCoroutine(
                WrongPairRoutine(
                    clickedCard
                )
            );


            return;
        }


        // =====================================================
        // CORRECT PAIR
        // =====================================================

        Debug.Log(
            "Correct pair: " +
            clickedCard.cardID
        );


        firstSelectedCard = null;


        // Move to next Simon card.
        currentSequenceIndex++;


        // =====================================================
        // SEQUENCE COMPLETE
        // =====================================================

        if (
            currentSequenceIndex >=
            sequence.Count
        )
        {
            StartCoroutine(
                CompleteRound()
            );


            return;
        }


        Debug.Log(
            "Next expected ID: " +
            sequence[currentSequenceIndex]
        );
    }


    // =========================================================
    // WRONG SIMON CARD
    // =========================================================

    private IEnumerator WrongMoveRoutine(
        Cards wrongCard
    )
    {
        playerCanInput = false;


        Debug.Log(
            "WRONG SEQUENCE. " +
            "Clicked: " +
            wrongCard.cardID +
            " | Expected: " +
            sequence[currentSequenceIndex]
        );


        yield return new WaitForSeconds(
            0.6f
        );


        cardManager.HideAllCards();


        firstSelectedCard = null;


        LoseLifeAndRetry();
    }


    // =========================================================
    // WRONG PAIR
    // =========================================================

    private IEnumerator WrongPairRoutine(
        Cards wrongCard
    )
    {
        playerCanInput = false;


        Debug.Log(
            "WRONG MATCH. " +
            "Expected pair: " +
            firstSelectedCard.cardID +
            " | Clicked: " +
            wrongCard.cardID
        );


        yield return new WaitForSeconds(
            0.8f
        );


        cardManager.HideAllCards();


        firstSelectedCard = null;


        LoseLifeAndRetry();
    }


    // =========================================================
    // LOSE LIFE
    // =========================================================

    private void LoseLifeAndRetry()
    {
        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.StopPlayerTimer();

            GameManagerScript.Instance.LoseLife();
        }


        if (
            GameManagerScript.Instance != null &&
            GameManagerScript.Instance.lives <= 0
        )
        {
            return;
        }


        StartCoroutine(
            RetrySameSequence()
        );
    }


    // =========================================================
    // RETRY SAME SEQUENCE
    // =========================================================

    private IEnumerator RetrySameSequence()
    {
        yield return new WaitForSeconds(
            1f
        );


        cardManager.HideAllCards();


        // Restart from beginning of SAME sequence.
        currentSequenceIndex = 0;


        firstSelectedCard = null;


        StartCoroutine(
            ShowSimonSequence()
        );
    }


    // =========================================================
    // COMPLETE ROUND
    // =========================================================

    private IEnumerator CompleteRound()
    {
        playerCanInput = false;


        if (GameManagerScript.Instance != null)
        {
            GameManagerScript.Instance.StopPlayerTimer();


            // Score:
            // Round 1 = 100
            // Round 2 = 200
            // etc.
            GameManagerScript.Instance.AddScore(
                sequence.Count * 100
            );
        }


        int currentRound = 1;


        if (GameManagerScript.Instance != null)
        {
            currentRound =
                GameManagerScript.Instance.round;
        }


        Debug.Log(
            "ROUND " +
            currentRound +
            " COMPLETE!"
        );


        yield return new WaitForSeconds(
            0.8f
        );


        cardManager.HideAllCards();


        if (GameManagerScript.Instance == null)
        {
            yield break;
        }


        // =====================================================
        // FINAL ROUND
        // =====================================================

        if (
            GameManagerScript.Instance.round >=
            maximumRounds
        )
        {
            Debug.Log(
                "FINAL ROUND COMPLETE!"
            );


            Debug.Log(
                "Difficulty: " +
                DifficultyManager.Instance.currentDifficulty
            );


            Debug.Log(
                "Maximum rounds: " +
                maximumRounds
            );


            GameManagerScript.Instance.WinGame();


            yield break;
        }


        // =====================================================
        // NEXT ROUND
        // =====================================================

        GameManagerScript.Instance.round++;


        yield return new WaitForSeconds(
            0.8f
        );


        StartNewRound();
    }


    // =========================================================
    // FIND CARD
    // =========================================================

    private Cards FindCardByID(
        int id
    )
    {
        foreach (
            Cards card
            in cardManager.GetCards()
        )
        {
            if (
                card != null &&
                card.cardID == id
            )
            {
                return card;
            }
        }


        return null;
    }


    // =========================================================
    // SEQUENCE TEXT
    // =========================================================

    private string SequenceText()
    {
        string result = "";


        for (
            int i = 0;
            i < sequence.Count;
            i++
        )
        {
            result += sequence[i];


            if (
                i <
                sequence.Count - 1
            )
            {
                result += " → ";
            }
        }


        return result;
    }


    // =========================================================
    // UI
    // =========================================================

    private void ShowMessage(
        string message
    )
    {
        if (gameMessage != null)
        {
            gameMessage.SetActive(true);
        }


        if (gameMessageText != null)
        {
            gameMessageText.text =
                message;
        }
    }


    private void HideMessage()
    {
        if (gameMessage != null)
        {
            gameMessage.SetActive(false);
        }
    }
}
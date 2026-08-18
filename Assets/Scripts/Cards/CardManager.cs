using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;
    public Transform cardParent;


    [Header("Unique Card Sprites")]
    public Sprite[] cardSprites;


    [Header("Grid Setup")]
    public int columns = 4;
    public int rows = 2;

    public float spacingX = 2.6f;
    public float spacingY = 2.0f;


    // Stores all cards currently in the game.
    private List<Cards> cards = new List<Cards>();


    // =========================================================
    // UNITY
    // =========================================================

    private void Start()
    {
        // Make sure the DifficultyManager exists.
        if (DifficultyManager.Instance == null)
        {
            Debug.LogError(
                "CardManager: DifficultyManager not found!"
            );

            return;
        }


        // Apply the selected difficulty.
        SetDifficultySettings();


        // Create the cards.
        CreateCards();
    }


    // =========================================================
    // SET DIFFICULTY SETTINGS
    // =========================================================

    public void SetDifficultySettings()
    {
        if (DifficultyManager.Instance == null)
        {
            Debug.LogWarning(
                "CardManager: DifficultyManager not found. " +
                "Using Easy settings."
            );

            columns = 3;
            rows = 2;

            return;
        }


        switch (
            DifficultyManager.Instance.currentDifficulty
        )
        {
            // =================================================
            // EASY
            // =================================================

            case DifficultyManager.Difficulty.Easy:

                columns = 3;
                rows = 2;

                spacingX = 3.0f;
                spacingY = 3.8f;

                Debug.Log(
                    "CardManager: EASY - 4 x 2 grid"
                );

                break;


            // =================================================
            // MEDIUM
            // =================================================

            case DifficultyManager.Difficulty.Medium:

                columns = 4;
                rows = 3;

                spacingX = 2.6f;
                spacingY = 2.6f;

                Debug.Log(
                    "CardManager: MEDIUM - 4 x 3 grid"
                );

                break;


            // =================================================
            // HARD
            // =================================================

            case DifficultyManager.Difficulty.Hard:

                columns = 4;
                rows = 4;

                spacingX = 2.6f;
                spacingY = 3.2f;

                Debug.Log(
                    "CardManager: HARD - 4 x 4 grid"
                );

                break;
        }
    }


    // =========================================================
    // REBUILD GRID
    // =========================================================
    //
    // This is important when the player changes difficulty
    // while the game scene is already loaded.
    //
    // =========================================================

    public void RebuildGrid()
    {
        Debug.Log(
            "CardManager: Rebuilding grid..."
        );


        // Remove old cards.
        ClearCards();


        // Get new difficulty settings.
        SetDifficultySettings();


        // Create new cards.
        CreateCards();
    }


    // =========================================================
    // CLEAR OLD CARDS
    // =========================================================

    private void ClearCards()
    {
        // Destroy every existing card GameObject.
        foreach (Cards card in cards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }


        // Clear the list.
        cards.Clear();


        // Safety check:
        // If something was created but isn't in our list,
        // remove all children from cardParent.
        if (cardParent != null)
        {
            for (
                int i = cardParent.childCount - 1;
                i >= 0;
                i--
            )
            {
                Destroy(
                    cardParent.GetChild(i).gameObject
                );
            }
        }
    }


    // =========================================================
    // CREATE CARDS
    // =========================================================

    private void CreateCards()
    {
        if (cardPrefab == null)
        {
            Debug.LogError(
                "CardManager: Card Prefab is not assigned."
            );

            return;
        }


        if (cardParent == null)
        {
            Debug.LogError(
                "CardManager: Card Parent is not assigned."
            );

            return;
        }


        // Number of pairs required.
        //
        // Easy:
        // 6 cards / 2 = 3 pairs
        //
        // Medium:
        // 12 cards / 2 = 6 pairs
        //
        // Hard:
        // 16 cards / 2 = 8 pairs

        int pairCount =
            (columns * rows) / 2;


        // Make sure enough unique sprites exist.
        if (
            cardSprites == null ||
            cardSprites.Length < pairCount
        )
        {
            Debug.LogError(
                "CardManager: Not enough unique card sprites. " +
                "Required pairs: " +
                pairCount +
                ", Available sprites: " +
                (
                    cardSprites == null
                        ? 0
                        : cardSprites.Length
                )
            );

            return;
        }


        cards.Clear();


        // =====================================================
        // CREATE PAIRS
        // =====================================================

        List<int> cardTypes =
            new List<int>();


        for (
            int i = 0;
            i < pairCount;
            i++
        )
        {
            // Add the same ID twice.
            //
            // Example for Easy:
            //
            // 0, 0
            // 1, 1
            // 2, 2

            cardTypes.Add(i);
            cardTypes.Add(i);
        }


        // Shuffle the cards.
        Shuffle(cardTypes);


        // =====================================================
        // CREATE CARD GAMEOBJECTS
        // =====================================================

        for (
            int i = 0;
            i < cardTypes.Count;
            i++
        )
        {
            GameObject newCard =
                Instantiate(
                    cardPrefab,
                    cardParent
                );


            // Calculate row.
            int row =
                i / columns;


            // Calculate column.
            int column =
                i % columns;


            // Calculate X position.
            float x =
                (
                    column -
                    (columns - 1) / 2f
                ) *
                spacingX;


            // Calculate Y position.
            float y =
                (
                    (rows - 1) / 2f -
                    row
                ) *
                spacingY;


            // Position card.
            newCard.transform.localPosition =
                new Vector3(
                    x,
                    y,
                    0f
                );


            // Find Cards component.
            Cards card =
                newCard.GetComponentInChildren<Cards>();


            if (card != null)
            {
                // Get card type.
                int cardType =
                    cardTypes[i];


                // Assign ID.
                card.cardID =
                    cardType;


                // Assign sprite.
                card.cardFront =
                    cardSprites[cardType];


                // Start hidden.
                card.Hide();


                // Add to list.
                cards.Add(card);
            }
            else
            {
                Debug.LogError(
                    "CardManager: Card prefab does not contain " +
                    "a Cards component."
                );
            }
        }


        // =====================================================
        // DEBUG INFORMATION
        // =====================================================

        Debug.Log(
            "========================================"
        );


        Debug.Log(
            "CardManager: Difficulty = " +
            (
                DifficultyManager.Instance != null
                    ? DifficultyManager.Instance.currentDifficulty.ToString()
                    : "Unknown"
            )
        );


        Debug.Log(
            "CardManager: Grid = " +
            columns +
            " x " +
            rows
        );


        Debug.Log(
            "CardManager: Total cards = " +
            cards.Count
        );


        Debug.Log(
            "CardManager: Pairs = " +
            pairCount
        );


        Debug.Log(
            "========================================"
        );
    }


    // =========================================================
    // SHUFFLE
    // =========================================================

    private void Shuffle(
        List<int> list
    )
    {
        for (
            int i = list.Count - 1;
            i > 0;
            i--
        )
        {
            int randomIndex =
                Random.Range(
                    0,
                    i + 1
                );


            int temp =
                list[i];


            list[i] =
                list[randomIndex];


            list[randomIndex] =
                temp;
        }
    }


    // =========================================================
    // GET CARDS
    // =========================================================

    public List<Cards> GetCards()
    {
        return cards;
    }


    // =========================================================
    // HIDE ALL CARDS
    // =========================================================

    public void HideAllCards()
    {
        foreach (
            Cards card
            in cards
        )
        {
            if (card != null)
            {
                card.Hide();
            }
        }
    }
}
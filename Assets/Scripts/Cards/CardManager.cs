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
    public int rows = 3;
    public float spacingX = 2.6f;
    public float spacingY = 3.5f;

    private List<Cards> cards = new List<Cards>();

    private void Start()
    {
        SetDifficultySettings();
        CreateCards();
    }

    private void SetDifficultySettings()
    {
        if (DifficultyManager.Instance == null)
        {
            Debug.LogWarning(
                "CardManager: DifficultyManager not found. Using Medium settings."
            );

            columns = 4;
            rows = 3;
            return;
        }

        switch (DifficultyManager.Instance.currentDifficulty)
        {
            case DifficultyManager.Difficulty.Easy:

                columns = 3;
                rows = 2;

                spacingX = 3.0f;
                spacingY = 3.8f;

                Debug.Log("CardManager: EASY - 3 x 2 grid");
                break;

            case DifficultyManager.Difficulty.Medium:

                columns = 4;
                rows = 3;

                spacingX = 2.6f;
                spacingY = 3.5f;

                Debug.Log("CardManager: MEDIUM - 4 x 3 grid");
                break;

            case DifficultyManager.Difficulty.Hard:

                columns = 4;
                rows = 4;

                spacingX = 2.6f;
                spacingY = 3.2f;

                Debug.Log("CardManager: HARD - 4 x 4 grid");
                break;
        }
    }

    private void CreateCards()
    {
        if (cardPrefab == null)
        {
            Debug.LogError("CardManager: Card Prefab is not assigned.");
            return;
        }

        if (cardParent == null)
        {
            Debug.LogError("CardManager: Card Parent is not assigned.");
            return;
        }

        int pairCount = (columns * rows) / 2;

        if (cardSprites == null || cardSprites.Length < pairCount)
        {
            Debug.LogError(
                "CardManager: Not enough unique card sprites. " +
                "Required pairs: " + pairCount +
                ", Available sprites: " +
                (cardSprites == null ? 0 : cardSprites.Length)
            );

            return;
        }

        cards.Clear();

        List<int> cardTypes = new List<int>();

        // Create the required number of pairs.
        for (int i = 0; i < pairCount; i++)
        {
            cardTypes.Add(i);
            cardTypes.Add(i);
        }

        Shuffle(cardTypes);

        for (int i = 0; i < cardTypes.Count; i++)
        {
            GameObject newCard =
                Instantiate(cardPrefab, cardParent);

            int row = i / columns;
            int column = i % columns;

            float x =
                (column - (columns - 1) / 2f)
                * spacingX;

            float y =
                ((rows - 1) / 2f - row)
                * spacingY;

            newCard.transform.localPosition =
                new Vector3(x, y, 0f);

            Cards card =
                newCard.GetComponentInChildren<Cards>();

            if (card != null)
            {
                int cardType = cardTypes[i];

                card.cardID = cardType;
                card.cardFront = cardSprites[cardType];

                card.Hide();

                cards.Add(card);
            }
            else
            {
                Debug.LogError(
                    "CardManager: Card prefab does not contain a Cards component."
                );
            }
        }

        Debug.Log(
            "CardManager: Created "
            + cards.Count
            + " cards."
        );
    }

    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            int temp = list[i];

            list[i] = list[randomIndex];

            list[randomIndex] = temp;
        }
    }

    public List<Cards> GetCards()
    {
        return cards;
    }

    public void HideAllCards()
    {
        foreach (Cards card in cards)
        {
            card.Hide();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;
    public Transform cardParent;

    [Header("Unique Card Sprites - 6 Pairs")]
    public Sprite[] cardSprites;

    [Header("Grid Setup")]
    public int columns = 4;
    public int rows = 3;
    public float spacingX = 2.2f;
    public float spacingY = 2.8f;

    [Header("Memory Phase")]
    public float revealTime = 10f;

    private List<Cards> cards = new List<Cards>();

    private void Start()
    {
        CreateCards();

        StartCoroutine(MemoryPhase());
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

        if (cardSprites == null || cardSprites.Length != 6)
        {
            Debug.LogError(
                "CardManager: Please assign exactly 6 unique card sprites."
            );
            return;
        }

        cards.Clear();

        List<int> cardTypes = new List<int>();

        for (int i = 0; i < 6; i++)
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

    private IEnumerator MemoryPhase()
    {
        Debug.Log("Memory Phase Started");

        // Reveal all cards.
        foreach (Cards card in cards)
        {
            card.Reveal();
        }

        // Keep them visible for the memorization period.
        yield return new WaitForSeconds(revealTime);

        // Hide all cards again.
        foreach (Cards card in cards)
        {
            card.Hide();
        }

        Debug.Log("Memory Phase Finished");
    }

    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex =
                Random.Range(0, i + 1);

            int temp = list[i];

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public List<Cards> GetCards()
    {
        return cards;
    }
}
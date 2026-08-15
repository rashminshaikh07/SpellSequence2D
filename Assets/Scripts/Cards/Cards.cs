using UnityEngine;

public class Cards : MonoBehaviour
{
    [Header("Card Information")]
    public int cardID;

    [Header("Card Visual")]
    public SpriteRenderer cardRenderer;
    public Sprite cardFront;
    public Sprite cardBack;

    private bool isRevealed = false;

    private void Awake()
    {
        // Always use the SpriteRenderer on this Card object.
        cardRenderer = GetComponent<SpriteRenderer>();

        if (cardRenderer == null)
        {
            Debug.LogError(
                "Card " + cardID +
                ": SpriteRenderer was not found on this Card object."
            );
            return;
        }

        ShowBack();
    }

    public void Reveal()
    {
        if (cardRenderer == null)
            return;

        if (cardFront == null)
        {
            Debug.LogError(
                "Card " + cardID +
                ": Card Front is NULL."
            );
            return;
        }

        isRevealed = true;

        cardRenderer.sprite = cardFront;

        Debug.Log(
            "REVEAL | ID: " +
            cardID +
            " | SpriteRenderer now has: " +
            cardRenderer.sprite.name
        );
    }

    public void Hide()
    {
        isRevealed = false;

        if (cardRenderer != null && cardBack != null)
        {
            cardRenderer.sprite = cardBack;
        }
    }

    private void ShowBack()
    {
        if (cardRenderer != null && cardBack != null)
        {
            cardRenderer.sprite = cardBack;
        }
    }

    public bool IsRevealed()
    {
        return isRevealed;
    }
}
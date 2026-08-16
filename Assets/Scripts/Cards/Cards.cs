using UnityEngine;
using UnityEngine.EventSystems;

public class Cards : MonoBehaviour, IPointerClickHandler
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

    // --------------------------------------------------
    // REVEAL CARD
    // --------------------------------------------------

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
    }

    // --------------------------------------------------
    // HIDE CARD
    // --------------------------------------------------

    public void Hide()
    {
        isRevealed = false;

        if (cardRenderer != null && cardBack != null)
        {
            cardRenderer.sprite = cardBack;
        }
    }

    // --------------------------------------------------
    // SHOW BACK
    // --------------------------------------------------

    private void ShowBack()
    {
        if (cardRenderer != null && cardBack != null)
        {
            cardRenderer.sprite = cardBack;
        }

        isRevealed = false;
    }

    // --------------------------------------------------
    // CHECK IF CARD IS CURRENTLY FACE-UP
    // --------------------------------------------------

    public bool IsRevealed()
    {
        return isRevealed;
    }

    // --------------------------------------------------
    // PLAYER CLICK
    // --------------------------------------------------

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CARD CLICKED! ID = " + cardID);

        if (SimonManager.Instance != null)
        {
            SimonManager.Instance.CardClicked(this);
        }
        else
        {
            Debug.LogError("SimonManager.Instance is NULL!");
        }
    }
}
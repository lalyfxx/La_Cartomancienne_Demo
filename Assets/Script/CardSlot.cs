using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public CardView CurrentCard { get; private set; }

    public event Action<CardView> OnCardChanged;

    public void OnDrop(PointerEventData eventData)
    {
        if (CurrentCard != null)
        {
            return;
        }

        if (eventData.pointerDrag == null)
        {
            return;
        }

        CardView card = eventData.pointerDrag.GetComponent<CardView>();
        if (card == null)
        {
            return;
        }

        Place(card);
    }

    private void Place(CardView card)
    {
        CurrentCard = card;
        card.CurrentSlot = this;

        RectTransform rt = (RectTransform)card.transform;
        rt.SetParent(transform, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        OnCardChanged?.Invoke(card);
    }

    public void Clear()
    {
        CurrentCard = null;
        OnCardChanged?.Invoke(null);
    }
}
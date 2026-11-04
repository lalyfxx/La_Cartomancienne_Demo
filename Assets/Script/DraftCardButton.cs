using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class DraftCardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Références (enfants du prefab)")]
    [SerializeField] private Image background;
    [SerializeField] private Image illustrationImage;
    [SerializeField] private TMP_Text nameText;

    [Header("Couleurs")]
    [SerializeField] private Color couleurNormale = new Color(0.25f, 0.18f, 0.35f);
    [SerializeField] private Color couleurChoisie = new Color(0.85f, 0.7f, 0.3f);

    public CardData Data { get; private set; }
    public bool Selected { get; private set; }

    private Action<DraftCardButton> onClick;

    public void Setup(CardData data, Action<DraftCardButton> clickCallback)
    {
        Data = data;
        onClick = clickCallback;

        if (nameText != null)
        {
            nameText.text = data.cardName;
        }

        if (illustrationImage != null && data.illustration != null)
        {
            illustrationImage.sprite = data.illustration;
        }

        GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(this));
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        Selected = selected;

        if (background != null)
        {
            background.color = selected ? couleurChoisie : couleurNormale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardTooltip.Instance != null)
        {
            CardTooltip.Instance.Show(Data, (RectTransform)transform);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardTooltip.Instance != null)
        {
            CardTooltip.Instance.Hide();
        }
    }
}
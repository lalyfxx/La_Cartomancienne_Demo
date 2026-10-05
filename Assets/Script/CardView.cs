using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Références (enfants du prefab)")]
    [SerializeField] private Image illustrationImage;
    [SerializeField] private TMP_Text nameText;

    public CardData Data { get; private set; }

    public CardSlot CurrentSlot { get; set; }

    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;
    private Transform handParent;

    private bool isDragging;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(CardData data, Transform hand)
    {
        Data = data;
        handParent = hand;
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

        if (nameText != null)
        {
            nameText.text = data.cardName;
        }

        if (illustrationImage != null && data.illustration != null)
        {
            illustrationImage.sprite = data.illustration;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging || CardTooltip.Instance == null)
        {
            return;
        }

        CardTooltip.Instance.Show(Data, (RectTransform)transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardTooltip.Instance != null)
        {
            CardTooltip.Instance.Hide();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        if (CardTooltip.Instance != null)
        {
            CardTooltip.Instance.Hide();
        }

        if (CurrentSlot != null)
        {
            CurrentSlot.Clear();
            CurrentSlot = null;
        }

        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        if (CurrentSlot == null)
        {
            ReturnToHand();
        }
    }

    public void ReturnToHand()
    {
        transform.SetParent(handParent, false);
    }
}
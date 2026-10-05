using UnityEngine;
using TMPro;

public class CardTooltip : MonoBehaviour
{
    public static CardTooltip Instance { get; private set; }

    [Header("Références UI")]
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Réglages")]
    [Tooltip("Espace entre le haut de la carte et la bulle")]
    [SerializeField] private float margin = 15f;

    private readonly Vector3[] corners = new Vector3[4];

    private void Awake()
    {
        Instance = this;

        panelRoot.pivot = new Vector2(0.5f, 0f);

        CanvasGroup group = panelRoot.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = panelRoot.gameObject.AddComponent<CanvasGroup>();
        }
        group.blocksRaycasts = false;
        group.interactable = false;

        Hide();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Show(CardData data, RectTransform card)
    {
        if (data == null)
        {
            return;
        }

        titleText.text = data.cardName;

        string typeLabel = data.type == CardType.Bonne ? "Bénédiction" : "Châtiment";
        descriptionText.text = data.description + "\n<i>" + typeLabel + "</i>";

        card.GetWorldCorners(corners);
        Vector3 topCenter = (corners[1] + corners[2]) * 0.5f;

        float scale = panelRoot.lossyScale.y;
        panelRoot.position = topCenter + new Vector3(0f, margin * scale, 0f);

        panelRoot.gameObject.SetActive(true);
        panelRoot.SetAsLastSibling();
    }

    public void Hide()
    {
        panelRoot.gameObject.SetActive(false);
    }
}
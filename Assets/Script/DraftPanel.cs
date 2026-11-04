using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraftPanel : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text lettresText;
    [SerializeField] private Transform zoneCartes;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private Button confirmButton;

    [Header("Prefab")]
    [SerializeField] private DraftCardButton cardButtonPrefab;

    private readonly List<DraftCardButton> boutons = new List<DraftCardButton>();
    private Action<List<CardData>> onConfirm;

    private int quotaBonnes;
    private int quotaMauvaises;
    private int quotaSpeciales;

    private void Awake()
    {
        confirmButton.onClick.AddListener(Confirm);
        panelRoot.SetActive(false);
    }

    public void Open(string titre, string lettres, List<CardData> offres,
        int bonnes, int mauvaises, int speciales, Action<List<CardData>> callback)
    {
        onConfirm = callback;

        for (int i = zoneCartes.childCount - 1; i >= 0; i--)
        {
            Destroy(zoneCartes.GetChild(i).gameObject);
        }
        boutons.Clear();

        foreach (CardData carte in offres)
        {
            DraftCardButton bouton = Instantiate(cardButtonPrefab, zoneCartes);
            bouton.Setup(carte, OnCardClicked);
            boutons.Add(bouton);
        }

        quotaBonnes = Mathf.Min(bonnes, CompterProposees(CardType.Bonne));
        quotaMauvaises = Mathf.Min(mauvaises, CompterProposees(CardType.Mauvaise));
        quotaSpeciales = Mathf.Min(speciales, CompterProposees(CardType.Speciale));

        titleText.text = titre;

        bool aDesLettres = !string.IsNullOrEmpty(lettres);
        lettresText.gameObject.SetActive(aDesLettres);
        lettresText.text = lettres;

        Refresh();
        panelRoot.SetActive(true);
    }

    private void OnCardClicked(DraftCardButton bouton)
    {
        if (bouton.Selected)
        {
            bouton.SetSelected(false);
        }
        else if (CompterChoisies(bouton.Data.type) < Quota(bouton.Data.type))
        {
            bouton.SetSelected(true);
        }

        Refresh();
    }

    private void Refresh()
    {
        int b = CompterChoisies(CardType.Bonne);
        int m = CompterChoisies(CardType.Mauvaise);
        int s = CompterChoisies(CardType.Speciale);

        counterText.text = "Bonnes " + b + "/" + quotaBonnes
            + "   ·   Mauvaises " + m + "/" + quotaMauvaises
            + "   ·   Spéciales " + s + "/" + quotaSpeciales;

        confirmButton.interactable = b == quotaBonnes && m == quotaMauvaises && s == quotaSpeciales;
    }

    private void Confirm()
    {
        List<CardData> choisies = new List<CardData>();
        foreach (DraftCardButton bouton in boutons)
        {
            if (bouton.Selected)
            {
                choisies.Add(bouton.Data);
            }
        }

        if (CardTooltip.Instance != null)
        {
            CardTooltip.Instance.Hide();
        }

        panelRoot.SetActive(false);
        onConfirm?.Invoke(choisies);
    }

    private int Quota(CardType type)
    {
        if (type == CardType.Bonne)
        {
            return quotaBonnes;
        }

        if (type == CardType.Mauvaise)
        {
            return quotaMauvaises;
        }

        return quotaSpeciales;
    }

    private int CompterChoisies(CardType type)
    {
        int total = 0;
        foreach (DraftCardButton bouton in boutons)
        {
            if (bouton.Selected && bouton.Data.type == type)
            {
                total++;
            }
        }
        return total;
    }

    private int CompterProposees(CardType type)
    {
        int total = 0;
        foreach (DraftCardButton bouton in boutons)
        {
            if (bouton.Data.type == type)
            {
                total++;
            }
        }
        return total;
    }
}
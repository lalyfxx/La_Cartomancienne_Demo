using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Données")]
    [Tooltip("Les nuits, dans l'ordre (Nuit1 à Nuit5).")]
    [SerializeField] private List<NightData> nights = new List<NightData>();
    [SerializeField] private CardView cardPrefab;

    [Header("Règles du deck (à tester)")]
    [Tooltip("Coché : les cartes non utilisées sont gardées pour la nuit suivante. Décoché : la main est vidée chaque matin.")]
    [SerializeField] private bool garderCartesNonUtilisees = true;

    [Tooltip("Nombre maximum de cartes en main. Au-delà, les cartes gardées les plus anciennes sont défaussées.")]
    [SerializeField] [Min(1)] private int plafondDeck = 8;

    [Header("Zones de jeu")]
    [SerializeField] private Transform handArea;
    [SerializeField] private CardSlot destinySlot;
    [SerializeField] private PalmReadingPanel palmPanel;
    [Tooltip("Le bouton « Examiner », caché s'il n'y a aucun pouvoir cette nuit.")]
    [SerializeField] private Button examineButton;
    [SerializeField] private DraftPanel draftPanel;

    [Header("Affichage du client")]
    [SerializeField] private Image clientPortrait;
    [SerializeField] private TMP_Text clientNameText;
    [SerializeField] private TMP_Text clientDialogueText;
    [SerializeField] private Button validateButton;

    [Header("Affichage de la nuit")]
    [Tooltip("Ex : « Nuit 2 »")]
    [SerializeField] private TMP_Text nightTitleText;

    [Header("Fin de nuit")]
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMP_Text endText;
    [Tooltip("Le bouton « Nuit suivante » de l'écran de fin.")]
    [SerializeField] private Button nextNightButton;

    private class EtatPersonnage
    {
        public int destin;
        public int maladie;
        public bool mort;
        public bool lettreEnvoyee;
        public readonly List<CardData> cartes = new List<CardData>();
    }

    private readonly Dictionary<PersonnageData, EtatPersonnage> etats = new Dictionary<PersonnageData, EtatPersonnage>();

    private readonly List<PersonnageData> ordreRencontre = new List<PersonnageData>();

    private readonly List<CardData> heldCards = new List<CardData>();

    private readonly List<ClientData> visitesDuSoir = new List<ClientData>();

    private readonly List<ClientData> resultClients = new List<ClientData>();
    private readonly List<CardData> resultCards = new List<CardData>();

    private int cartesJustes;
    private int cartesInjustes;
    private int cartesGenereuses;
    private int cartesSeveres;

    private int currentNightIndex;
    private int currentClientIndex;
    private int cartesDefaussees;

    private NightData CurrentNight => nights[currentNightIndex];
    private bool DerniereNuit => currentNightIndex >= nights.Count - 1;

    private void Start()
    {
        destinySlot.OnCardChanged += HandleCardChanged;
        validateButton.onClick.AddListener(ValidateClient);

        if (nextNightButton != null)
        {
            nextNightButton.onClick.AddListener(NextNight);
        }

        if (nights.Count == 0)
        {
            Debug.LogError("Aucune nuit dans le GameManager : glisse tes nuits dans la liste « Nights ».");
            return;
        }

        StartNight(0);
    }

    private void OnDestroy()
    {
        if (destinySlot != null)
        {
            destinySlot.OnCardChanged -= HandleCardChanged;
        }
    }

    private void StartNight(int index)
    {
        currentNightIndex = index;
        endScreen.SetActive(false);
        resultClients.Clear();
        resultCards.Clear();
        cartesDefaussees = 0;

        NightData night = CurrentNight;

        if (nightTitleText != null)
        {
            nightTitleText.text = "Nuit " + (index + 1);
        }

        palmPanel.SetPouvoirs(night);
        if (examineButton != null)
        {
            examineButton.gameObject.SetActive(palmPanel.Disponible);
        }

        if (!garderCartesNonUtilisees)
        {
            heldCards.Clear();
        }
        ClearHandView();

        string lettres = LettresDuMatin(night);
        List<CardData> offres = TirerOffres(night);

        string titre = "Matin de la nuit " + (index + 1) + " : choisis tes cartes";
        if (heldCards.Count > 0)
        {
            titre += " (tu en as déjà " + heldCards.Count + ")";
        }

        draftPanel.Open(titre, lettres, offres,
            night.bonnesAChoisir, night.mauvaisesAChoisir, night.specialesAChoisir,
            OnDraftConfirmed);
    }

    private string LettresDuMatin(NightData night)
    {
        StringBuilder sb = new StringBuilder();

        foreach (string lettre in night.lettresDuMatin)
        {
            if (!string.IsNullOrEmpty(lettre))
            {
                sb.AppendLine(lettre);
                sb.AppendLine();
            }
        }

        foreach (PersonnageData perso in ordreRencontre)
        {
            EtatPersonnage etat = etats[perso];
            if (etat.mort && !etat.lettreEnvoyee)
            {
                etat.lettreEnvoyee = true;
                string lettre = string.IsNullOrEmpty(perso.lettreDeces)
                    ? "Une lettre de la famille : " + perso.nom + " nous a quittés."
                    : perso.lettreDeces;
                sb.AppendLine(lettre);
                sb.AppendLine();
            }
        }

        return sb.ToString().Trim();
    }

    private List<CardData> TirerOffres(NightData night)
    {
        List<CardData> offres = new List<CardData>();
        AjouterOffres(offres, night, CardType.Bonne, night.bonnesAChoisir);
        AjouterOffres(offres, night, CardType.Mauvaise, night.mauvaisesAChoisir);
        AjouterOffres(offres, night, CardType.Speciale, night.specialesAChoisir);
        return offres;
    }

    private void AjouterOffres(List<CardData> offres, NightData night, CardType type, int aChoisir)
    {
        if (aChoisir <= 0)
        {
            return;
        }

        int nombre = Mathf.Max(night.proposeesParFamille, aChoisir);
        for (int i = 0; i < nombre; i++)
        {
            CardData carte = TirerCarte(type, night.cartesDisponibles);
            if (carte == null)
            {
                Debug.LogWarning("Nuit " + (currentNightIndex + 1) + " : aucune carte de type " + type + " dans les cartes disponibles.");
                return;
            }
            offres.Add(carte);
        }
    }

    private CardData TirerCarte(CardType type, List<CardData> pool)
    {
        int totalPoids = 0;
        foreach (CardData card in pool)
        {
            if (card != null && card.type == type)
            {
                totalPoids += card.poidsTirage;
            }
        }

        if (totalPoids <= 0)
        {
            return null;
        }

        int tirage = Random.Range(0, totalPoids);
        foreach (CardData card in pool)
        {
            if (card == null || card.type != type)
            {
                continue;
            }

            tirage -= card.poidsTirage;
            if (tirage < 0)
            {
                return card;
            }
        }

        return null;
    }

    private void OnDraftConfirmed(List<CardData> choisies)
    {
        heldCards.AddRange(choisies);

        while (heldCards.Count > plafondDeck)
        {
            heldCards.RemoveAt(0);
            cartesDefaussees++;
        }

        SpawnHand();

        visitesDuSoir.Clear();
        foreach (ClientData visite in CurrentNight.clients)
        {
            if (visite == null)
            {
                continue;
            }

            if (visite.personnage != null && etats.TryGetValue(visite.personnage, out EtatPersonnage etat) && etat.mort)
            {
                continue;
            }

            visitesDuSoir.Add(visite);
        }

        if (visitesDuSoir.Count == 0)
        {
            ShowEndScreen();
            return;
        }

        if (heldCards.Count < visitesDuSoir.Count)
        {
            Debug.LogWarning("Nuit " + (currentNightIndex + 1) + " : moins de cartes que de clients, le joueur va être bloqué !");
        }

        ShowClient(0);
    }

    private void ClearHandView()
    {
        for (int i = handArea.childCount - 1; i >= 0; i--)
        {
            Destroy(handArea.GetChild(i).gameObject);
        }
    }

    private void SpawnHand()
    {
        ClearHandView();

        foreach (CardData card in heldCards)
        {
            CardView view = Instantiate(cardPrefab, handArea);
            view.Setup(card, handArea);
        }
    }

    private void ShowClient(int index)
    {
        currentClientIndex = index;
        ClientData client = visitesDuSoir[index];

        clientNameText.text = client.NomAffiche;
        clientDialogueText.text = client.dialogue;

        Sprite portrait = client.PortraitAffiche;
        if (portrait != null)
        {
            clientPortrait.sprite = portrait;
        }

        palmPanel.SetClient(client);

        validateButton.interactable = false;
    }

    private void HandleCardChanged(CardView card)
    {
        validateButton.interactable = card != null;
    }

    private void ValidateClient()
    {
        CardView placedCard = destinySlot.CurrentCard;
        if (placedCard == null)
        {
            return;
        }

        ClientData client = visitesDuSoir[currentClientIndex];
        CardData carte = placedCard.Data;

        resultClients.Add(client);
        resultCards.Add(carte);

        AppliquerCarte(client.personnage, carte);

        heldCards.Remove(carte);
        destinySlot.Clear();
        Destroy(placedCard.gameObject);

        int nextIndex = currentClientIndex + 1;
        if (nextIndex < visitesDuSoir.Count)
        {
            ShowClient(nextIndex);
        }
        else
        {
            ShowEndScreen();
        }
    }

    private void AppliquerCarte(PersonnageData perso, CardData carte)
    {
        if (perso == null)
        {
            Debug.LogWarning("Une visite n'a pas de personnage : la carte ne s'accumule sur personne.");
            return;
        }

        if (!etats.TryGetValue(perso, out EtatPersonnage etat))
        {
            etat = new EtatPersonnage();
            etats.Add(perso, etat);
            ordreRencontre.Add(perso);
        }

        etat.destin += carte.Effet;
        etat.maladie += carte.maladie;
        etat.cartes.Add(carte);

        if (carte.Effet > 0)
        {
            cartesGenereuses++;
        }
        else if (carte.Effet < 0)
        {
            cartesSeveres++;
        }

        if (carte.Effet != 0)
        {
            bool meritee = (carte.Effet > 0) == perso.estBonnePersonne;
            if (meritee)
            {
                cartesJustes++;
            }
            else
            {
                cartesInjustes++;
            }
        }
    }

    private void ShowEndScreen()
    {
        palmPanel.Close();
        endScreen.SetActive(true);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>Fin de la nuit " + (currentNightIndex + 1) + "</b>");
        sb.AppendLine();

        for (int i = 0; i < resultClients.Count; i++)
        {
            ClientData client = resultClients[i];
            CardData card = resultCards[i];

            sb.AppendLine("<b>" + client.NomAffiche + "</b> a reçu " + card.cardName + ".");
            sb.AppendLine(Reaction(client, card));
            sb.AppendLine();
        }

        foreach (PersonnageData perso in ordreRencontre)
        {
            EtatPersonnage etat = etats[perso];
            if (!etat.mort && etat.maladie >= perso.seuilMort)
            {
                etat.mort = true;
            }
        }

        if (garderCartesNonUtilisees)
        {
            sb.AppendLine("Cartes gardées pour demain : " + heldCards.Count);
        }

        if (cartesDefaussees > 0)
        {
            sb.AppendLine("Ton paquet débordait : " + cartesDefaussees + " carte(s) défaussée(s) ce matin.");
        }

        if (DerniereNuit)
        {
            sb.AppendLine();
            sb.AppendLine(RecapDeFin());
        }

        if (nextNightButton != null)
        {
            nextNightButton.gameObject.SetActive(!DerniereNuit);
        }

        endText.text = sb.ToString();
    }

    private string Reaction(ClientData client, CardData card)
    {
        string texte;

        if (card.type == CardType.Bonne)
        {
            texte = client.resultatBonneCarte;
        }
        else if (card.type == CardType.Mauvaise)
        {
            texte = client.resultatMauvaiseCarte;
        }
        else
        {
            texte = string.IsNullOrEmpty(client.resultatSpeciale) ? card.description : client.resultatSpeciale;
        }

        return string.IsNullOrEmpty(texte) ? "(réaction à écrire)" : texte;
    }

    private string RecapDeFin()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<b>Ce qu'ils sont devenus</b>");

        foreach (PersonnageData perso in ordreRencontre)
        {
            EtatPersonnage etat = etats[perso];
            string fin;

            if (etat.mort)
            {
                fin = perso.finMort;
            }
            else if (etat.destin > 0)
            {
                fin = perso.finPositive;
            }
            else if (etat.destin < 0)
            {
                fin = perso.finNegative;
            }
            else
            {
                fin = perso.finNeutre;
            }

            if (string.IsNullOrEmpty(fin))
            {
                fin = "(fin à écrire)";
            }

            sb.AppendLine("<b>" + perso.nom + "</b> : " + fin);
        }

        sb.AppendLine();
        sb.AppendLine("<b>Ce qu'on dit de toi : " + Jugement() + "</b>");
        return sb.ToString();
    }

    private string Jugement()
    {
        bool juste = cartesJustes >= cartesInjustes;
        bool genereuse = cartesGenereuses >= cartesSeveres;

        if (juste && genereuse)
        {
            return "bienveillante";
        }

        if (juste)
        {
            return "dure mais juste";
        }

        if (genereuse)
        {
            return "naïve";
        }

        return "horrible et méchante";
    }

    public void NextNight()
    {
        if (DerniereNuit)
        {
            return;
        }

        StartNight(currentNightIndex + 1);
    }

    public void RestartNight()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
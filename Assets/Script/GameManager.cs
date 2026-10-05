using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Données")]
    [SerializeField] private NightData night;
    [SerializeField] private CardView cardPrefab;

    [Header("Zones de jeu")]
    [SerializeField] private Transform handArea;
    [SerializeField] private CardSlot destinySlot;
    [SerializeField] private PalmReadingPanel palmPanel;

    [Header("Affichage du client")]
    [SerializeField] private Image clientPortrait;
    [SerializeField] private TMP_Text clientNameText;
    [SerializeField] private TMP_Text clientDialogueText;
    [SerializeField] private Button validateButton;

    [Header("Ordre du Bureau")]
    [SerializeField] private TMP_Text orderText;

    [Header("Suspicion")]
    [SerializeField] private SuspicionGauge suspicion;
    [SerializeField] private int effetObeissance = -10;
    [SerializeField] private int effetDesobeissance = 25;

    [Header("Fin de nuit")]
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMP_Text endText;

    private int currentClientIndex;

    private readonly List<ClientData> resultClients = new List<ClientData>();
    private readonly List<CardData> resultCards = new List<CardData>();

    private readonly List<string> resultOrdres = new List<string>();

    private void Start()
    {
        endScreen.SetActive(false);

        destinySlot.OnCardChanged += HandleCardChanged;
        validateButton.onClick.AddListener(ValidateClient);

        if (night.deck.Count < night.clients.Count)
        {
            Debug.LogWarning("Il y a moins de cartes que de clients : le joueur va se retrouver bloqué !");
        }

        SpawnDeck();
        ShowClient(0);
    }

    private void OnDestroy()
    {
        if (destinySlot != null)
        {
            destinySlot.OnCardChanged -= HandleCardChanged;
        }
    }

    private void SpawnDeck()
    {
        foreach (CardData card in night.deck)
        {
            CardView view = Instantiate(cardPrefab, handArea);
            view.Setup(card, handArea);
        }
    }

    private void ShowClient(int index)
    {
        currentClientIndex = index;
        ClientData client = night.clients[index];

        clientNameText.text = client.clientName;
        clientDialogueText.text = client.dialogue;

        if (client.portrait != null)
        {
            clientPortrait.sprite = client.portrait;
        }

        palmPanel.SetClient(client);

        if (orderText != null)
        {
            orderText.text = client.aUnOrdreDuBureau
                ? "Ordre du Bureau : " + client.texteOrdre
                : "Aucun ordre du Bureau pour ce client.";
        }

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

        ClientData client = night.clients[currentClientIndex];

        resultClients.Add(client);
        resultCards.Add(placedCard.Data);

        string noteOrdre = "";
        if (client.aUnOrdreDuBureau)
        {
            bool aObei = placedCard.Data == client.carteOrdonnee;
            noteOrdre = aObei ? "obéi" : "désobéi";

            if (suspicion != null)
            {
                suspicion.Add(aObei ? effetObeissance : effetDesobeissance);
            }
        }
        resultOrdres.Add(noteOrdre);

        destinySlot.Clear();
        Destroy(placedCard.gameObject);

        if (suspicion != null && suspicion.IsFull)
        {
            ShowEndScreen();
            return;
        }

        int nextIndex = currentClientIndex + 1;
        if (nextIndex < night.clients.Count)
        {
            ShowClient(nextIndex);
        }
        else
        {
            ShowEndScreen();
        }
    }

    private void ShowEndScreen()
    {
        palmPanel.Close();
        endScreen.SetActive(true);

        int destinsJustes = 0;
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < resultClients.Count; i++)
        {
            ClientData client = resultClients[i];
            CardData card = resultCards[i];

            bool aRecuBonneCarte = card.type == CardType.Bonne;
            string consequence = aRecuBonneCarte ? client.resultatBonneCarte : client.resultatMauvaiseCarte;

            sb.AppendLine("<b>" + client.clientName + "</b> a reçu " + card.cardName + ".");
            if (resultOrdres[i] != "")
            {
                sb.AppendLine("Tu as " + resultOrdres[i] + " au Bureau.");
            }
            sb.AppendLine(consequence);
            sb.AppendLine();

            if (aRecuBonneCarte == client.estBonnePersonne)
            {
                destinsJustes++;
            }
        }

        sb.AppendLine("Destins justes : " + destinsJustes + " / " + resultClients.Count);

        if (suspicion != null)
        {
            sb.AppendLine("Suspicion de l'État : " + suspicion.Value);

            if (suspicion.IsFull)
            {
                sb.AppendLine();
                sb.AppendLine("<b>On frappe à la porte. Le Bureau est venu t'arrêter.</b>");
            }
        }

        endText.text = sb.ToString();
    }

    public void RestartNight()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
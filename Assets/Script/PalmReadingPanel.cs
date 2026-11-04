using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PalmReadingPanel : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image palmImage;
    [SerializeField] private TMP_Text clueText;

    [Header("Boutons des pouvoirs (cachés si le pouvoir n'est pas débloqué)")]
    [Tooltip("Un objet qui regroupe les 3 boutons de lignes (ou laisse vide).")]
    [SerializeField] private GameObject groupeMain;
    [SerializeField] private GameObject boutonCafe;
    [SerializeField] private GameObject boutonPendule;
    [SerializeField] private GameObject boutonAura;

    private ClientData currentClient;

    private bool mainOk = true;
    private bool cafeOk;
    private bool penduleOk;
    private bool auraOk;

    private bool penduleUtilise;

    public bool Disponible => mainOk || cafeOk || penduleOk || auraOk;

    public void SetPouvoirs(NightData night)
    {
        mainOk = night.main;
        cafeOk = night.cafe;
        penduleOk = night.pendule;
        auraOk = night.aura;
        penduleUtilise = false;

        Montrer(groupeMain, mainOk);
        Montrer(boutonCafe, cafeOk);
        Montrer(boutonPendule, penduleOk);
        Montrer(boutonAura, auraOk);

        if (palmImage != null)
        {
            palmImage.gameObject.SetActive(mainOk);
        }
    }

    private void Montrer(GameObject obj, bool visible)
    {
        if (obj != null)
        {
            obj.SetActive(visible);
        }
    }

    public void SetClient(ClientData client)
    {
        currentClient = client;

        if (palmImage != null && client.palmImage != null)
        {
            palmImage.sprite = client.palmImage;
        }

        clueText.text = "Choisis un pouvoir pour examiner ce client.";
        Close();
    }

    public void Open()
    {
        if (!Disponible)
        {
            return;
        }

        panelRoot.SetActive(true);
    }

    public void Close()
    {
        panelRoot.SetActive(false);
    }

    public void ReadLigneDeVie()
    {
        if (mainOk && currentClient != null)
        {
            ShowClue("Ligne de vie", currentClient.ligneDeVie);
        }
    }

    public void ReadLigneDeCoeur()
    {
        if (mainOk && currentClient != null)
        {
            ShowClue("Ligne de cœur", currentClient.ligneDeCoeur);
        }
    }

    public void ReadLigneDeTete()
    {
        if (mainOk && currentClient != null)
        {
            ShowClue("Ligne de tête", currentClient.ligneDeTete);
        }
    }

    public void ReadMarcDeCafe()
    {
        if (cafeOk && currentClient != null)
        {
            ShowClue("Marc de café", currentClient.marcDeCafe);
        }
    }

    public void UsePendule()
    {
        if (!penduleOk || currentClient == null)
        {
            return;
        }

        if (penduleUtilise)
        {
            ShowClue("Pendule", "Le pendule ne répond qu'une fois par nuit.");
            return;
        }

        if (string.IsNullOrEmpty(currentClient.questionPendule))
        {
            ShowClue("Pendule", "Le pendule reste immobile : il n'y a rien à lui demander pour ce client.");
            return;
        }

        penduleUtilise = true;
        string reponse = currentClient.reponsePendule ? "Oui." : "Non.";
        ShowClue("Pendule", currentClient.questionPendule + "\n" + reponse);
    }

    public void ReadAura()
    {
        if (auraOk && currentClient != null)
        {
            ShowClue("Aura", currentClient.aura);
        }
    }

    private void ShowClue(string title, string clue)
    {
        if (string.IsNullOrEmpty(clue))
        {
            clue = "Rien de particulier.";
        }

        clueText.text = "<b>" + title + "</b>\n" + clue;
    }
}
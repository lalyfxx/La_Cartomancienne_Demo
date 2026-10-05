using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PalmReadingPanel : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image palmImage;
    [SerializeField] private TMP_Text clueText;

    private ClientData currentClient;

    public void SetClient(ClientData client)
    {
        currentClient = client;

        if (palmImage != null && client.palmImage != null)
        {
            palmImage.sprite = client.palmImage;
        }

        clueText.text = "Clique sur une ligne de la main pour la lire.";
        Close();
    }


    public void Open()
    {
        panelRoot.SetActive(true);
    }

    public void Close()
    {
        panelRoot.SetActive(false);
    }

    public void ReadLigneDeVie()
    {
        ShowClue("Ligne de vie", currentClient.ligneDeVie);
    }

    public void ReadLigneDeCoeur()
    {
        ShowClue("Ligne de cœur", currentClient.ligneDeCoeur);
    }

    public void ReadLigneDeTete()
    {
        ShowClue("Ligne de tête", currentClient.ligneDeTete);
    }

    private void ShowClue(string title, string clue)
    {
        if (currentClient == null)
        {
            return;
        }

        clueText.text = "<b>" + title + "</b>\n" + clue;
    }
}
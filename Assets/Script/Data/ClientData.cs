using UnityEngine;

[CreateAssetMenu(fileName = "NouveauClient", menuName = "Cartomancienne/Client")]
public class ClientData : ScriptableObject
{
    [Header("Personnage")]
    [Tooltip("Le personnage qui vient. Ses cartes s'accumulent d'une visite à l'autre.")]
    public PersonnageData personnage;

    [Header("Ancien nom et portrait (utilisés seulement si aucun personnage n'est mis)")]
    public string clientName;

    public Sprite portrait;

    [Header("Ce qu'il raconte cette nuit")]
    [TextArea(3, 6)]
    public string dialogue;

    [Header("Pouvoir : lecture des lignes de la main")]
    public Sprite palmImage;

    [TextArea(2, 4)]
    public string ligneDeVie;

    [TextArea(2, 4)]
    public string ligneDeCoeur;

    [TextArea(2, 4)]
    public string ligneDeTete;

    [Header("Pouvoir : marc de café")]
    [TextArea(2, 4)]
    public string marcDeCafe;

    [Header("Pouvoir : pendule (une seule question par nuit)")]
    public string questionPendule;

    public bool reponsePendule;

    [Header("Pouvoir : aura photographiée")]
    [TextArea(2, 4)]
    public string aura;

    [Header("Réaction affichée en fin de nuit")]
    [TextArea(2, 4)]
    public string resultatBonneCarte;

    [TextArea(2, 4)]
    public string resultatMauvaiseCarte;

    [TextArea(2, 4)]
    public string resultatSpeciale;

    public string NomAffiche
    {
        get { return personnage != null ? personnage.nom : clientName; }
    }

    public Sprite PortraitAffiche
    {
        get { return personnage != null && personnage.portrait != null ? personnage.portrait : portrait; }
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "NouveauClient", menuName = "Cartomancienne/Client")]
public class ClientData : ScriptableObject
{
    [Header("Identité")]
    public string clientName;

    public Sprite portrait;

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

    [Header("Ordre du Bureau (laisser décoché si aucun ordre)")]
    public bool aUnOrdreDuBureau;

    public CardData carteOrdonnee;

    [TextArea(2, 3)]
    public string texteOrdre;

    [Header("Vérité cachée (le joueur ne la voit pas)")]
    public bool estBonnePersonne;

    [Header("Conséquences affichées en fin de nuit")]
    [TextArea(2, 4)]
    public string resultatBonneCarte;

    [TextArea(2, 4)]
    public string resultatMauvaiseCarte;
}
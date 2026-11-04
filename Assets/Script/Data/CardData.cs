using UnityEngine;

public enum CardType
{
    Bonne,
    Mauvaise,
    Speciale
}

[CreateAssetMenu(fileName = "NouvelleCarte", menuName = "Cartomancienne/Carte")]
public class CardData : ScriptableObject
{
    [Header("Infos de la carte")]
    public string cardName;

    [TextArea(2, 4)]
    public string description;

    public Sprite illustration;

    [Header("Famille")]
    public CardType type;

    [Header("Effet sur le destin (s'accumule sur le personnage)")]
    [Tooltip("Ce que la carte ajoute au destin du personnage. Laisse 0 pour la valeur par défaut : +1 pour une bonne carte, -1 pour une mauvaise, 0 pour une spéciale.")]
    public int effetDestin = 0;

    [Tooltip("Maladie ajoutée au personnage (ex : La Mort = 1). Quand elle atteint son seuil, il meurt.")]
    [Min(0)]
    public int maladie = 0;

    [Header("Tirage du matin")]
    [Tooltip("Chance d'être proposée le matin. Commune = 10, rare = 3, très rare = 1.")]
    [Min(0)]
    public int poidsTirage = 10;

    public int Effet
    {
        get
        {
            if (effetDestin != 0)
            {
                return effetDestin;
            }

            if (type == CardType.Bonne)
            {
                return 1;
            }

            if (type == CardType.Mauvaise)
            {
                return -1;
            }

            return 0;
        }
    }
}
using UnityEngine;

public enum CardType
{
    Bonne,
    Mauvaise
}

[CreateAssetMenu(fileName = "NouvelleCarte", menuName = "Cartomancienne/Carte")]
public class CardData : ScriptableObject
{
    [Header("Infos de la carte")]
    public string cardName;

    [TextArea(2, 4)]
    public string description;

    public Sprite illustration;

    [Header("Effet sur le destin")]
    public CardType type;
}
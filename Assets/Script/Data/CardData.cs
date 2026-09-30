using UnityEngine;

[CreateAssetMenu(fileName = "NouvelleCarte", menuName = "Cartomancienne/Carte")]
public class CardData : ScriptableObject
{
    public string cardName;

    [TextArea(2, 4)]
    public string description; 

    public Sprite artwork; 

    [Range(-3, 3)]
    public int value;
}

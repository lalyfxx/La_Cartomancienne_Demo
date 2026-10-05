using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NouvelleNuit", menuName = "Cartomancienne/Nuit")]
public class NightData : ScriptableObject
{
    [Tooltip("Les cartes reçues au début de la nuit. Il faut au moins autant de cartes que de clients.")]
    public List<CardData> deck = new List<CardData>();

    [Tooltip("Les clients, dans l'ordre où ils passent.")]
    public List<ClientData> clients = new List<ClientData>();
}
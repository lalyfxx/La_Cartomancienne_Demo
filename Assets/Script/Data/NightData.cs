using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NouvelleNuit", menuName = "Cartomancienne/Nuit")]
public class NightData : ScriptableObject
{
    [Header("Matin : le choix des cartes")]
    [Tooltip("Les cartes qui peuvent être proposées ce matin, tirées au hasard selon leur poids de tirage.")]
    public List<CardData> cartesDisponibles = new List<CardData>();

    [Tooltip("Combien de cartes de chaque famille sont proposées au joueur.")]
    [Min(1)] public int proposeesParFamille = 3;

    [Min(0)] public int bonnesAChoisir = 2;
    [Min(0)] public int mauvaisesAChoisir = 2;
    [Min(0)] public int specialesAChoisir = 1;

    [Tooltip("Lettres ou événements écrits à l'avance, lus ce matin (ex : la lettre annonçant la mort de la veuve).")]
    [TextArea(2, 5)]
    public List<string> lettresDuMatin = new List<string>();

    [Header("Pouvoirs disponibles cette nuit")]
    public bool main = true;
    public bool cafe = true;
    public bool pendule = false;
    public bool aura = false;

    [Header("Clients")]
    [Tooltip("Les visites de la nuit, dans l'ordre. Un personnage mort est sauté automatiquement.")]
    public List<ClientData> clients = new List<ClientData>();
}
using UnityEngine;

[CreateAssetMenu(fileName = "NouveauPersonnage", menuName = "Cartomancienne/Personnage")]
public class PersonnageData : ScriptableObject
{
    [Header("Identité")]
    public string nom;

    public Sprite portrait;

    [Header("Vérité cachée (le joueur ne la voit pas)")]
    [Tooltip("Sert au jugement de fin de partie : est-ce que ce personnage méritait d'être aidé ?")]
    public bool estBonnePersonne = true;

    [Header("Maladie")]
    [Tooltip("Quand la maladie accumulée atteint ce nombre, le personnage meurt.")]
    [Min(1)]
    public int seuilMort = 2;

    [Tooltip("La lettre de la famille, reçue le matin qui suit sa mort.")]
    [TextArea(3, 6)]
    public string lettreDeces;

    [Header("Récap de fin, selon son destin accumulé")]
    [TextArea(2, 4)]
    public string finPositive;

    [TextArea(2, 4)]
    public string finNeutre;

    [TextArea(2, 4)]
    public string finNegative;

    [TextArea(2, 4)]
    public string finMort;
}
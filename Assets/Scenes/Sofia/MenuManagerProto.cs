using UnityEngine;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void NouvellePartie()
    {
        PlayerPrefs.DeleteKey("sauvegarde"); // à adapter à ton système de save
        SceneManager.LoadScene("Jeu");       // nom exact de ta scène
    }

    public void Continuer()
    {
        if (PlayerPrefs.HasKey("sauvegarde"))
            SceneManager.LoadScene("Jeu");
    }

    public void OuvrirOptions()
    {
        // ex : activer un panneau Settings
    }

    public void Quitter()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerProto : MonoBehaviour
{
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject cartesMenu;

    public void NewPart()
    {
        SceneManager.LoadScene("DEMO");
    }

    public void Continue()
    {
        SceneManager.LoadScene("DEMO"); 
    }

    public void OpenSettings()
    {
        panelSettings.SetActive(true);
        cartesMenu.SetActive(false);
    }

    public void ShutSettings()
    {
        panelSettings.SetActive(false);
        cartesMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
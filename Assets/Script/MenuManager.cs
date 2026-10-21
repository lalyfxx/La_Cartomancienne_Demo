using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MenuManager : MonoBehaviour
{
    [Header("Vidéo d'intro")]
    [Tooltip("L'objet VideoPlayer de la scène")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("UI")]
    [Tooltip("L'objet --Menu-- (tout ce qui doit être caché pendant la vidéo)")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Scène du jeu")]
    [Tooltip("Nom exact de la scène à charger quand on clique sur Play (elle doit être dans les Build Settings)")]
    [SerializeField] private string gameSceneName = "Game";

    private bool introFinished = false;

    private void Awake()
    {
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void Start()
    {
        if (videoPlayer == null)
        {

            ShowMenu();
            return;
        }


        menuRoot.SetActive(false);


        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }

    private void Update()
    {
        if (introFinished) return;

        if (SkipPressed())
        {
            SkipVideo();
        }
    }

    private bool SkipPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        ShowMenu();
    }

    private void SkipVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
        ShowMenu();
    }

    private void ShowMenu()
    {
        if (introFinished) return;
        introFinished = true;

        menuRoot.SetActive(true);
    }

    private void OnPlayClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
       
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}


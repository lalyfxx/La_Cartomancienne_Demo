using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CarteMenu : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Survol")]
    [SerializeField] private float echelleSurvol = 1.15f;
    [SerializeField] private float vitesse = 12f;

    [Header("Transition on clic")]
    [SerializeField] private bool zoomAuClic = false;
    [SerializeField] private float echelleZoom = 6f;
    [SerializeField] private float dureeZoom = 0.6f;
    [SerializeField] private CanvasGroup fondNoir;

    [Header("Action on clic")]
    public UnityEvent onClick;

    private Vector3 echelleBase;
    private Vector3 echelleCible;
    private bool enTransition;

    void Start()
    {
        echelleBase = transform.localScale;
        echelleCible = echelleBase;
    }

    void Update()
    {
        if (enTransition) return;
        float t = 1f - Mathf.Exp(-vitesse * Time.unscaledDeltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, echelleCible, t);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (enTransition) return;
        echelleCible = echelleBase * echelleSurvol;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (enTransition) return;
        echelleCible = echelleBase;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (enTransition) return;

        if (zoomAuClic)
            StartCoroutine(ZoomPuisAction());
        else
            onClick?.Invoke();
    }

    private IEnumerator ZoomPuisAction()
    {
        enTransition = true;

   
        LayoutGroup layout = GetComponentInParent<LayoutGroup>();
        if (layout != null) layout.enabled = false;

        transform.SetAsLastSibling(); 

        Vector3 posDepart = transform.position;
        Vector3 centre = GetComponentInParent<Canvas>().rootCanvas.transform.position;
        Vector3 echelleDepart = transform.localScale;
        Vector3 echelleFin = echelleBase * echelleZoom;

        if (fondNoir != null)
        {
            fondNoir.alpha = 0f;
            fondNoir.blocksRaycasts = true; 
        }

        float temps = 0f;
        while (temps < dureeZoom)
        {
            temps += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(temps / dureeZoom);
            float e = t * t * (3f - 2f * t); 

            transform.localScale = Vector3.Lerp(echelleDepart, echelleFin, e);
            transform.position = Vector3.Lerp(posDepart, centre, e);

            if (fondNoir != null)
                fondNoir.alpha = Mathf.InverseLerp(0.5f, 1f, t);

            yield return null;
        }

        onClick?.Invoke(); 
    }
}
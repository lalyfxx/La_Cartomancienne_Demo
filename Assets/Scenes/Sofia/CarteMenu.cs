using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CarteMenu : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Effet de survol")]
    [SerializeField] private float echelleSurvol = 1.15f;
    [SerializeField] private float vitesse = 12f;

    [Header("Action au clic")]
    public UnityEvent onClick;

    private Vector3 echelleBase;
    private Vector3 echelleCible;

    void Start()
    {
        echelleBase = transform.localScale;
        echelleCible = echelleBase;
    }

    void Update()
    {
        float t = 1f - Mathf.Exp(-vitesse * Time.unscaledDeltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, echelleCible, t);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        echelleCible = echelleBase * echelleSurvol;
    }

    public void OnPointerExit(PointerEventData e)
    {
        echelleCible = echelleBase;
    }

    public void OnPointerClick(PointerEventData e)
    {
        onClick?.Invoke();
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SuspicionGauge : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text valueText;

    [Header("Réglages")]
    [SerializeField] private int maxSuspicion = 100;
    [SerializeField] private int startSuspicion = 20;

    public int Value { get; private set; }

    public bool IsFull => Value >= maxSuspicion;

    private void Awake()
    {
        Value = startSuspicion;

        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = maxSuspicion;
            slider.interactable = false;
        }

        Refresh();
    }

    public void Add(int amount)
    {
        Value = Mathf.Clamp(Value + amount, 0, maxSuspicion);
        Refresh();
    }

    private void Refresh()
    {
        if (slider != null)
        {
            slider.value = Value;
        }

        if (valueText != null)
        {
            valueText.text = "Suspicion : " + Value + " / " + maxSuspicion;
        }
    }
}
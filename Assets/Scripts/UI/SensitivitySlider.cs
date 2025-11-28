using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SensitivitySlider : MonoBehaviour
{
    [SerializeField] private QuakeMovement targetMovement;
    [SerializeField] private float sliderMin = 0.1f;
    [SerializeField] private float sliderMax = 10f;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = sliderMin;
        slider.maxValue = sliderMax;

        if (targetMovement == null)
            targetMovement = FindObjectOfType<QuakeMovement>();

        if (targetMovement != null)
            slider.value = targetMovement.GetMouseSensitivity();

        slider.onValueChanged.AddListener(HandleValueChanged);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(HandleValueChanged);
    }

    private void HandleValueChanged(float value)
    {
        if (targetMovement != null)
            targetMovement.SetMouseSensitivity(value);
    }

    public void SetTarget(QuakeMovement movement)
    {
        targetMovement = movement;
        if (movement != null)
            slider.value = movement.GetMouseSensitivity();
    }
}

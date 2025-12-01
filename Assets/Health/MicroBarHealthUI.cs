using UnityEngine;
using Microlight.MicroBar;

public class MicroBarHealthUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private MicroBar bar;

    private void Start()
    {
        bar.Initialize(health.GetMaxHealth());
        bar.UpdateBar(health.CurrentHealth);

        health.onHealthChanged.AddListener(UpdateBar);
    }

    private void UpdateBar(float newValue)
    {
        bar.UpdateBar(newValue);
    }
}

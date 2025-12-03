using UnityEngine;
using UnityEngine.UI;

namespace Microlight.MicroBar
{
    // Simple health bar component used by MicroBarHealthUI.
    // Assign a UI Image (set to Filled type) to fillImage in the inspector.
    public class MicroBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage; // Expected to have Image.type = Filled
        [SerializeField] private bool invertFill; // Optional invert

        private float maxValue;

        /// <summary>
        /// Initialize the bar with a maximum value.
        /// </summary>
        public void Initialize(float max)
        {
            maxValue = Mathf.Max(1f, max); // prevent divide by zero
            UpdateBar(maxValue);
        }

        /// <summary>
        /// Update the bar's displayed value.
        /// </summary>
        public void UpdateBar(float current)
        {
            if (fillImage == null)
            {
                // Gracefully handle missing reference
                Debug.LogWarning("MicroBar: fillImage not set.");
                return;
            }

            float pct = Mathf.Clamp01(current / maxValue);
            if (invertFill) pct = 1f - pct;
            fillImage.fillAmount = pct;
        }
    }
}

// Assets/Scripts/UI/HUD/HealthBarUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GothicShooter.Health;

namespace GothicShooter.UI
{
    /// <summary>
    /// HUD component that displays player health and shield bars.
    /// Subscribes to PlayerHealth events - contains zero game logic.
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] private Image healthFillImage;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Color healthColor = new Color(0.8f, 0.2f, 0.2f);
        [SerializeField] private Color lowHealthColor = new Color(1f, 0f, 0f);
        [SerializeField] private float lowHealthThreshold = 0.25f;

        [Header("Shield Bar (Optional)")]
        [SerializeField] private Image shieldFillImage;
        [SerializeField] private GameObject shieldBarParent;
        [SerializeField] private Color shieldColor = new Color(0.2f, 0.6f, 1f);

        [Header("Animation")]
        [SerializeField] private bool animateHealthChange = true;
        [SerializeField] private float animationSpeed = 5f;

        private PlayerHealth playerHealth;
        private HealthComponent playerHealthComponent;
        private float targetHealthFill = 1f;
        private float currentHealthFill = 1f;

        private void Start()
        {
            FindPlayer();
            InitializeUI();
            UpdateHealthBar();
        }

        private void Update()
        {
            if (playerHealth == null) return;

            // Smooth animation
            if (animateHealthChange && Mathf.Abs(currentHealthFill - targetHealthFill) > 0.01f)
            {
                currentHealthFill = Mathf.Lerp(currentHealthFill, targetHealthFill, Time.deltaTime * animationSpeed);
                ApplyHealthFill(currentHealthFill);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void FindPlayer()
        {
            // Find player in scene
            playerHealth = FindObjectOfType<PlayerHealth>();
            
            if (playerHealth != null)
            {
                playerHealthComponent = playerHealth.GetComponent<HealthComponent>();
                SubscribeToEvents();
            }
            else
            {
                Debug.LogWarning("[HealthBarUI] PlayerHealth not found in scene!");
            }
        }

        private void SubscribeToEvents()
        {
            if (playerHealthComponent != null)
            {
                playerHealthComponent.OnDamageTaken += OnPlayerDamageTaken;
                playerHealthComponent.OnHealed += OnPlayerHealed;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (playerHealthComponent != null)
            {
                playerHealthComponent.OnDamageTaken -= OnPlayerDamageTaken;
                playerHealthComponent.OnHealed -= OnPlayerHealed;
            }
        }

        private void InitializeUI()
        {
            // Set initial colors
            if (healthFillImage != null)
            {
                healthFillImage.color = healthColor;
            }

            if (shieldFillImage != null)
            {
                shieldFillImage.color = shieldColor;
            }

            // Hide shield bar if max shield is 0
            if (shieldBarParent != null && playerHealth != null)
            {
                shieldBarParent.SetActive(playerHealth.MaxShield > 0);
            }
        }

        private void OnPlayerDamageTaken(float damageAmount, float newHealth, GameObject source)
        {
            UpdateHealthBar();
        }

        private void OnPlayerHealed(float healAmount)
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (playerHealth == null) return;

            // Update target fill amount
            targetHealthFill = playerHealth.GetHealthPercentage();

            if (!animateHealthChange)
            {
                currentHealthFill = targetHealthFill;
                ApplyHealthFill(currentHealthFill);
            }

            // Update health text
            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(playerHealth.CurrentHealth)}/{Mathf.CeilToInt(playerHealth.MaxHealth)}";
            }

            // Update shield bar
            if (shieldFillImage != null && playerHealth.MaxShield > 0)
            {
                shieldFillImage.fillAmount = playerHealth.GetShieldPercentage();
            }
        }

        private void ApplyHealthFill(float fillAmount)
        {
            if (healthFillImage == null) return;

            healthFillImage.fillAmount = fillAmount;

            // Change color if health is low
            if (fillAmount <= lowHealthThreshold)
            {
                healthFillImage.color = Color.Lerp(lowHealthColor, healthColor, fillAmount / lowHealthThreshold);
            }
            else
            {
                healthFillImage.color = healthColor;
            }
        }

        // Public method to manually refresh (useful after scene transitions)
        public void RefreshHealthBar()
        {
            FindPlayer();
            UpdateHealthBar();
        }

        // Editor utility
        [ContextMenu("Find Player")]
        private void EditorFindPlayer()
        {
            FindPlayer();
            Debug.Log(playerHealth != null ? "Player found!" : "Player not found!");
        }
    }
}

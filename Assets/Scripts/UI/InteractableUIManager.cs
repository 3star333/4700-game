using UnityEngine;
using System.Collections;

/// <summary>
/// Global UI Manager for all interactable panels.
/// Manages showing/hiding panels with fade effects and handles ESC key to close.
/// Singleton pattern for easy access from any interactable.
/// </summary>
public class InteractableUIManager : MonoBehaviour
{
    private static InteractableUIManager instance;
    public static InteractableUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InteractableUIManager>();
                if (instance == null)
                {
                    Debug.LogWarning("InteractableUIManager not found in scene. Please add it to your UI Canvas.");
                }
            }
            return instance;
        }
    }

    [Header("UI Panels")]
    [SerializeField] private GameObject perkStationPanel;
    [SerializeField] private GameObject packAPunchPanel;
    [SerializeField] private GameObject weaponBuyPanel;
    [SerializeField] private GameObject mysteryChestPanel;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private bool enableESCToClose = true;

    private GameObject currentActivePanel;
    private CanvasGroup currentCanvasGroup;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Ensure all panels are hidden at start
        HideAllPanels();
    }

    private void Update()
    {
        // ESC key to close any active panel
        if (enableESCToClose && Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentActivePanel != null)
            {
                HideCurrentPanel();
            }
        }
    }

    public void ShowPerkStationPanel()
    {
        ShowPanel(perkStationPanel);
    }

    public void ShowPackAPunchPanel()
    {
        ShowPanel(packAPunchPanel);
    }

    public void ShowWeaponBuyPanel()
    {
        ShowPanel(weaponBuyPanel);
    }

    public void ShowMysteryChestPanel()
    {
        ShowPanel(mysteryChestPanel);
    }

    public void HideCurrentPanel()
    {
        if (currentActivePanel != null)
        {
            StartCoroutine(FadeOut(currentActivePanel));
            currentActivePanel = null;
            currentCanvasGroup = null;
        }
    }

    public void HideAllPanels()
    {
        if (perkStationPanel != null) perkStationPanel.SetActive(false);
        if (packAPunchPanel != null) packAPunchPanel.SetActive(false);
        if (weaponBuyPanel != null) weaponBuyPanel.SetActive(false);
        if (mysteryChestPanel != null) mysteryChestPanel.SetActive(false);
        currentActivePanel = null;
        currentCanvasGroup = null;
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogWarning("Attempted to show null panel in InteractableUIManager");
            return;
        }

        // Hide current panel first
        if (currentActivePanel != null && currentActivePanel != panel)
        {
            currentActivePanel.SetActive(false);
        }

        currentActivePanel = panel;
        currentCanvasGroup = panel.GetComponent<CanvasGroup>();

        // If no CanvasGroup, add one for fade effects
        if (currentCanvasGroup == null)
        {
            currentCanvasGroup = panel.AddComponent<CanvasGroup>();
        }

        panel.SetActive(true);
        StartCoroutine(FadeIn(panel));
    }

    private IEnumerator FadeIn(GameObject panel)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float elapsed = 0f;
        cg.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    private IEnumerator FadeOut(GameObject panel)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            panel.SetActive(false);
            yield break;
        }

        float elapsed = 0f;
        cg.alpha = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = 0f;
        panel.SetActive(false);
    }

    public bool IsAnyPanelActive()
    {
        return currentActivePanel != null && currentActivePanel.activeSelf;
    }

    public GameObject GetActivePanel()
    {
        return currentActivePanel;
    }
}

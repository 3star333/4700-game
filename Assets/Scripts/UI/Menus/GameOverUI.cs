// Assets/Scripts/UI/Menus/GameOverUI.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GothicShooter.Core;

namespace GothicShooter.UI
{
    /// <summary>
    /// Game Over screen UI controller.
    /// Listens to GameStateManager.OnGameOver event and displays game over panel.
    /// Provides restart and main menu functionality.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI roundSurvivedText;
        [SerializeField] private TextMeshProUGUI totalKillsText;
        [SerializeField] private TextMeshProUGUI finalScoreText;

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        [Header("Settings")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private bool showCursorOnGameOver = true;

        private void Start()
        {
            // Hide panel initially
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            // Subscribe to game state events
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnGameOver += ShowGameOver;
            }

            // Setup button listeners
            SetupButtons();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnGameOver -= ShowGameOver;
            }
        }

        private void SetupButtons()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
        }

        private void ShowGameOver()
        {
            if (gameOverPanel == null) return;

            // Show panel
            gameOverPanel.SetActive(true);

            // Update stats
            UpdateGameOverStats();

            // Show cursor
            if (showCursorOnGameOver)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            Debug.Log("[GameOverUI] Game Over screen displayed");
        }

        private void UpdateGameOverStats()
        {
            // Display round survived
            if (roundSurvivedText != null)
            {
                // TODO: Get actual round from RoundManager when integrated
                // int currentRound = RoundManager.Instance?.GetCurrentRound() ?? 0;
                int currentRound = 0; // Placeholder
                roundSurvivedText.text = $"Rounds Survived: {currentRound}";
            }

            // Display total kills
            if (totalKillsText != null)
            {
                // TODO: Get actual kills from KillTracker when implemented
                // int totalKills = KillTracker.TotalKills;
                int totalKills = 0; // Placeholder
                totalKillsText.text = $"Total Kills: {totalKills}";
            }

            // Display final score
            if (finalScoreText != null)
            {
                int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.Points : 0;
                finalScoreText.text = $"Final Score: {finalScore}";
            }
        }

        public void OnRestartButtonClicked()
        {
            Debug.Log("[GameOverUI] Restarting game...");

            // Reset time scale
            Time.timeScale = 1f;

            // Reload current scene
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }

        public void OnMainMenuButtonClicked()
        {
            Debug.Log("[GameOverUI] Returning to main menu...");

            // Reset time scale
            Time.timeScale = 1f;

            // Transition to main menu state
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.TransitionToMainMenu();
            }

            // Load main menu scene
            if (!string.IsNullOrEmpty(mainMenuSceneName))
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
            else
            {
                Debug.LogWarning("[GameOverUI] Main menu scene name not set!");
            }
        }

        public void OnQuitButtonClicked()
        {
            Debug.Log("[GameOverUI] Quitting game...");

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        // Public method to manually show game over (for testing)
        [ContextMenu("Test Game Over")]
        public void TestGameOver()
        {
            ShowGameOver();
        }
    }
}

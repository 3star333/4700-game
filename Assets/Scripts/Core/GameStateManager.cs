// Assets/Scripts/Core/GameStateManager.cs
using UnityEngine;
using System;

namespace GothicShooter.Core
{
    /// <summary>
    /// Central game state manager.
    /// Manages game flow and broadcasts state changes via events.
    /// Non-monolithic: other systems subscribe to events, not direct calls.
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public enum GameState
        {
            MainMenu,
            Loading,
            Playing,
            Paused,
            BetweenWaves,
            WaveStarting,
            GameOver,
            Victory
        }

        [Header("Initial Settings")]
        [SerializeField] private GameState initialState = GameState.Playing;

        private GameState currentState;
        public GameState CurrentState => currentState;

        // State Change Events
        public event Action<GameState, GameState> OnGameStateChanged; // (previousState, newState)
        
        // Specific State Events
        public event Action OnWaveStarted;
        public event Action OnWaveEnded;
        public event Action OnGameOver;
        public event Action OnVictory;
        public event Action OnPaused;
        public event Action OnResumed;

        private void Awake()
        {
            // Singleton pattern with DontDestroyOnLoad
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentState = initialState;
        }

        private void Update()
        {
            HandlePauseInput();
        }

        private void HandlePauseInput()
        {
            // ESC key toggles pause (only when playing)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentState == GameState.Playing)
                {
                    TransitionToPaused();
                }
                else if (currentState == GameState.Paused)
                {
                    TransitionToPlaying();
                }
            }
        }

        #region State Transitions

        public void TransitionToMainMenu()
        {
            ChangeState(GameState.MainMenu);
            Time.timeScale = 1f;
        }

        public void TransitionToLoading()
        {
            ChangeState(GameState.Loading);
        }

        public void TransitionToPlaying()
        {
            ChangeState(GameState.Playing);
            Time.timeScale = 1f;
            OnResumed?.Invoke();
        }

        public void TransitionToPaused()
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            OnPaused?.Invoke();
        }

        public void TransitionToBetweenWaves()
        {
            ChangeState(GameState.BetweenWaves);
            OnWaveEnded?.Invoke();
        }

        public void TransitionToWaveStarting()
        {
            ChangeState(GameState.WaveStarting);
            OnWaveStarted?.Invoke();
        }

        public void TransitionToGameOver()
        {
            if (currentState == GameState.GameOver) return; // Prevent double trigger

            ChangeState(GameState.GameOver);
            Time.timeScale = 0f;
            OnGameOver?.Invoke();
        }

        public void TransitionToVictory()
        {
            ChangeState(GameState.Victory);
            Time.timeScale = 0f;
            OnVictory?.Invoke();
        }

        #endregion

        private void ChangeState(GameState newState)
        {
            if (currentState == newState) return;

            GameState previousState = currentState;
            currentState = newState;

            OnGameStateChanged?.Invoke(previousState, newState);

            Debug.Log($"[GameStateManager] State Changed: {previousState} → {newState}");
        }

        #region Public Queries

        public bool IsPlaying() => currentState == GameState.Playing;
        public bool IsPaused() => currentState == GameState.Paused;
        public bool IsGameOver() => currentState == GameState.GameOver;
        public bool IsBetweenWaves() => currentState == GameState.BetweenWaves;
        public bool IsInGame() => currentState == GameState.Playing || currentState == GameState.BetweenWaves;

        #endregion

        // Editor utility
        [ContextMenu("Transition to Game Over")]
        private void DebugGameOver() => TransitionToGameOver();

        [ContextMenu("Transition to Playing")]
        private void DebugPlaying() => TransitionToPlaying();
    }
}

using System;
using UnityEngine;

/// <summary>
/// Singleton ScoreManager handles player points, spending & earning.
/// Use ScoreManager.Instance.AddPoints(amount) to award points.
/// Use ScoreManager.Instance.SpendPoints(cost) to attempt to spend points; returns true on success.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = UnityEngine.Object.FindObjectOfType<ScoreManager>();
                if (instance == null)
                {
                    // Create a new GameObject with the manager
                    GameObject go = new GameObject("ScoreManager");
                    instance = go.AddComponent<ScoreManager>();
                }
            }
            return instance;
        }
        private set { instance = value; }
    }

    public event Action<int> OnPointsChanged;

    [SerializeField] private int startingPoints = 0;
    private int points;

    public int Points => points;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    points = startingPoints;
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0) return;
        points += amount;
        OnPointsChanged?.Invoke(points);
    }

    public bool SpendPoints(int amount)
    {
        if (amount <= 0) return true; // Nothing to spend
        if (points < amount) return false;
        points -= amount;
        OnPointsChanged?.Invoke(points);
        return true;
    }

    public bool CanAfford(int amount)
    {
        return points >= amount;
    }

    public void SetPoints(int newPoints)
    {
        points = newPoints;
        OnPointsChanged?.Invoke(points);
    }
}

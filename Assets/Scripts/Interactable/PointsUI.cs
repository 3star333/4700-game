using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Updates a UI text object with the player's current points.
/// Attach to a Canvas object and assign the text reference.
/// </summary>
public class PointsUI : MonoBehaviour
{
    [Header("Assign either one of these")]
    public Text pointsText; // Unity UI
    public TextMeshProUGUI pointsTextTMP; // TextMeshPro

    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnPointsChanged += UpdatePoints;
            UpdatePoints(ScoreManager.Instance.Points);
        }
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnPointsChanged -= UpdatePoints;
    }

    private void UpdatePoints(int value)
    {
        if (pointsText != null)
            pointsText.text = $"Points: {value}";

        if (pointsTextTMP != null)
            pointsTextTMP.text = $"Points: {value}";
    }
}

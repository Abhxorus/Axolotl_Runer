using UnityEngine;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [Header("Puntuación de la Partida")]
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        // Configuramos el Singleton exclusivo para esta escena
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        ActualizarHUD();
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        ActualizarHUD();
    }

    private void ActualizarHUD()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore.ToString();
        }
    }
}
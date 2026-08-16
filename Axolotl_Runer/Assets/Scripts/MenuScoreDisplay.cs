using UnityEngine;
using TMPro;

public class MenuScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // Nos aseguramos de que el DataManager exista (por si corres la escena del menú directamente)
        if (DataManager.Instance != null)
        {
            scoreText.text = "Último Puntaje: " + DataManager.Instance.lastScore + "\n" +
                             "Récord: " + DataManager.Instance.highScore;
        }
        else
        {
            scoreText.text = "Último Puntaje: 0\nRécord: " + PlayerPrefs.GetInt("HighScore", 0);
        }
    }
}
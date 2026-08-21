using UnityEngine;

public class DataManager : MonoBehaviour
{
    // La instancia estática que permite acceder al script desde cualquier parte
    public static DataManager Instance;

    [Header("Datos Persistentes")]
    public int lastScore = 0;
    public int highScore = 0;

    [Header("Control de Escenas")]
    public bool showRecordsOnLoad = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // --- CORRECCIÓN: Cargamos el récord desde el disco duro de inmediato ---
            highScore = PlayerPrefs.GetInt("HighScore", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveScore(int currentScore)
    {
        lastScore = currentScore;

        // Si superaste tu récord, lo actualiza y lo guarda en la memoria del celular/PC
        if (lastScore > highScore)
        {
            highScore = lastScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
}
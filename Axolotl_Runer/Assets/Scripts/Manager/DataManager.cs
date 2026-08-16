using UnityEngine;

public class DataManager : MonoBehaviour
{
    // La instancia estática que permite acceder al script desde cualquier parte
    public static DataManager Instance;

    [Header("Datos Persistentes")]
    public int lastScore = 0;
    public int highScore = 0;

    void Awake()
    {
        // Configuramos el Singleton
        if (Instance == null)
        {
            Instance = this;
            // Evita que este objeto se destruya al cambiar de escena
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe uno (por ejemplo, si vuelves a cargar el nivel), destruye la copia
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Opcional: Cargar el puntaje máximo guardado en el dispositivo
        highScore = PlayerPrefs.GetInt("HighScore", 0);
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
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Esta función la llamaremos desde el botón
    public void IniciarJuego(string nombreDeTuEscena)
    {
        SceneManager.LoadScene(nombreDeTuEscena);
    }

    // De paso, te dejo la función para salir de la aplicación por si tienes un botón de Salir
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
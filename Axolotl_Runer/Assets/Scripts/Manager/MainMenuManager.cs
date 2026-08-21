using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip musicaMenuPrincipal;

    void Start()
    {
        // Se agregaron validaciones de seguridad y se quitaron las llaves extra
        if (AudioManager.Instance != null && musicaMenuPrincipal != null)
        {
            AudioManager.Instance.PlayMusic(musicaMenuPrincipal);
        }
    }

    // Esta función la llamaremos desde el botón
    public void IniciarJuego(string nombreDeTuEscena)
    {
        // Llamamos al TransitionManager en lugar del SceneManager directo
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.CargarEscena(nombreDeTuEscena);
        }
        else
        {
            // Respaldo por si abres el juego sin el TransitionManager
            SceneManager.LoadScene(nombreDeTuEscena);
        }
    }

    // De paso, te dejo la función para salir de la aplicación por si tienes un botón de Salir
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
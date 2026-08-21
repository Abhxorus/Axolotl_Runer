using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("Elementos Visuales")]
    public GameObject contenedorPrincipal;
    public GameObject fondoCargando;
    public Animator animBarrido;

    [Header("Ajustes")]
    public float duracionBarrido = 0.5f;
    public float tiempoMinimoCarga = 1.5f;

    private bool enTransicion = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        // Nos aseguramos de que el contenedor esté oculto al iniciar el juego
        if (contenedorPrincipal != null)
        {
            contenedorPrincipal.SetActive(false);
        }
    }

    public void CargarEscena(string nombreEscena)
    {
        if (enTransicion) return;
        StartCoroutine(SecuenciaDeCarga(nombreEscena));
    }

    private IEnumerator SecuenciaDeCarga(string nombreEscena)
    {
        enTransicion = true;
        // 1. Iniciamos el barrido (de izquierda al centro)
        contenedorPrincipal.SetActive(true);
        fondoCargando.SetActive(false);
        if (animBarrido != null) animBarrido.SetTrigger("Entrada");

        yield return new WaitForSeconds(duracionBarrido);

        // 2. La pantalla está tapada. Encendemos tu fondo bonito con el círculo
        fondoCargando.SetActive(true);

        // 3. Cargamos el nivel ocultos detrás del fondo
        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscena);
        operacion.allowSceneActivation = false;
        float tiempoTranscurrido = 0f;

        while (!operacion.isDone)
        {
            tiempoTranscurrido += Time.deltaTime;
            if (operacion.progress >= 0.9f && tiempoTranscurrido >= tiempoMinimoCarga)
            {
                operacion.allowSceneActivation = true;
            }
            yield return null;
        }

        // 4. Nivel cargado. Apagamos el fondo estático. 
        // El jugador no verá el juego aún porque la ImagenBarrido sigue tapando la pantalla.
        fondoCargando.SetActive(false);

        // 5. Continuamos el barrido (del centro hacia la derecha)
        if (animBarrido != null) animBarrido.SetTrigger("Salida");

        yield return new WaitForSeconds(duracionBarrido);

        contenedorPrincipal.SetActive(false);

        enTransicion = false;
    }
}
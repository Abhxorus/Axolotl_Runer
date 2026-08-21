using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Canales de Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Ajustes de Playlist")]
    public AudioClip[] playlist; // Aquí arrastras tus 3 (o más) canciones
    public float tiempoEntreCanciones = 3.0f; // Segundos de silencio entre pistas

    private List<AudioClip> cancionesBarajadas = new List<AudioClip>();
    private Coroutine playlistCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- NUEVO SISTEMA DE PLAYLIST ---

    public void IniciarPlaylist()
    {
        // Detenemos cualquier playlist anterior para que no se encimen
        if (playlistCoroutine != null) StopCoroutine(playlistCoroutine);

        // Apagamos el loop del reproductor para que las canciones terminen naturalmente
        if (musicSource != null) musicSource.loop = false;

        // Barajamos la lista y comenzamos
        BarajarPlaylist();
        playlistCoroutine = StartCoroutine(ReproducirPlaylist());
    }

    private void BarajarPlaylist()
    {
        cancionesBarajadas.Clear();
        cancionesBarajadas.AddRange(playlist);

        // Algoritmo para barajar aleatoriamente
        for (int i = 0; i < cancionesBarajadas.Count; i++)
        {
            AudioClip temp = cancionesBarajadas[i];
            int randomIndex = Random.Range(i, cancionesBarajadas.Count);
            cancionesBarajadas[i] = cancionesBarajadas[randomIndex];
            cancionesBarajadas[randomIndex] = temp;
        }
    }

    private IEnumerator ReproducirPlaylist()
    {
        int indiceActual = 0;

        while (true) // Bucle infinito para que la música nunca acabe en la partida
        {
            // Si llegamos al final de la lista, volvemos a barajar
            if (indiceActual >= cancionesBarajadas.Count)
            {
                AudioClip ultimaCancion = cancionesBarajadas[cancionesBarajadas.Count - 1];
                BarajarPlaylist();

                // Evitamos que la nueva lista empiece con la misma canción que acaba de terminar
                if (cancionesBarajadas[0] == ultimaCancion && cancionesBarajadas.Count > 1)
                {
                    AudioClip temp = cancionesBarajadas[0];
                    cancionesBarajadas.RemoveAt(0);
                    cancionesBarajadas.Add(temp);
                }

                indiceActual = 0;
            }

            // Reproducimos la canción actual
            AudioClip clipActual = cancionesBarajadas[indiceActual];
            musicSource.clip = clipActual;
            musicSource.Play();

            // Esperamos el tiempo que dura la canción MÁS los segundos de silencio que elegiste
            yield return new WaitForSeconds(clipActual.length + tiempoEntreCanciones);

            indiceActual++;
        }
    }

    public void DetenerMusica()
    {
        if (playlistCoroutine != null) StopCoroutine(playlistCoroutine);
        if (musicSource != null) musicSource.Stop();
    }

    // --- MÉTODOS ORIGINALES CONSERVADOS ---

    // Este método lo puedes seguir usando para el menú principal, donde quieres 1 sola pista en bucle
    public void PlayMusic(AudioClip musicClip)
    {
        if (playlistCoroutine != null) StopCoroutine(playlistCoroutine);

        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
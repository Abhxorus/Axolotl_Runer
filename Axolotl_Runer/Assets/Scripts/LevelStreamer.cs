using System.Collections.Generic;
using UnityEngine;

public class LevelStreamer : MonoBehaviour
{
    public enum TipoBioma { Agua, Tierra }

    [Header("Máquina de Biomas")]
    public TipoBioma biomaActual = TipoBioma.Agua;
    [Tooltip("Mínimo de segmentos que durará un bioma antes de cambiar")]
    public int minSegmentos = 5;
    [Tooltip("Máximo de segmentos que durará un bioma")]
    public int maxSegmentos = 10;
    private int segmentosRestantes;

    [Header("Prefabs de Agua")]
    public GameObject[] segmentosAgua;
    public GameObject transicionAguaATierra;

    [Header("Prefabs de Tierra")]
    public GameObject[] segmentosTierra;
    public GameObject transicionTierraAAgua;

    [Header("Configuración General")]
    public GameObject startingSegmentPrefab;
    public int poolSize = 6;
    public float segmentLength = 20f;
    public float despawnZ = -30f;
    public float scrollSpeed = 7.0f;
    public bool isGameOver = false;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private GameObject instantiatedStartSegment;

    void Start()
    {
        // Definimos cuántos bloques durará el primer bioma
        segmentosRestantes = Random.Range(minSegmentos, maxSegmentos);

        // 1. Instanciamos el segmento de inicio obligatoriamente en la posición Z = 0
        if (startingSegmentPrefab != null)
        {
            instantiatedStartSegment = Instantiate(startingSegmentPrefab, Vector3.zero, Quaternion.identity);
            instantiatedStartSegment.transform.SetParent(this.transform);
            activeSegments.Enqueue(instantiatedStartSegment);
        }

        // 2. Generamos el resto de la piscina (empezando con el bioma actual)
        for (int i = 1; i < poolSize; i++)
        {
            SpawnInitialSegment(i * segmentLength);
        }

        AudioManager.Instance.IniciarPlaylist();
    }

    void Update()
    {
        if (isGameOver) return;

        // Movemos todos los segmentos activos hacia atrás
        foreach (GameObject segment in activeSegments)
        {
            segment.transform.Translate(Vector3.back * (scrollSpeed * Time.deltaTime), Space.World);
        }

        // Revisamos si el primer segmento de la cola ya pasó el límite de despawn
        if (activeSegments.Peek().transform.position.z < despawnZ)
        {
            RecycleSegment();
        }
    }

    private void SpawnInitialSegment(float zPosition)
    {
        // Al iniciar la partida, llenamos la cola con segmentos de agua por defecto
        int randomIndex = Random.Range(0, segmentosAgua.Length);
        GameObject go = Instantiate(segmentosAgua[randomIndex], new Vector3(0, 0, zPosition), Quaternion.identity);
        go.transform.SetParent(this.transform);
        activeSegments.Enqueue(go);
    }

    private GameObject ObtenerSiguientePrefab()
    {
        segmentosRestantes--;

        // Si el contador llega a cero, toca cambiar de bioma usando las rampas
        if (segmentosRestantes <= 0)
        {
            if (biomaActual == TipoBioma.Agua)
            {
                biomaActual = TipoBioma.Tierra;
                segmentosRestantes = Random.Range(minSegmentos, maxSegmentos);
                return transicionAguaATierra;
            }
            else
            {
                biomaActual = TipoBioma.Agua;
                segmentosRestantes = Random.Range(minSegmentos, maxSegmentos);
                return transicionTierraAAgua;
            }
        }

        // Si no hay transición, entregamos un bloque normal del bioma actual
        if (biomaActual == TipoBioma.Agua)
        {
            return segmentosAgua[Random.Range(0, segmentosAgua.Length)];
        }
        else
        {
            return segmentosTierra[Random.Range(0, segmentosTierra.Length)];
        }
    }

    private void RecycleSegment()
    {
        // Sacamos el segmento viejo de la cola
        GameObject recycledSegment = activeSegments.Dequeue();

        // Encontramos la posición Z del segmento que está más lejos hacia adelante
        float maxZ = -9999f;
        foreach (GameObject segment in activeSegments)
        {
            if (segment.transform.position.z > maxZ)
            {
                maxZ = segment.transform.position.z;
            }
        }

        // Calculamos dónde debe aparecer el nuevo bloque
        Vector3 nuevaPosicion = new Vector3(0, 0, maxZ + segmentLength);

        // Llamamos a nuestra lógica para saber qué bloque toca (Tierra, Agua o Transición)
        GameObject prefabSiguiente = ObtenerSiguientePrefab();

        // Instanciamos el bloque correcto en la posición calculada
        GameObject newSegment = Instantiate(prefabSiguiente, nuevaPosicion, Quaternion.identity);
        newSegment.transform.SetParent(this.transform);

        // Destruimos el bloque viejo que ya pasó por la cámara
        Destroy(recycledSegment);

        // Agregamos el nuevo bloque a la fila para que se empiece a mover
        activeSegments.Enqueue(newSegment);
    }
}
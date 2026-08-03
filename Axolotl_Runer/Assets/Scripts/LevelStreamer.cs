using System.Collections.Generic;
using UnityEngine;

public class LevelStreamer : MonoBehaviour
{
    [Header("Configuración del Pool")]
    [Tooltip("El segmento que siempre aparecerá al inicio (ej. los nenúfares fijos)")]
    public GameObject startingSegmentPrefab; // <-- NUEVO
    public GameObject[] segmentPrefabs;
    public int poolSize = 6;
    public float segmentLength = 20f;

    [Tooltip("Punto en Z donde el segmento desaparece por detrás de la cámara")]
    public float despawnZ = -30f;

    [Header("Velocidad del Río")]
    public float scrollSpeed = 7.0f;

    public bool isGameOver = false;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private GameObject instantiatedStartSegment; // <-- NUEVO: Para identificarlo

    void Start()
    {
        // 1. Instanciamos el segmento de inicio obligatoriamente en la posición Z = 0
        if (startingSegmentPrefab != null)
        {
            instantiatedStartSegment = Instantiate(startingSegmentPrefab, Vector3.zero, Quaternion.identity);
            instantiatedStartSegment.transform.SetParent(this.transform);
            activeSegments.Enqueue(instantiatedStartSegment);
        }

        // 2. Generamos los segmentos restantes aleatoriamente (empezando desde el índice 1)
        for (int i = 1; i < poolSize; i++)
        {
            SpawnInitialSegment(i * segmentLength);
        }
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
        int randomIndex = Random.Range(0, segmentPrefabs.Length);
        GameObject go = Instantiate(segmentPrefabs[randomIndex], new Vector3(0, 0, zPosition), Quaternion.identity);
        go.transform.SetParent(this.transform);
        activeSegments.Enqueue(go);
    }

    private void RecycleSegment()
    {
        GameObject recycledSegment = activeSegments.Dequeue();

        // --- NUEVO: Si es el segmento de inicio, lo cambiamos por uno normal ---
        if (recycledSegment == instantiatedStartSegment)
        {
            int randomIndex = Random.Range(0, segmentPrefabs.Length);
            GameObject newSegment = Instantiate(segmentPrefabs[randomIndex], Vector3.zero, Quaternion.identity);
            newSegment.transform.SetParent(this.transform);

            Destroy(recycledSegment); // Lo destruimos para que no vuelva a aparecer
            recycledSegment = newSegment; // El nuevo toma su lugar en el ciclo
        }
        // ------------------------------------------------------------------------

        // Encontramos la posición Z del segmento que está más lejos hacia adelante
        float maxZ = -9999f;
        foreach (GameObject segment in activeSegments)
        {
            if (segment.transform.position.z > maxZ)
            {
                maxZ = segment.transform.position.z;
            }
        }

        // Colocamos el segmento reciclado justo detrás del último segmento
        recycledSegment.transform.position = new Vector3(0, 0, maxZ + segmentLength);

        // Reactivamos todas las monedas o items
        Transform[] todosLosHijos = recycledSegment.GetComponentsInChildren<Transform>(true);
        foreach (Transform hijo in todosLosHijos)
        {
            hijo.gameObject.SetActive(true);
        }

        activeSegments.Enqueue(recycledSegment);
    }
}
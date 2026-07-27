using System.Collections.Generic;
using UnityEngine;

public class LevelStreamer : MonoBehaviour
{
    [Header("Configuración del Pool")]
    public GameObject[] segmentPrefabs;
    public int poolSize = 6;
    public float segmentLength = 20f;

    [Tooltip("Punto en Z donde el segmento desaparece por detrás de la cámara")]
    public float despawnZ = -30f;

    [Header("Velocidad del Río")]
    public float scrollSpeed = 7.0f; // La velocidad que antes estaba en el jugador

    public bool isGameOver = false;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();

    void Start()
    {
        // Generamos los segmentos iniciales
        for (int i = 0; i < poolSize; i++)
        {
            SpawnInitialSegment(i * segmentLength);
        }
    }

    void Update()
    {
        if (isGameOver) return;

        // 1. Movemos todos los segmentos activos hacia atrás
        foreach (GameObject segment in activeSegments)
        {
            segment.transform.Translate(Vector3.back * (scrollSpeed * Time.deltaTime), Space.World);
        }

        // 2. Revisamos si el primer segmento de la cola ya pasó el límite de despawn
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
        activeSegments.Enqueue(recycledSegment);
    }
}
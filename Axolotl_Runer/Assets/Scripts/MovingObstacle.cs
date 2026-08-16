using System.Collections;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El modelo del obstáculo que se va a mover (debe ser hijo de este objeto)")]
    public Transform obstacleModel;

    [Header("Configuración de Movimiento")]
    [Tooltip("Hacia dónde se moverá en relación a su posición inicial (Ej: X = -5 lo mueve 5 unidades a la izquierda)")]
    public Vector3 moveOffset = new Vector3(-5f, 0f, 0f);

    [Tooltip("Velocidad de desplazamiento")]
    public float moveSpeed = 10f;

    private Vector3 initialLocalPos;
    private bool hasTriggered = false;

    void Awake()
    {
        if (obstacleModel != null)
        {
            // Guardamos el Punto A original
            initialLocalPos = obstacleModel.localPosition;
        }
    }

    // Fundamental para que funcione en tu Object Pool al reciclar segmentos
    void OnEnable()
    {
        hasTriggered = false;
        if (obstacleModel != null)
        {
            // Lo regresamos al Punto A cuando el segmento reaparece
            obstacleModel.localPosition = initialLocalPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta al jugador usando su script principal
        if (!hasTriggered && other.GetComponent<PlayerMovement>() != null)
        {
            hasTriggered = true;
            StartCoroutine(MoveRoutine());
        }
    }

    private IEnumerator MoveRoutine()
    {
        // El Punto B es su posición original más el offset que decidiste
        Vector3 targetPos = initialLocalPos + moveOffset;

        // Movemos el objeto poco a poco hasta que llegue a su destino
        while (Vector3.Distance(obstacleModel.localPosition, targetPos) > 0.01f)
        {
            obstacleModel.localPosition = Vector3.MoveTowards(
                obstacleModel.localPosition,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            yield return null; // Esperamos al siguiente frame
        }

        // Lo fijamos exactamente en el Punto B al terminar
        obstacleModel.localPosition = targetPos;
    }
}
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3f, -6f);

    [Header("Ajustes de Suavizado")]
    [Tooltip("Velocidad para seguir saltos y el avance hacia adelante")]
    public float smoothSpeedYZ = 15f;
    [Tooltip("Velocidad para moverse a los lados. Mantenlo bajo para máxima suavidad")]
    public float smoothSpeedX = 5f;

    [Header("Límites y Carriles")]
    public float minY = 1.0f;
    [Tooltip("0 = La cámara no se mueve a los lados. 1 = Sigue al jugador por completo. 0.3 = Se asoma un poco al carril")]
    [Range(0f, 1f)]
    public float multiplicadorX = 0.3f;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculamos la posición ideal original
        Vector3 desiredPosition = target.position + offset;

        // 2. Imponemos el límite de agua
        if (desiredPosition.y < minY)
        {
            desiredPosition.y = minY;
        }

        // 3. Separamos las posiciones actuales
        Vector3 currentPos = transform.position;

        // 4. Suavizamos Y y Z rápidamente para que no se atrase al correr
        float newY = Mathf.Lerp(currentPos.y, desiredPosition.y, smoothSpeedYZ * Time.deltaTime);
        float newZ = Mathf.Lerp(currentPos.z, desiredPosition.z, smoothSpeedYZ * Time.deltaTime);

        // 5. Para el eje X, calculamos una posición reducida usando el multiplicador
        // y la suavizamos de forma independiente
        float targetX = (target.position.x * multiplicadorX) + offset.x;
        float newX = Mathf.Lerp(currentPos.x, targetX, smoothSpeedX * Time.deltaTime);

        // 6. Aplicamos la posición combinada
        transform.position = new Vector3(newX, newY, newZ);
    }
}
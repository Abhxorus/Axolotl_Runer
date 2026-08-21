using UnityEngine;
using System.Collections;

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

    private Vector3 shakeOffset = Vector3.zero;

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

    public void TriggerShake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Generamos un desplazamiento aleatorio en los ejes X e Y
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Devolvemos la cámara a la normalidad exacta
        shakeOffset = Vector3.zero;
    }
}
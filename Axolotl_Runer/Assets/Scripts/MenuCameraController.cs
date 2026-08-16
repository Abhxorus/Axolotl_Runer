using System.Collections;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    [Header("Ajustes de Rotación")]
    [Tooltip("Qué tan rápido gira la cámara hacia la nueva pared")]
    public float rotationSpeed = 3.0f;

    private bool isRotating = false;

    // Esta es la función pública que llamarán tus botones
    public void RotateToWall(float targetAngleY)
    {
        if (!isRotating)
        {
            StartCoroutine(RotateRoutine(targetAngleY));
        }
    }

    private IEnumerator RotateRoutine(float targetAngleY)
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        // Creamos la rotación destino basada solo en el eje Y (giro horizontal)
        Quaternion endRotation = Quaternion.Euler(0, targetAngleY, 0);

        float timeElapsed = 0f;

        // Giramos suavemente hasta llegar al 100% (1f)
        while (timeElapsed < 1f)
        {
            timeElapsed += Time.deltaTime * rotationSpeed;

            // Slerp hace que la rotación sea esférica y fluida
            // SmoothStep le da ese efecto de acelerar al inicio y frenar suave al final
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, Mathf.SmoothStep(0f, 1f, timeElapsed));

            yield return null;
        }

        // Nos aseguramos de quedar exactamente en el ángulo pedido
        transform.rotation = endRotation;
        isRotating = false;
    }
}
using System.Collections;
using UnityEngine;

public class JumpingFish : MonoBehaviour
{
    // Definimos las modalidades posibles
    public enum ModoSalto { Vertical, Parabolico }

    [Header("Referencias")]
    [Tooltip("Arrastra aquí al hijo (el modelo del pez)")]
    public Transform fishModel;

    [Header("Configuración General")]
    public ModoSalto modalidad = ModoSalto.Vertical;
    public float jumpHeight = 4.0f;
    public float jumpDuration = 1.0f;

    [Header("Configuración Parabólico")]
    [Tooltip("Distancia que recorrerá el pez al saltar. (Ej: X = 5 para saltar un carril a la derecha)")]
    public Vector3 offsetSalto = new Vector3(5f, 0f, 0f);

    private bool hasJumped = false;
    private Vector3 initialLocalPos;

    void Awake()
    {
        if (fishModel != null)
        {
            // Guardamos la posición original (bajo el agua)
            initialLocalPos = fishModel.localPosition;
        }
    }

    // OnEnable se ejecuta cada vez que el Object Pool recicla el segmento
    void OnEnable()
    {
        hasJumped = false;
        if (fishModel != null)
        {
            // Reseteamos al pez a su posición inicial
            fishModel.localPosition = initialLocalPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasJumped && other.GetComponent<PlayerMovement>() != null)
        {
            hasJumped = true;
            StartCoroutine(JumpRoutine());
        }
    }

    private IEnumerator JumpRoutine()
    {
        float timeElapsed = 0f;
        Vector3 startPos = initialLocalPos;

        // Calculamos el destino dependiendo de la modalidad elegida
        Vector3 endPos = startPos;
        if (modalidad == ModoSalto.Parabolico)
        {
            // Sumamos el offset al punto de inicio
            endPos = startPos + offsetSalto;
        }

        while (timeElapsed < jumpDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / jumpDuration;

            // Interpolación lineal para el avance (ejes X y Z)
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, progress);

            // La onda Seno para el arco de altura (eje Y)
            currentPos.y = startPos.y + (Mathf.Sin(progress * Mathf.PI) * jumpHeight);

            fishModel.localPosition = currentPos;

            yield return null;
        }

        // Aseguramos que termine exactamente en su posición final
        fishModel.localPosition = endPos;
    }
}
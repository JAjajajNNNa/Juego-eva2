using UnityEngine;

public class CamaraDinamica : MonoBehaviour
{
    [Header("Objetos a Seguir")]
    public Transform jugador;
    public Transform oponente;

    [Header("Configuración del Zoom")]
    public float sizeMinimo = 5f;       // Zoom máximo cuando están cerca
    public float sizeMaximo = 10f;      // Zoom mínimo cuando están muy lejos
    public float limiteDistancia = 8f;  // Distancia a la cual la cámara alcanza el zoom máximo
    public float velocidadZoom = 2f;    // Suavizado del cambio de zoom

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (jugador == null || oponente == null) return;

        // 1. Calcular el centro entre ambos personajes
        Vector3 puntoMedio = (jugador.position + oponente.position) / 2f;
        
        // Mantener la posición Z de la cámara (por defecto -10)
        transform.position = new Vector3(puntoMedio.x, transform.position.y, transform.position.z);

        // 2. Calcular la distancia horizontal entre los dos
        float distancia = Mathf.Abs(jugador.position.x - oponente.position.x);

        // 3. Convertir la distancia a un valor de tamaño de cámara
        float t = Mathf.Clamp01(distancia / limiteDistancia);
        float sizeDeseado = Mathf.Lerp(sizeMinimo, sizeMaximo, t);

        // 4. Aplicar zoom de forma suave
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, sizeDeseado, Time.deltaTime * velocidadZoom);
    }
}
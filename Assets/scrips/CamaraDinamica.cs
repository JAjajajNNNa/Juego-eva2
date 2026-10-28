using UnityEngine;

public class CamaraDinamica : MonoBehaviour
{
    [Header("Objetos a Seguir")]
    public Transform jugador;
    public Transform oponente;

    [Header("Configuración del Zoom")]
    public float sizeMinimo = 5f;     
    public float sizeMaximo = 10f;      
    public float limiteDistancia = 8f;  
    public float velocidadZoom = 2f;    

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (jugador == null || oponente == null) return;

       
        Vector3 puntoMedio = (jugador.position + oponente.position) / 2f;
        
        
        transform.position = new Vector3(puntoMedio.x, transform.position.y, transform.position.z);

        
        float distancia = Mathf.Abs(jugador.position.x - oponente.position.x);

        
        float t = Mathf.Clamp01(distancia / limiteDistancia);
        float sizeDeseado = Mathf.Lerp(sizeMinimo, sizeMaximo, t);

        
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, sizeDeseado, Time.deltaTime * velocidadZoom);
    }
}
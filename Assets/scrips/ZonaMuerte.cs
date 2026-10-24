using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    public AdministradorJuego administradorJuego;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡El Jugador cayó fuera de la plataforma! Gana el Oponente.");
            if (administradorJuego != null)
            {
                administradorJuego.DeclararGanador(false); // Falso = Perdió el jugador
            }
        }
        else if (other.CompareTag("Oponente"))
        {
            Debug.Log("¡El Oponente cayó fuera de la plataforma! Gana el Jugador.");
            if (administradorJuego != null)
            {
                administradorJuego.DeclararGanador(true); // Verdadero = Ganó el jugador
            }
        }
    }
}
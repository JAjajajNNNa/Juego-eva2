using UnityEngine;

[CreateAssetMenu(fileName = "NuevaCarta", menuName = "JuegoCartas/Carta")]

public class CartaDatos : ScriptableObject
{
    public string nombreCarta; // Ej: "As de Corazones"
    public int valor;          // Ej: 1, 2, 10, 11
    public Sprite imagenCarta; // Sprite/Imagen de la carta
}
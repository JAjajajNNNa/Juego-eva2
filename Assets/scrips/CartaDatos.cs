using UnityEngine;

[CreateAssetMenu(fileName = "NuevaCarta", menuName = "JuegoCartas/Carta")]

public class CartaDatos : ScriptableObject
{
    public string nombreCarta; 
    public int valor;          
    public Sprite imagenCarta; 
}
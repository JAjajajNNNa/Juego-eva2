using UnityEngine;

public static class DatosJuego
{
    public enum ModoJuego { ModificadoresRonda, PorPintas }

    public static ModoJuego modoSeleccionado = ModoJuego.ModificadoresRonda;
    public static float volumenGeneral = 1.0f; 

    
    public static void RegistrarVictoriaRonda()
    {
        int victorias = PlayerPrefs.GetInt("VictoriasRonda", 0);
        victorias++;
        PlayerPrefs.SetInt("VictoriasRonda", victorias);
        PlayerPrefs.Save();
    }

    public static bool EsModoPintasDesbloqueado()
    {
        return PlayerPrefs.GetInt("VictoriasRonda", 0) >= 3;
    }
}
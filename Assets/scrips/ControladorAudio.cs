using UnityEngine;

public class ControladorAudio : MonoBehaviour
{
    public static ControladorAudio Instancia;

    public AudioSource fuenteMusica;
    public AudioSource fuenteSFX;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
       
        if (fuenteMusica != null) fuenteMusica.volume = DatosJuego.volumenGeneral;
        if (fuenteSFX != null) fuenteSFX.volume = DatosJuego.volumenGeneral;
    }

    public void ReproducirSFX(AudioClip clip)
    {
        if (fuenteSFX != null && clip != null)
        {
            fuenteSFX.PlayOneShot(clip, DatosJuego.volumenGeneral);
        }
    }
}
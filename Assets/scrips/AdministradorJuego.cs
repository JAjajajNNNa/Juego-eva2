using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AdministradorJuego : MonoBehaviour
{
    public enum TipoModificador { Normal, DoblePunto, ReglasInvertidas, GravedadBaja }
    public enum EstadoJuego { EsperandoJugador, TurnoOponente, Resolviendo, FinDelJuego }

    [Header("Estado del Juego")]
    public EstadoJuego estadoActual = EstadoJuego.EsperandoJugador;

    [Header("Mazo de Cartas")]
    public List<CartaDatos> mazoOriginal;
    private List<CartaDatos> mazoActual;

    [Header("Configuración de Reglas y Parámetros")]
    public float fuerzaCastigoPerder = 2.4f;
    public float fuerzaRecuperacionJoker = 2.5f;
    public float distanciaAtaque = 0.2f;
    public float duracionAtaque = 0.25f;

    [Header("Sistema de Vidas (Al mejor de 3)")]
    public int vidasMaximas = 3;
    private int vidasJugador;
    private int vidasOponente;

    [Header("Referencias UI")]
    public TextMeshProUGUI textoMarcador;
    public TextMeshProUGUI textoModificador;
    public Image imagenCartaUI;
    public Button botonPedir;
    public Button botonPlantarse;

    [Header("Referencias Personajes")]
    public Transform jugador;
    public Transform oponente;
    public Animator animJugador;
    public Animator animOponente;

    [Header("Efectos de Sonido (Opcional)")]
    public AudioClip clipAtaque;
    public AudioClip clipCarta;

    private Vector3 posInicialJugador;
    private Vector3 posInicialOponente;

    private CartaDatos cartaJugadorActual;
    private CartaDatos cartaOponenteActual;
    
    private bool yaCambioOponente = false;
    private TipoModificador modificadorRondaActual = TipoModificador.Normal;

    void Start()
    {
        posInicialJugador = jugador.position;
        posInicialOponente = oponente.position;

        vidasJugador = vidasMaximas;
        vidasOponente = vidasMaximas;

        InicializarMazo();
        ConfigurarBotones();
        ActualizarMarcadorUI();
        IniciarNuevaRonda();
    }

    void InicializarMazo()
    {
        mazoActual = new List<CartaDatos>(mazoOriginal);

        for (int i = mazoActual.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CartaDatos temp = mazoActual[i];
            mazoActual[i] = mazoActual[randomIndex];
            mazoActual[randomIndex] = temp;
        }
    }

    void ConfigurarBotones()
    {
        if (botonPedir != null)
        {
            botonPedir.onClick.RemoveAllListeners();
            botonPedir.onClick.AddListener(JugadorPideCambio);
        }

        if (botonPlantarse != null)
        {
            botonPlantarse.onClick.RemoveAllListeners();
            botonPlantarse.onClick.AddListener(JugadorSePlanta);
        }
    }

    void IniciarNuevaRonda()
    {
        if (estadoActual == EstadoJuego.FinDelJuego) return;

        estadoActual = EstadoJuego.EsperandoJugador;
        yaCambioOponente = false;

        if (mazoActual == null || mazoActual.Count < 2)
        {
            InicializarMazo();
        }

        cartaJugadorActual = ObtenerCartaAleatoria();
        cartaOponenteActual = ObtenerCartaAleatoria();

        if (DatosJuego.modoSeleccionado == DatosJuego.ModoJuego.PorPintas)
        {
            ActualizarModificadorPorPinta();
        }
        else
        {
            SeleccionarModificadorAleatorio();
        }

        ActualizarUI();
        
        if (botonPedir != null) botonPedir.interactable = true;
        if (botonPlantarse != null) botonPlantarse.interactable = true;
    }

    void SeleccionarModificadorAleatorio()
    {
        modificadorRondaActual = (TipoModificador)Random.Range(0, 4);

        if (textoModificador != null)
        {
            switch (modificadorRondaActual)
            {
                case TipoModificador.Normal:
                    textoModificador.text = "Ronda Normal";
                    break;
                case TipoModificador.DoblePunto:
                    textoModificador.text = "¡Ronda Fuerte! (Doble Empuje)";
                    break;
                case TipoModificador.ReglasInvertidas:
                    textoModificador.text = "Reglas Invertidas: ¡Gana la carta MENOR!";
                    break;
                case TipoModificador.GravedadBaja:
                    textoModificador.text = "Gravedad Baja: ¡Empujes más altos!";
                    break;
            }
        }
    }

    void ActualizarModificadorPorPinta()
    {
        if (cartaJugadorActual == null) return;

        string nombreLwr = cartaJugadorActual.nombreCarta.ToLower();

        if (nombreLwr.Contains("diamante"))
        {
            modificadorRondaActual = TipoModificador.DoblePunto;
            if (textoModificador != null)
                textoModificador.text = "Carta Diamante: ¡Doble Empuje!";
        }
        else if (nombreLwr.Contains("pica"))
        {
            modificadorRondaActual = TipoModificador.ReglasInvertidas;
            if (textoModificador != null)
                textoModificador.text = "Carta Pica: ¡Reglas Invertidas! (Gana la menor)";
        }
        else if (nombreLwr.Contains("trebol"))
        {
            modificadorRondaActual = TipoModificador.GravedadBaja;
            if (textoModificador != null)
                textoModificador.text = "Carta Trébol: ¡Gravedad Baja!";
        }
        else if (nombreLwr.Contains("comodin") || nombreLwr.Contains("joker"))
        {
            modificadorRondaActual = TipoModificador.Normal;
            if (textoModificador != null)
                textoModificador.text = "¡COMODÍN! Fuerza de recuperación activada";
        }
        else
        {
            modificadorRondaActual = TipoModificador.Normal;
            if (textoModificador != null)
                textoModificador.text = "Ronda Normal";
        }
    }

    CartaDatos ObtenerCartaAleatoria()
    {
        if (mazoActual.Count == 0) InicializarMazo();

        CartaDatos carta = mazoActual[0];
        mazoActual.RemoveAt(0);
        return carta;
    }

    void ActualizarUI()
    {
        if (cartaJugadorActual != null && imagenCartaUI != null)
        {
            imagenCartaUI.sprite = cartaJugadorActual.imagenCarta;
            imagenCartaUI.gameObject.SetActive(true);
        }
    }

    void ActualizarMarcadorUI()
    {
        if (textoMarcador != null)
        {
            textoMarcador.text = $"Jugador: {vidasJugador} | Oponente: {vidasOponente}";
        }
    }

    public void JugadorPideCambio()
    {
        if (estadoActual != EstadoJuego.EsperandoJugador) return;

        cartaJugadorActual = ObtenerCartaAleatoria();

        if (ControladorAudio.Instancia != null)
        {
            ControladorAudio.Instancia.ReproducirSFX(clipCarta);
        }

        if (DatosJuego.modoSeleccionado == DatosJuego.ModoJuego.PorPintas)
        {
            ActualizarModificadorPorPinta();
        }

        ActualizarUI();
        FinalizarTurnoJugador();
    }

    public void JugadorSePlanta()
    {
        if (estadoActual != EstadoJuego.EsperandoJugador) return;

        FinalizarTurnoJugador();
    }

    void FinalizarTurnoJugador()
    {
        if (botonPedir != null) botonPedir.interactable = false;
        if (botonPlantarse != null) botonPlantarse.interactable = false;

        estadoActual = EstadoJuego.TurnoOponente;
        ProcesarTurnoOponente();
    }

    void ProcesarTurnoOponente()
    {
        int umbralBot = 6;
        if (vidasOponente < vidasJugador)
        {
            umbralBot = 8;
        }

        if (!yaCambioOponente && cartaOponenteActual.valor < umbralBot)
        {
            cartaOponenteActual = ObtenerCartaAleatoria();
            yaCambioOponente = true;
        }

        estadoActual = EstadoJuego.Resolviendo;
        StartCoroutine(ResolverRondaCo());
    }

    IEnumerator ResolverRondaCo()
    {
        yield return new WaitForSeconds(0.4f);

        VerificarEfectoJoker(jugador, cartaJugadorActual, true);
        VerificarEfectoJoker(oponente, cartaOponenteActual, false);

        yield return new WaitForSeconds(0.3f);

        int valorJ = cartaJugadorActual.valor;
        int valorO = cartaOponenteActual.valor;

        bool ganaJugador = false;
        bool ganaOponente = false;

        if (modificadorRondaActual == TipoModificador.ReglasInvertidas)
        {
            if (valorJ < valorO) ganaJugador = true;
            else if (valorO < valorJ) ganaOponente = true;
        }
        else
        {
            if (valorJ > valorO) ganaJugador = true;
            else if (valorO > valorJ) ganaOponente = true;
        }

        if (ganaJugador)
        {
            yield return StartCoroutine(EjecutarAtaque(jugador, oponente, animJugador, true));
        }
        else if (ganaOponente)
        {
            yield return StartCoroutine(EjecutarAtaque(oponente, jugador, animOponente, false));
        }

        yield return new WaitForSeconds(0.6f);

        if (estadoActual != EstadoJuego.FinDelJuego)
        {
            IniciarNuevaRonda();
        }
    }

    void VerificarEfectoJoker(Transform personaje, CartaDatos carta, bool esJugador)
    {
        if (carta == null) return;

        string nombreLwr = carta.nombreCarta.ToLower();

        if (nombreLwr.Contains("comodin") || nombreLwr.Contains("joker") || carta.valor == 15)
        {
            Rigidbody2D rb = personaje.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float direccionRecuperacion = esJugador ? 1f : -1f;
                rb.AddForce(new Vector2(direccionRecuperacion * fuerzaRecuperacionJoker, 2f), ForceMode2D.Impulse);
            }
        }
    }

    IEnumerator EjecutarAtaque(Transform atacante, Transform defensor, Animator animAtacante, bool atacanteEsJugador)
    {
        if (animAtacante != null)
        {
            animAtacante.SetTrigger("AtaqueTrigger");
        }

        if (ControladorAudio.Instancia != null)
        {
            ControladorAudio.Instancia.ReproducirSFX(clipAtaque);
        }

        if (atacante != null && defensor != null)
        {
            Vector3 posInicial = atacante.position;
            float direccion = atacanteEsJugador ? 1f : -1f;
            Vector3 posAtaque = posInicial + new Vector3(direccion * distanciaAtaque, 0, 0);

            float tiempo = 0;
            while (tiempo < duracionAtaque)
            {
                atacante.position = Vector3.Lerp(posInicial, posAtaque, tiempo / duracionAtaque);
                tiempo += Time.deltaTime;
                yield return null;
            }

            Rigidbody2D rbDefensor = defensor.GetComponent<Rigidbody2D>();
            if (rbDefensor != null)
            {
                float multiplicadorFuerza = (modificadorRondaActual == TipoModificador.DoblePunto) ? 2.0f : 1.0f;
                float fuerzaY = (modificadorRondaActual == TipoModificador.GravedadBaja) ? 3.5f : 1.0f;

                rbDefensor.AddForce(new Vector2(direccion * fuerzaCastigoPerder * multiplicadorFuerza, fuerzaY), ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(0.15f);

            tiempo = 0;
            while (tiempo < duracionAtaque)
            {
                atacante.position = Vector3.Lerp(posAtaque, posInicial, tiempo / duracionAtaque);
                tiempo += Time.deltaTime;
                yield return null;
            }
        }
    }

    // Se activa cuando alguien cae al vacío (detectado por ZonaMuerte.cs)
    public void DeclararGanador(bool ganoJugador)
    {
        if (estadoActual == EstadoJuego.FinDelJuego) return;

        if (ganoJugador)
        {
            vidasOponente--;
        }
        else
        {
            vidasJugador--;
        }

        ActualizarMarcadorUI();

        if (vidasJugador <= 0 || vidasOponente <= 0)
        {
            estadoActual = EstadoJuego.FinDelJuego;
            
            if (botonPedir != null) botonPedir.interactable = false;
            if (botonPlantarse != null) botonPlantarse.interactable = false;

            if (textoModificador != null)
            {
                if (vidasJugador > 0)
                {
                    textoModificador.text = "¡VICTORIA TOTAL!";

                    // Registra victoria si ganó en Modo Ronda
                    if (DatosJuego.modoSeleccionado == DatosJuego.ModoJuego.ModificadoresRonda)
                    {
                        DatosJuego.RegistrarVictoriaRonda();
                    }
                }
                else
                {
                    textoModificador.text = "¡DERROTA FINAL!";
                }
            }

            Invoke(nameof(VolverAlMenu), 3.0f);
        }
        else
        {
            StartCoroutine(ReposicionarPersonajes());
        }
    }

    IEnumerator ReposicionarPersonajes()
    {
        yield return new WaitForSeconds(0.8f);

        Rigidbody2D rbJ = jugador.GetComponent<Rigidbody2D>();
        Rigidbody2D rbO = oponente.GetComponent<Rigidbody2D>();

        if (rbJ) { rbJ.linearVelocity = Vector2.zero; rbJ.angularVelocity = 0f; }
        if (rbO) { rbO.linearVelocity = Vector2.zero; rbO.angularVelocity = 0f; }

        jugador.position = posInicialJugador;
        oponente.position = posInicialOponente;

        IniciarNuevaRonda();
    }

    void VolverAlMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
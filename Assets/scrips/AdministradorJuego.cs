using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AdministradorJuego : MonoBehaviour
{
    [Header("Mazo de Cartas")]
    public List<CartaDatos> mazoOriginal;
    private List<CartaDatos> mazoActual;

    [Header("Configuración de Reglas y Parámetros")]
    public float fuerzaCastigoPerder = 2.4f;
    public float fuerzaRecuperacionJoker = 2.5f;
    public float distanciaAtaque = 0.2f;
    public float duracionAtaque = 0.25f;

    [Tooltip("Si la carta del bot vale MENOS que esto (ej. 6), cambiará su carta 1 sola vez.")]
    public int umbralCambioOponente = 6; 

    [Header("Referencias UI")]
    public Image imagenCartaUI;
    public Button botonPedir;
    public Button botonPlantarse;

    [Header("Referencias Personajes")]
    public Transform jugador;
    public Transform oponente;
    public Animator animJugador;
    public Animator animOponente;

    private CartaDatos cartaJugadorActual;
    private CartaDatos cartaOponenteActual;
    
    private bool yaCambioJugador = false;
    private bool yaCambioOponente = false;
    private bool juegoTerminado = false;

    void Start()
    {
        InicializarMazo();
        ConfigurarBotones();
        IniciarNuevaRonda();
    }

    void InicializarMazo()
    {
        mazoActual = new List<CartaDatos>(mazoOriginal);

        // Barajamos el mazo completo con Fisher-Yates
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
        if (juegoTerminado) return;

        yaCambioJugador = false;
        yaCambioOponente = false;

        if (mazoActual == null || mazoActual.Count < 2)
        {
            InicializarMazo();
        }

        cartaJugadorActual = ObtenerCartaAleatoria();
        cartaOponenteActual = ObtenerCartaAleatoria();

        ActualizarUI();
        ActivarBotonesUI(true);
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

    public void JugadorPideCambio()
    {
        if (yaCambioJugador || juegoTerminado) return;

        cartaJugadorActual = ObtenerCartaAleatoria();
        yaCambioJugador = true;
        ActualizarUI();

        TurnoOponente();
    }

    public void JugadorSePlanta()
    {
        if (juegoTerminado) return;
        TurnoOponente();
    }

    void TurnoOponente()
    {
        ActivarBotonesUI(false);

        if (!yaCambioOponente && cartaOponenteActual.valor < umbralCambioOponente)
        {
            cartaOponenteActual = ObtenerCartaAleatoria();
            yaCambioOponente = true;
        }

        StartCoroutine(ResolverRondaCo());
    }

    IEnumerator ResolverRondaCo()
    {
        yield return new WaitForSeconds(0.6f);

        // 1. Detectar si salieron Comodines
        VerificarEfectoJoker(jugador, cartaJugadorActual, true);
        VerificarEfectoJoker(oponente, cartaOponenteActual, false);

        yield return new WaitForSeconds(0.4f);

        int valorJugador = cartaJugadorActual.valor;
        int valorOponente = cartaOponenteActual.valor;

        // 2. Resolver combate físico
        if (valorJugador > valorOponente)
        {
            yield return StartCoroutine(EjecutarAtaque(jugador, oponente, animJugador, true));
        }
        else if (valorOponente > valorJugador)
        {
            yield return StartCoroutine(EjecutarAtaque(oponente, jugador, animOponente, false));
        }

        yield return new WaitForSeconds(0.8f);

        if (!juegoTerminado)
        {
            IniciarNuevaRonda();
        }
    }

    void VerificarEfectoJoker(Transform personaje, CartaDatos carta, bool esJugador)
    {
        if (carta == null) return;

        string nombreLwr = carta.nombreCarta.ToLower();

        // Detecta "comodin", "joker" o valor 15
        if (nombreLwr.Contains("comodin") || nombreLwr.Contains("joker") || carta.valor == 15)
        {
            Rigidbody2D rb = personaje.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float direccionRecuperacion = esJugador ? 1f : -1f;
                rb.AddForce(new Vector2(direccionRecuperacion * fuerzaRecuperacionJoker, 2f), ForceMode2D.Impulse);
                Debug.Log($"¡Comodín activado! {personaje.name} vuelve al centro.");
            }
        }
    }

    IEnumerator EjecutarAtaque(Transform atacante, Transform defensor, Animator animAtacante, bool atacanteEsJugador)
    {
        if (animAtacante != null)
            animAtacante.SetTrigger("AtaqueTrigger");

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
                rbDefensor.AddForce(new Vector2(direccion * fuerzaCastigoPerder, 1f), ForceMode2D.Impulse);
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

    public void DeclararGanador(bool ganoJugador)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        ActivarBotonesUI(false);
        Debug.Log(ganoJugador ? "¡VICTORIA!" : "¡DERROTA!");

        // Reinicia la partida 2.5 segundos después de la caída
        Invoke(nameof(ReiniciarEscena), 2.5f);
    }

    void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ActivarBotonesUI(bool estado)
    {
        if (botonPedir != null) botonPedir.interactable = estado && !yaCambioJugador;
        if (botonPlantarse != null) botonPlantarse.interactable = estado;
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Paneles UI")]
    public GameObject panelMenuPrincipal;
    public GameObject panelAjustes;

    [Header("Botones UI")]
    public Button botonModoPintas; 

    [Header("Slider General")]
    public Slider sliderVolumenGeneral;

    void Start()
    {
        if (sliderVolumenGeneral != null)
        {
            sliderVolumenGeneral.value = DatosJuego.volumenGeneral;
            sliderVolumenGeneral.onValueChanged.AddListener(CambiarVolumenGeneral);
        }

      
        if (botonModoPintas != null)
        {
            botonModoPintas.interactable = DatosJuego.EsModoPintasDesbloqueado();
        }

        AbrirMenuPrincipal();
    }

    public void CambiarVolumenGeneral(float valor)
    {
        DatosJuego.volumenGeneral = valor;
    }

    public void JugarModoRonda()
    {
        DatosJuego.modoSeleccionado = DatosJuego.ModoJuego.ModificadoresRonda;
        SceneManager.LoadScene("SampleScene");
    }

    public void JugarModoPintas()
    {
        
        if (DatosJuego.EsModoPintasDesbloqueado())
        {
            DatosJuego.modoSeleccionado = DatosJuego.ModoJuego.PorPintas;
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void AbrirAjustes()
    {
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(false);
        if (panelAjustes != null) panelAjustes.SetActive(true);
    }

    public void CerrarAjustes()
    {
        if (panelAjustes != null) panelAjustes.SetActive(false);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    private void AbrirMenuPrincipal()
    {
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
        if (panelAjustes != null) panelAjustes.SetActive(false);
    }
}
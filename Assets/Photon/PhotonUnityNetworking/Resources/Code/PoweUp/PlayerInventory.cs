using UnityEngine;
using Photon.Pun;
using UnityEngine.UI; 

public class PlayerInventory : MonoBehaviourPun
{
    [Header("Item Guardado")]
    public TipoItem itemActual = TipoItem.Ninguno;

    [Header("Icono UI")]
    public Sprite iconoProyectil;
    public Sprite iconoEscudo;
    public Sprite iconoMultiplicador;

    private Image iconoUI; 

    void Start()
    {
        if (photonView.IsMine)
        {
            bool soyJ1 = PhotonNetwork.IsMasterClient;
            
            string nombreMiIcono = soyJ1 ? "IconoItemUI_J1" : "IconoItemUI_J2";

            GameObject miIconoEnEscena = GameObject.Find(nombreMiIcono);
            
            if (miIconoEnEscena != null)
            {
                iconoUI = miIconoEnEscena.GetComponent<Image>();
                ActualizarUI();
            }
            else
            {
                Debug.LogWarning("No se encontro " + nombreMiIcono + " en el Canvas.");
            }

            string nombreIconoRival = soyJ1 ? "IconoItemUI_J2" : "IconoItemUI_J1";
            GameObject iconoRivalEnEscena = GameObject.Find(nombreIconoRival);
            
            if (iconoRivalEnEscena != null)
            {
                iconoRivalEnEscena.SetActive(false);
            }
        }
    }

    public void RecogerItem(TipoItem nuevoItem)
    {
        if (!photonView.IsMine) return;

        // sobreescibir el item por uno nuevo 
        itemActual = nuevoItem;
        Debug.Log("¡Recogiste un: " + itemActual.ToString() + "!");
        
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (iconoUI == null) return;

        if (itemActual == TipoItem.Ninguno)
        {
            iconoUI.enabled = false;
            return;
        }

        iconoUI.enabled = true;

        switch (itemActual)
        {
            case TipoItem.Proyectil:
                iconoUI.sprite = iconoProyectil;
                break;
            case TipoItem.Escudo:
                iconoUI.sprite = iconoEscudo;
                break;
            case TipoItem.Multiplicador:
                iconoUI.sprite = iconoMultiplicador;
                break;
        }
    }

    void Update()
    {
        
    }
}
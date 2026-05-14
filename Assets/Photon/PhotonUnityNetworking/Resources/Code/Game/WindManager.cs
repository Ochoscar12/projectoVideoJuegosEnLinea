using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable; 

public class WindManager : MonoBehaviourPunCallbacks
{
    public static WindManager Instance;

    [Header("Configuración del Viento")]
    public float minTiempoEntreVientos = 5f;
    public float maxTiempoEntreVientos = 12f;
    public float duracionViento = 4f;
    public float fuerzaViento = 20f; 

    [Header("UI del Viento - Jugador 1 (Izquierda)")]
    public GameObject panelVientoJ1; 
    public TextMeshProUGUI textoDireccionJ1;    

    [Header("UI del Viento - Jugador 2 (Derecha)")]
    public GameObject panelVientoJ2; 
    public TextMeshProUGUI textoDireccionJ2; 

    [HideInInspector] public float direccionVientoActual = 0f; 

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Apagar ambos paneles al iniciar
        if (panelVientoJ1 != null) panelVientoJ1.SetActive(false);
        if (panelVientoJ2 != null) panelVientoJ2.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CicloDeViento());
        }
    }

    System.Collections.IEnumerator CicloDeViento()
    {
        while (true)
        {
            float tiempoEspera = Random.Range(minTiempoEntreVientos, maxTiempoEntreVientos);
            yield return new WaitForSeconds(tiempoEspera);

            int dir = Random.Range(0, 2) == 0 ? -1 : 1;
            ActualizarVientoEnRed(dir);

            yield return new WaitForSeconds(duracionViento);

            ActualizarVientoEnRed(0);
        }
    }

    void ActualizarVientoEnRed(int direccion)
    {
        if (PhotonNetwork.InRoom)
        {
            Hashtable hash = new Hashtable();
            hash.Add("WindDir", direccion);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("WindDir"))
        {
            int dir = (int)propertiesThatChanged["WindDir"];
            direccionVientoActual = dir;

            bool soyJ1 = PhotonNetwork.IsMasterClient;
            GameObject miPanel = soyJ1 ? panelVientoJ1 : panelVientoJ2;
            TextMeshProUGUI miTexto = soyJ1 ? textoDireccionJ1 : textoDireccionJ2;

            if (dir == 0)
            {
                if (miPanel != null) miPanel.SetActive(false);
                if (miTexto != null) miTexto.text = "";
            }
            else
            {
                if (miPanel != null) miPanel.SetActive(true);
                if (miTexto != null)
                {
                    if (dir == 1)
                    {
                        miTexto.text = "VIENTO: >>>";
                        miTexto.color = Color.red; 
                    }
                    else
                    {
                        miTexto.text = "<<< :VIENTO";
                        miTexto.color = Color.blue;
                    }
                }
            }
        }
    }
}
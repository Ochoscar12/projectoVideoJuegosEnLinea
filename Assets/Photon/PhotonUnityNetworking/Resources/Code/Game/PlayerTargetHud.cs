using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerTargetHud : MonoBehaviourPunCallbacks
{
    [Header("UI Progreso - Jugador 1 (Izquierda)")]
    public GameObject contenedorProgresoJ1;
    public RectTransform progressBarBgJ1;
    public RectTransform heightIndicatorJ1; 
    public RectTransform puntoMinimoJ1; 
    public RectTransform puntoMaximoJ1;
    public RectTransform opponentTargetJ1;  
    public Image opponentTargetImageJ1;     

    [Header("UI Progreso - Jugador 2 (Derecha)")]
    public GameObject contenedorProgresoJ2; 
    public RectTransform progressBarBgJ2;
    public RectTransform heightIndicatorJ2; 
    public RectTransform puntoMinimoJ2; 
    public RectTransform puntoMaximoJ2;
    public RectTransform opponentTargetJ2;  
    public Image opponentTargetImageJ2;  

    [Header("Configuración del Nivel")]
    public float alturaTotalDelNivel = 100f;

    [Header("Configuración del Radar (Ataque)")]
    public float distanciaRadar = 15f; 
    public float attackVerticalRange = 2f; 
    public Color targetMatchColor = Color.cyan; 

    [Header("UI Fin de Partida - Jugador 1 (Izquierda)")]
    public GameObject panelVictoriaJ1;
    public TMPro.TextMeshProUGUI textoNombreJ1;
    public TMPro.TextMeshProUGUI textoPuntajeJ1;
    public TMPro.TextMeshProUGUI textoPosicionJ1;
    public TMPro.TextMeshProUGUI textoResultadoFinalJ1;

    [Header("UI Fin de Partida - Jugador 2 (Derecha)")]
    public GameObject panelVictoriaJ2;
    public TMPro.TextMeshProUGUI textoNombreJ2;
    public TMPro.TextMeshProUGUI textoPuntajeJ2;
    public TMPro.TextMeshProUGUI textoPosicionJ2;
    public TMPro.TextMeshProUGUI textoResultadoFinalJ2;

    [Header("Sistema de Puntos y Red")]
    private bool miJuegoTerminado = false;    
    
    // Variables estáticas para sincronizar el estado global entre ambos clientes
    private static int jugadoresQueLlegaron = 0;
    private static int puntajeFinalJ1 = 0;
    private static int puntajeFinalJ2 = 0;

    private PlayerMovement localPlayer;
    private PlayerMovement opponentPlayer;
    private Color originalTargetColor;

    bool canMove = true;

    void Start()
    {
        // Apagar la barra del oponente dependiendo de quién soy en la red
        if (PhotonNetwork.IsMasterClient)
        {
            // Soy el Jugador 1
            if (contenedorProgresoJ1 != null) contenedorProgresoJ1.SetActive(true);
            if (contenedorProgresoJ2 != null) contenedorProgresoJ2.SetActive(false);
            if (opponentTargetImageJ1 != null) originalTargetColor = opponentTargetImageJ1.color;
        }
        else
        {
            // Soy el Jugador 2
            if (contenedorProgresoJ1 != null) contenedorProgresoJ1.SetActive(false);
            if (contenedorProgresoJ2 != null) contenedorProgresoJ2.SetActive(true);
            if (opponentTargetImageJ2 != null) originalTargetColor = opponentTargetImageJ2.color;
        }
    }

    void Update()
    {
        if (localPlayer == null || opponentPlayer == null)
        {
            FindPlayersInScene();
            if (localPlayer == null || opponentPlayer == null) return;
        }

        UpdateHeightIndicator();
        UpdateOpponentTarget();
        CheckAttackProximity();
    }

    [PunRPC]
    void RPC_JugadorLlegoPantallaDividida(string nombreJugador, int puntajeBase, bool esJugador1)
    {
        jugadoresQueLlegaron++;
        
        int puntajeCalculado = puntajeBase;
        string textoPosicion = "";

        // Calcular el bonus dependiendo de quién llegó primero
        if (jugadoresQueLlegaron == 1)
        {
            puntajeCalculado += 100; // Bonus de 100 al primero
            textoPosicion = "Llegada: 1er Lugar (+100 Bonus)";
        }
        else
        {
            textoPosicion = "Llegada: 2do Lugar (Sin Bonus)";
        }

        // Guardar el puntaje final en la variable correspondiente para comparar luego
        if (esJugador1) puntajeFinalJ1 = puntajeCalculado;
        else puntajeFinalJ2 = puntajeCalculado;

        // === ENCENDER EL PANEL DEL LADO CORRECTO ===
        if (esJugador1)
        {
            if (panelVictoriaJ1 != null) panelVictoriaJ1.SetActive(true);
            if (textoNombreJ1 != null) textoNombreJ1.text = "Jugador: " + nombreJugador;
            if (textoPuntajeJ1 != null) textoPuntajeJ1.text = "Puntaje Final: " + puntajeCalculado.ToString();
            if (textoPosicionJ1 != null) textoPosicionJ1.text = textoPosicion;
            if (textoResultadoFinalJ1 != null)
            {
                textoResultadoFinalJ1.text = "Esperando al otro jugador...";
                textoResultadoFinalJ1.color = Color.white;
            }
        }
        else
        {
            if (panelVictoriaJ2 != null) panelVictoriaJ2.SetActive(true);
            if (textoNombreJ2 != null) textoNombreJ2.text = "Jugador: " + nombreJugador;
            if (textoPuntajeJ2 != null) textoPuntajeJ2.text = "Puntaje Final: " + puntajeCalculado.ToString();
            if (textoPosicionJ2 != null) textoPosicionJ2.text = textoPosicion;
            if (textoResultadoFinalJ2 != null)
            {
                textoResultadoFinalJ2.text = "Esperando al otro jugador...";
                textoResultadoFinalJ2.color = Color.white;
            }
        }

        // === CUANDO LLEGAN LOS DOS: DECIDIR GANADOR ===
        if (jugadoresQueLlegaron == 2)
        {
            if (puntajeFinalJ1 > puntajeFinalJ2)
            {
                // Gana J1
                if (textoResultadoFinalJ1 != null) { textoResultadoFinalJ1.text = "¡HAS GANADO!"; textoResultadoFinalJ1.color = Color.green; }
                if (textoResultadoFinalJ2 != null) { textoResultadoFinalJ2.text = "HAS PERDIDO"; textoResultadoFinalJ2.color = Color.red; }
            }
            else if (puntajeFinalJ2 > puntajeFinalJ1)
            {
                // Gana J2
                if (textoResultadoFinalJ2 != null) { textoResultadoFinalJ2.text = "¡HAS GANADO!"; textoResultadoFinalJ2.color = Color.green; }
                if (textoResultadoFinalJ1 != null) { textoResultadoFinalJ1.text = "HAS PERDIDO"; textoResultadoFinalJ1.color = Color.red; }
            }
            else 
            {
                // Empate
                if (textoResultadoFinalJ1 != null) { textoResultadoFinalJ1.text = "¡ES UN EMPATE!"; textoResultadoFinalJ1.color = Color.yellow; }
                if (textoResultadoFinalJ2 != null) { textoResultadoFinalJ2.text = "¡ES UN EMPATE!"; textoResultadoFinalJ2.color = Color.yellow; }
            }
        }
    }

    void FindPlayersInScene()
    {
        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement p in allPlayers)
        {
            if (p.photonView.IsMine) localPlayer = p;
            else opponentPlayer = p;
        }
    }

    void UpdateHeightIndicator()
    {
        RectTransform activeHeightIndicator = PhotonNetwork.IsMasterClient ? heightIndicatorJ1 : heightIndicatorJ2;
        RectTransform activeMin = PhotonNetwork.IsMasterClient ? puntoMinimoJ1 : puntoMinimoJ2;
        RectTransform activeMax = PhotonNetwork.IsMasterClient ? puntoMaximoJ1 : puntoMaximoJ2;

        if (activeHeightIndicator == null || activeMin == null || activeMax == null) return;

        float currentHeight = localPlayer.totalClimbedDistance; 
        float normalizedHeight = Mathf.Clamp01(currentHeight / alturaTotalDelNivel);

        float targetY = Mathf.Lerp(activeMin.anchoredPosition.y, activeMax.anchoredPosition.y, normalizedHeight);
        
        activeHeightIndicator.anchoredPosition = new Vector2(activeHeightIndicator.anchoredPosition.x, targetY);

        if (currentHeight >= alturaTotalDelNivel && !miJuegoTerminado)
        {
            miJuegoTerminado = true;
            localPlayer.enabled = false; 

            int misPuntosFinales = 0;
            if (PlayerStats.LocalInstance != null)
            {
                misPuntosFinales = PlayerStats.LocalInstance.score; 
            }

            bool soyJugador1 = PhotonNetwork.IsMasterClient;
            photonView.RPC("RPC_JugadorLlegoPantallaDividida", RpcTarget.All, PhotonNetwork.NickName, misPuntosFinales, soyJugador1);
        }
    }

    void UpdateOpponentTarget()
    {
        RectTransform activeProgressBarBg = PhotonNetwork.IsMasterClient ? progressBarBgJ1 : progressBarBgJ2;
        RectTransform activeOpponentTarget = PhotonNetwork.IsMasterClient ? opponentTargetJ1 : opponentTargetJ2;
        Image activeOpponentTargetImage = PhotonNetwork.IsMasterClient ? opponentTargetImageJ1 : opponentTargetImageJ2;

        if (activeProgressBarBg == null || activeOpponentTarget == null) return;

        float heightDifference = opponentPlayer.totalClimbedDistance - localPlayer.totalClimbedDistance;
        
        float normalizedDiff = Mathf.Clamp(heightDifference / distanciaRadar, -1f, 1f); 
        float barHeight = activeProgressBarBg.rect.height;
        
        float targetY = normalizedDiff * (barHeight / 2f); 
        activeOpponentTarget.anchoredPosition = new Vector2(activeOpponentTarget.anchoredPosition.x, targetY);

        if (activeOpponentTargetImage != null)
        {
            Color c = activeOpponentTargetImage.color;
            c.a = 0.3f + (Mathf.Sin(Time.time * 5f) + 1f) * 0.3f;
            activeOpponentTargetImage.color = c;
        }
    }

    void CheckAttackProximity()
    {
        Image activeOpponentTargetImage = PhotonNetwork.IsMasterClient ? opponentTargetImageJ1 : opponentTargetImageJ2;
        
        float heightDifference = Mathf.Abs(opponentPlayer.totalClimbedDistance - localPlayer.totalClimbedDistance);

        if (activeOpponentTargetImage != null)
        {
            if (heightDifference <= attackVerticalRange)
            {
                activeOpponentTargetImage.color = targetMatchColor; 
            }
            else
            {
                activeOpponentTargetImage.color = originalTargetColor; 
            }
        }
    }
}
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyPlayerSync : MonoBehaviourPunCallbacks
{
    [Header("Modelos 3D del Oponente (Lado Derecho)")]
    public GameObject[] remoteAvatar3DModels; 

    [Header("UI del Oponente")]
    public GameObject waitingText; 

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            waitingText.SetActive(false);
            
            foreach (Player p in PhotonNetwork.PlayerListOthers)
            {
                int remoteAvatarIndex = 0;
                if (p.CustomProperties.ContainsKey("AvatarIndex"))
                {
                    remoteAvatarIndex = (int)p.CustomProperties["AvatarIndex"];
                }
                UpdateRemoteAvatarVisibility(remoteAvatarIndex);
                break; 
            }
        }
        else
        {
            waitingText.SetActive(true);
            UpdateRemoteAvatarVisibility(-1); 
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        waitingText.SetActive(false);
        
        int remoteAvatarIndex = 0; 
        if (newPlayer.CustomProperties.ContainsKey("AvatarIndex"))
        {
            remoteAvatarIndex = (int)newPlayer.CustomProperties["AvatarIndex"];
        }
        
        UpdateRemoteAvatarVisibility(remoteAvatarIndex, true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRemoteAvatarVisibility(-1);
        waitingText.SetActive(true);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!targetPlayer.IsLocal && changedProps.ContainsKey("AvatarIndex"))
        {
            int newAvatarIndex = (int)changedProps["AvatarIndex"];
            
            UpdateRemoteAvatarVisibility(newAvatarIndex, true);
        }
    }
    
    private void UpdateRemoteAvatarVisibility(int activeIndex, bool reproducirBaile = false)
    {
        for (int i = 0; i < remoteAvatar3DModels.Length; i++)
        {
            if (remoteAvatar3DModels[i] != null)
            {
                bool esElSeleccionado = (i == activeIndex);
                remoteAvatar3DModels[i].SetActive(esElSeleccionado);

                if (esElSeleccionado && reproducirBaile && activeIndex != -1)
                {
                    Animator anim = remoteAvatar3DModels[i].GetComponent<Animator>();
                    if (anim == null) anim = remoteAvatar3DModels[i].GetComponentInChildren<Animator>();

                    if (anim != null)
                    {
                        StartCoroutine(RutinaBaileOponente(anim));
                    }
                }
            }
        }
    }

    private System.Collections.IEnumerator RutinaBaileOponente(Animator anim)
    {
        anim.SetBool("bailando", true);
        yield return new WaitForSeconds(5f);
        if (anim != null) anim.SetBool("bailando", false);
    }
}
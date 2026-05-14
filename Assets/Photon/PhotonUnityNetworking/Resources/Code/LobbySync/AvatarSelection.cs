using UnityEngine;
using Photon.Pun;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AvatarSelection : MonoBehaviour
{
    [Header("Elementos de la UI")]
    public GameObject avatarSelectionPanel; 
    
    [Header("Modelos 3D en la Escena")]
    public GameObject[] avatar3DModels; 

    private int currentAvatarIndex = 0;
    private Coroutine rutinaBaileActual; 

    void Start()
    {
        Update3DModelVisibility(0, false);
    }

    public void OpenSelectionPanel()
    {
        avatarSelectionPanel.SetActive(true);
    }

    public void ToggleSelectionPanel()
    {
        bool isActive = avatarSelectionPanel.activeSelf;
        avatarSelectionPanel.SetActive(!isActive);
    }

    public void CloseSelectionPanel()
    {
        avatarSelectionPanel.SetActive(false);
    }

    public void SelectAvatar(int index)
    {
        currentAvatarIndex = index;
        
        Update3DModelVisibility(index, true);

        SyncAvatarWithPhoton();
        CloseSelectionPanel();
    }

    private void Update3DModelVisibility(int activeIndex, bool reproducirBaile)
    {
        for (int i = 0; i < avatar3DModels.Length; i++)
        {
            if (avatar3DModels[i] != null)
            {
                bool esElSeleccionado = (i == activeIndex);
                avatar3DModels[i].SetActive(esElSeleccionado);

                if (esElSeleccionado && reproducirBaile)
                {
                    HacerBailar(avatar3DModels[i]);
                }
            }
        }
    }

    public void ActivarBaile()
    {
        if (avatar3DModels[currentAvatarIndex] != null)
        {
            HacerBailar(avatar3DModels[currentAvatarIndex]);
        }
    }

    private void HacerBailar(GameObject avatar)
    {
        Animator anim = avatar.GetComponent<Animator>();
        if (anim == null) anim = avatar.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            if (rutinaBaileActual != null) StopCoroutine(rutinaBaileActual);
            rutinaBaileActual = StartCoroutine(RutinaBaile(anim));
        }
    }

    private IEnumerator RutinaBaile(Animator anim)
    {
        anim.SetBool("bailando", true);
        yield return new WaitForSeconds(5f);
        if (anim != null) anim.SetBool("bailando", false);
    }

    private void SyncAvatarWithPhoton()
    {
        Hashtable playerProperties = new Hashtable();
        playerProperties["AvatarIndex"] = currentAvatarIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        
        Debug.Log("Avatar elegido y sincronizado: " + currentAvatarIndex);
    }
}
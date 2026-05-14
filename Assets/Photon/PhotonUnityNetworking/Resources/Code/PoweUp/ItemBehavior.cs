using UnityEngine;
using Photon.Pun;

public class ItemBehavior : MonoBehaviourPun
{
    [Header("Configuración del Item")]
    public TipoItem tipoDeItem; 
        public float fallSpeed = 2.5f; 
    public float destroyYLimit = -5f; 
    
    private bool yaRecogido = false;

    void Update()
    {
        // movimiento hacia abajo
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        if (photonView.IsMine && transform.position.y < destroyYLimit)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (yaRecogido) return; 

        PlayerInventory inventario = other.GetComponentInParent<PlayerInventory>();
        
        if (inventario != null)
        {
            if (inventario.photonView.IsMine)
            {
                // lo pasamos al invenatario
                inventario.RecogerItem(tipoDeItem);
                
                // destruimos el obbjeto
                photonView.RPC("DestruirItemRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void DestruirItemRPC()
    {
        yaRecogido = true; 
        
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
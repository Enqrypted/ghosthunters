using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CoinsFunctionality : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player plr in PhotonNetwork.PlayerList)
            {

                if (plr.NickName == collision.gameObject.name)
                {
                    GameObject.Find("ClientController").GetComponent<ClientController>().PlayerGotCoin(plr);
                }
            }

            PhotonView.Get(this).RPC("DeleteCoin", RpcTarget.All);
        }
    }

    [PunRPC]
    public void DeleteCoin()
    {
        Destroy(this.gameObject);
    }
}

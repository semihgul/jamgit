
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Photon.Pun;
using Photon.Realtime;
public class TeamMatcher : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        // Photon kullanarak iki farklı renk atama
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("MatchTeam", RpcTarget.AllBuffered, 0); // MasterClient'e bir renk atar
        }
        else
        {
            photonView.RPC("MatchTeam", RpcTarget.AllBuffered, 1); // Diğer oyuncuya farklı bir renk atar
        }

    }
    [PunRPC]
    void MatchTeam(int oyuncuRenk)
    {
        // Burada oyuncuRenk'e göre renk ataması yapabilirsiniz
        Renderer renderer = GetComponent<Renderer>();

        if (oyuncuRenk == 0)
        {
            renderer.material.color = Color.red;
        }
        else if (oyuncuRenk == 1)
        {
            renderer.material.color = Color.blue;
        }
    }
}

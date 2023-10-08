using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using Photon.Realtime;
public class Interactable : MonoBehaviourPunCallbacks
{
    public TeamManager.Team requiredTeam;
    public PhotonView PV;


    private void OnMouseDown()
    {
        Debug.Log("allahyok");
        if (requiredTeam == TeamManager.Team.blueTeam && PV.IsMine)

        {
            Debug.Log("You are blue");
        }
        else if(requiredTeam == TeamManager.Team.redTeam && PV.IsMine)
            Debug.Log("You are red or gay");
    }
}

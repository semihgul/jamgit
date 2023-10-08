using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Photon.Realtime;

using Photon.Pun;
using UnityEngine.SceneManagement;


public class S_GameManager : MonoBehaviourPunCallbacks
{
    
    public Player[] players;
    public Transform team1Pos;
    public Transform team2Pos;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("loby");
            return;
        }
        else
            GetPlayers();

    }
    public void GetPlayers()
    {

        players = PhotonNetwork.PlayerList; 

        GameObject pl1 = PhotonNetwork.Instantiate("Player", team1Pos.position, Quaternion.identity);
        pl1.GetComponent<TeamManager>().SetPlayer(players[0],TeamManager.Team.blueTeam);

        GameObject pl2 = PhotonNetwork.Instantiate("Player", team2Pos.position, Quaternion.identity);
        pl2.GetComponent<TeamManager>().SetPlayer(players[1],TeamManager.Team.redTeam);






    }
}

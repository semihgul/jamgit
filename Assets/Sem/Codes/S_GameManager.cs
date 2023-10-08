 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Photon.Realtime;

using Photon.Pun;
using UnityEngine.SceneManagement;


public class S_GameManager : MonoBehaviourPunCallbacks
{

    public GameObject Team1;
    public GameObject Team2;


    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("loby");
            return;
        }
        if (Team1 == null || Team2 == null)
        {
            Debug.LogError("Gama manager can not find the team prefabs");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "is enter the game");
        if (PhotonNetwork.IsMasterClient)
        {
            Team1.GetComponent<TeamManager>().SetPlayer(newPlayer);
        }
        else
        {
            Team2.GetComponent<TeamManager>().SetPlayer(newPlayer);
        }

    }
}

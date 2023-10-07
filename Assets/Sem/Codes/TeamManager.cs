using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class TeamManager : MonoBehaviourPunCallbacks
{
    public void SetPlayer(Player player)
    {
        Debug.Log("Mark the player " + player.NickName);
    }
}

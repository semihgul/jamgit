using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class TeamManager : MonoBehaviourPunCallbacks
{
    public enum Team
    {
        blueTeam,
        redTeam
    }
    public Team PlayersTeam;


    public void SetPlayer(Player player, Team team)
    {
        if (team == Team.blueTeam)
        {
            PlayersTeam = Team.blueTeam;
            Debug.Log(player.NickName + " 's team is blue");
        }
        else
        {
            PlayersTeam = Team.redTeam;
            Debug.Log(player.NickName + " 's team is red");
        }

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using TMPro;

public class RoomSlotTest : MonoBehaviourPunCallbacks
{
    public string roomName;
    public TMP_Text roomNameText;
    public TMP_Text statuText;
    [SerializeField] private PhotonView pv;


    void OnEnable()
    {
        EvntManager.StartListening<int>("Increase" + roomName, WriteText);
    }
    void OnDisable()
    {
        EvntManager.StartListening<int>("Increase" + roomName, WriteText);
    }
    public void WriteText(int count)
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + " " + playerCount + "/2";
        StartCoroutine(checkPlayers());
    }

    IEnumerator checkPlayers()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                roomNameText.text = PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + "/2";
                statuText.text = "Waiting for server. Game will start in 3 seconds";
                Invoke(nameof(LoadBabLevel),3);
                break;
            }
        }
    }

    public void LoadBabLevel()
    {
        PhotonNetwork.LoadLevel("mp_area_test");

    }

}
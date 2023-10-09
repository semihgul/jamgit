using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using System;
using UnityEngine.UI;
public class ss_loby : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte maxPlayers = 2;
    [SerializeField] private TMP_InputField InputFieldName;

    [SerializeField] private TMP_Text StatuText;

    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;


    void Start() => ConnectToPhoton();
    private void ConnectToPhoton() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        StatuText.text = "You are on the loby you can join a room now";
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");
        Debug.Log("Room name: " + PhotonNetwork.CurrentRoom.Name + " bağlandı");
        //refresh list butonu aktif edilecek
        string roomName = PhotonNetwork.CurrentRoom.Name;
        int roomPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        EvntManager.TriggerEvent("Increase"+roomName, roomPlayerCount);
        Debug.Log(roomName + " " + roomPlayerCount);
    }


    public void ChangeName()
    {
        PhotonNetwork.NickName = InputFieldName.text;
        Debug.Log("Your new name is " + PhotonNetwork.NickName);
    }


    public void ConnectRoom(string roomName)
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new Photon.Realtime.RoomOptions { MaxPlayers = maxPlayers }, null, null);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Realtime;
using Photon.Pun;


public class S_Laucher : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte maxPlayers = 4;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private TMP_InputField InputFieldName;
    [SerializeField] private TMP_InputField InputFieldRoomName;

    private bool isConnecting;
    RoomNames _roomNames;

    public TMP_Text roomNameText;
    public TMP_Text roomPlayersText;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        controlPanel.SetActive(true);
        Debug.Log("Esenlikler");
    }
    public string GetRandomName()
    {
        _roomNames = (RoomNames)Random.Range(0, System.Enum.GetNames(typeof(RoomNames)).Length);
        return _roomNames.ToString();
    }
    public void ChangeName()
    {
        PhotonNetwork.NickName = InputFieldName.text;
        Debug.Log("Your new name is " + PhotonNetwork.NickName);
    }

    public void Connect()
    {
        isConnecting = true;
        controlPanel.SetActive(false);


        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = "1.0";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void ConnectWithName()
    {
        isConnecting = true;
        controlPanel.SetActive(false);

        string roomName = InputFieldRoomName.text;
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            PhotonNetwork.GameVersion = "1.0";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        controlPanel.SetActive(true);
        Debug.Log("baglnati kesildi");
        isConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("There is no room like that. Creating one");

        PhotonNetwork.CreateRoom(GetRandomName(), new Photon.Realtime.RoomOptions { MaxPlayers = maxPlayers });

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");
        Debug.Log("Room name: " + PhotonNetwork.CurrentRoom.Name + " baglandi");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomPlayersText.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount;
        StartCoroutine(CheckGame());
        readyPanel.SetActive(true);
    }


    private IEnumerator CheckGame()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                roomPlayersText.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount;
                PhotonNetwork.LoadLevel("mp_area_test");

                break;

            }
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string roomName = "player1's lobby";

    IEnumerator AutoRefreshLobby()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            print("joining lobby..");
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }

        yield return new WaitForSeconds(5f);

        StartCoroutine(AutoRefreshLobby());
    }

    IEnumerator CreateRoom()
    {
        yield return new WaitForSeconds(1f);

        print(PhotonNetwork.IsConnectedAndReady);
        print(PhotonNetwork.IsConnected);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            print("creating room..");
            PhotonNetwork.CreateRoom(roomName, new RoomOptions(), TypedLobby.Default);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        StartCoroutine(AutoRefreshLobby());
    }

    public override void OnJoinedLobby()
    {
        print("joined lobby");
    }

    public override void OnConnectedToMaster()
    {
        print("joined master server");
        StartCoroutine(CreateRoom());
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print(roomList.Count);
        foreach (RoomInfo roomInfo in roomList)
        {
            print(roomInfo.Name);
        }
    }
}

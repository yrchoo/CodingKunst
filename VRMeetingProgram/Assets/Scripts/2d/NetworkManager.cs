//using System.Collections;
/*using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Hashtable = ExitGames.Client.Photon.Hashtable;*/

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    //ChatManager chatManager;
    //public GameObject CM;
    public ChatManager CM;
    public ChangeScene CS;

    static public string myID;
    public string UserName;
    public string ID;
    bool isLoaded;

    [Header("SideBar")]
    public GameObject SideBar;
    public InputField NickNameInput;

    [Header("Lobby")]
    public GameObject LobbyPanel; 
    public Text WelcomeText;
    public Text LobbyInfoText;
    List<RoomInfo> myList = new List<RoomInfo>();
    public Button[] CellBtn;
    //public Button PreviousBtn;
    //public Button NextBtn;

    [Header("CreatePanel")]
    public InputField RoomInput;
    public InputField RoomNum;

    [Header("Chat")]
    public GameObject Chat;
    //public GameObject ChatPrefab;
    //public GameObject ChatPos;

    public Text ListText;
    public Text RoomInfoText;
    //public Text ChatText;
    public InputField ChatInput;

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;
    //public Text NickNameText;
    //public InputField ChatInput;

    [Header("Disconnect")]
    public PlayerLeaderboardEntry MyPlayFabInfo; //내 정보 다 들어감
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();


    //int currentPage = 1, maxPage, multiple;

    #region 서버연결
    void Awake()
    {
        myID = CS.Load("userId");
        GetMyName(myID);

        PhotonNetwork.ConnectUsingSettings();

        //Screen.SetResolution(960, 540, false);

        /*PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;*/

        //NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        //NickNameText.color = PV.IsMine ? Color.green : Color.red;
        //CM = GameObject.Find("ChatManager");
    }

    void Update()
    {
        //Debug.Log(PV.IsMine);
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = PhotonNetwork.CountOfPlayers + "접속";

        /*if(Chat.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Return)){
                NMSend();
            }
        }*/

        if(PhotonNetwork.InRoom && Input.GetKeyDown(KeyCode.Return))
        {
            
            ChatInput.ActivateInputField();
            ChatInput.Select();
        }

        
    }

    //public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby(); //connect의 콜백

    public override void OnJoinedLobby()
    {
        
        //ShowPanel(LobbyPanel);

        if (isLoaded)
        {
            ShowPanel(LobbyPanel);
            //ShowUserNickName();
        }
        else Invoke("OnJoinedLobbyDelay", 1);

        //PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;
        //PhotonNetwork.LocalPlayer.NickName = UserName;
        //Debug.Log("here");
        
    }

    void OnJoinedLobbyDelay()
    {
        
        isLoaded = true;
        Debug.Log(UserName);
        //PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;
        PhotonNetwork.LocalPlayer.NickName = UserName;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
        ShowPanel(LobbyPanel);
        //ShowUserNickName();
    }

    /*void ShowUserNickName()
    {
        UserNickNameText.text = "";
        for (int i = 0; i < PlayFabUserList.Count; i++) UserNickNameText.text += PlayFabUserList[i].DisplayName + "\n";
    }*/

    /*public void Disconnect()
    {
        PhotonNetwork.Disconnect();      
    }*/

    public override void OnDisconnected(DisconnectCause cause) //disconnect콜백
    {
        isLoaded = false;
        ShowPanel(SideBar);

    }
    #endregion

    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        //if (num == -2) --currentPage;
        //else if (num == -1) ++currentPage;
        //else
        PhotonNetwork.JoinRoom(myList[num].Name); //onjoinedroom 호출됨
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        //maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        //PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        //NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        //multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (i < myList.Count) ? myList[i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (i < myList.Count) ? myList[i].PlayerCount + "/" + myList[i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        //Debug.Log(roomCount);
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion


    #region 방
    public void CreateConfirmBtn()
    {
        
        if(RoomInput.text != "" && RoomNum.text != "")
        {
           
            byte numByte = byte.Parse(RoomNum.text);            
            PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = numByte });
        }
          //성공적으로 만들어지면 onJoinedRoom으로
        RoomInput.text = "";
        RoomNum.text = "";
        //RoomRenewal();
    }

    //public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    /*public void LeaveRoom() => PhotonNetwork.LeaveRoom();*/


    public override void OnJoinedRoom()
    {
        Chat.SetActive(true);
        LobbyPanel.SetActive(true);

        //Destroy(ChatPos);
        //ShowPanel(Chat);

        //Instantiate(ChatPrefab, ChatPos);

        RoomRenewal();
        ChatInput.text = "";

        //여기서 채팅 방도 초기화
        CM.clean();
        //파베에 저장된 대화 내용까지 불러오기

        //for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
        //MyListRenewal();
        //SideBar2.SetActive(true);
        //PhotonNetwork.JoinLobby();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateConfirmBtn(); }

    //public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    //player가 방에 있을 때 호출
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal(); //사람이 들어왔다 나갔다 할 때 방 갱신
        InformRPC(newPlayer.NickName + "님이 입장하셨습니다");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        InformRPC(otherPlayer.NickName + "님이 퇴장하셨습니다");
    }

    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        //여기 수정!!!!!
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / 접속 중 : " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }
    #endregion


#region 채팅
    
    public void BtnSend()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if ((PhotonNetwork.PlayerList[i].NickName).Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                //Debug.Log("??");
                //CM.GetComponent<ChatManager>().Send(ChatInput.text);
                CM.Send(ChatInput.text);
            }
        }
        PV.RPC("ChatRPC", RpcTarget.Others, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    public void TFSend()
    {
        
        //Debug.Log(ChatInput.text);
        //CM = GameObject.Find("ChatManager");
        if(Input.GetKeyDown(KeyCode.Return) && ChatInput.text != "")
        {
            //Debug.Log("??");
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if ((PhotonNetwork.PlayerList[i].NickName).Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    //Debug.Log("??");
                    //CM.GetComponent<ChatManager>().Send(ChatInput.text);
                    CM.Send(ChatInput.text);
                }
            }
            PV.RPC("ChatRPC", RpcTarget.Others, PhotonNetwork.NickName + " : " + ChatInput.text);
            ChatInput.text = "";
        }
        
        //PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);

        //ChatRPC(ChatInput.text);
        
    }

    //public void 

    void InformRPC(string msg)
    {

        //CM.GetComponent<ChatManager>().Inform(msg);
        CM.Inform(msg);
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        //CM.GetComponent<ChatManager>().ChatTo(msg);
        CM.ChatTo(msg);
    }
    #endregion

    #region 기타
    /*public override void OnDisconnected(DisconnectCause cause)
    {
        ShowPanel(DisconnectPanel);
    }*/

    public void XBtn()
    {
        if (PhotonNetwork.InLobby) PhotonNetwork.Disconnect(); //ondisconnected
        else if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
    }
    #endregion

    void ShowPanel(GameObject CurPanel)
    {
        SideBar.SetActive(false);
        LobbyPanel.SetActive(false);
        Chat.SetActive(false);

        CurPanel.SetActive(true);
    }

    public void GetMyName(string myID)
    {
        var request = new GetUserDataRequest() { PlayFabId = myID };
        PlayFabClientAPI.GetUserData(request, (result) => {

            ID = result.Data["id"].Value;
            UserName = result.Data["name"].Value;

        },
            (error) => print("데이터 불러오기 실패")
            );
    }



    /* #region set get
     void SetRoomTag(int slotIndex, int value) => PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { slotIndex.ToString(), value } });
     int GetRoomTag(int slotIndex) => (int)PhotonNetwork.CurrentRoom.CustomProperties[slotIndex.ToString()];

     Player GetPlayer(int slotIndex)
     {
         for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
             if (PhotonNetwork.PlayerList[i].ActorNumber == GetRoomTag(slotIndex)) return PhotonNetwork.PlayerList[i];
         return null;
     }

     void setLocalTag(string key, bool value) => PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { key, value } });
     bool GetLocalTag(string key) => (bool)PhotonNetwork.LocalPlayer.CustomProperties[key];

     bool isMaster() => PhotonNetwork.LocalPlayer.IsMasterClient;*/

    /*void SetItemTag()
    {
        Item curCharacter = MyItemList.Find(x => x.Type == "Character" && x.isUsing == true);
        Item curBalloon = MyItemList.Find(x => x.Type == "Balloon" && x.isUsing == true);

        //PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Character", curCharacter.Name }, { "Balloon", curBalloon != null ? } })
    }*/
}

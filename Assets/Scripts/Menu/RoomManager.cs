using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityFigmaBridge.Runtime.UI;
using static UnityEngine.GraphicsBuffer;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public string mode;
    public FigmaImage Image2D;
    public FigmaImage Image3D;
    public int Scene2DID;
    public int Scene3DID;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 140;
    }

    private void Start()
    {
        Debug.Log("Connecting.");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 999);
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (Application.targetFrameRate != 140)
            Application.targetFrameRate = 140;   
    }

    public void Join(TMP_InputField roomCodeInput)
    {
        if(roomCodeInput != null)
            if(roomCodeInput.text != "")
                PhotonNetwork.JoinRoom(roomCodeInput.text);
    }

    public void CreateRoom()
    {
        if (mode != "")
        {
            RoomOptions roomOptions = new RoomOptions();
            ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
            prop.Add("mode", mode);
            roomOptions.CustomRoomProperties = prop;
            PhotonNetwork.CreateRoom(GenerateSixDigitCode(), roomOptions);
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to master server.");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        CrossData.mode = mode;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        string mode = (string)PhotonNetwork.CurrentRoom.CustomProperties["mode"];
        CrossData.mode = mode;
        PhotonNetwork.LoadLevel(mode == "2D" ? Scene2DID : Scene3DID);
    }

    public void Select2DMode()
    {
        mode = "2D";
        Image2D.StrokeWidth = 10;
        Image3D.StrokeWidth = 0;
    }

    public void Select3DMode()
    {
        mode = "3D";
        Image2D.StrokeWidth = 0;
        Image3D.StrokeWidth = 10;
    }

    public static string GenerateSixDigitCode()
    {
        string code = "";

        for (int i = 0; i < 6; i++)
        {
            int digit = Random.Range(0, 10);
            code += digit.ToString();
        }

        return code;
    }
}

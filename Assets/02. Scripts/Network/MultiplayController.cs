using System;
using Newtonsoft.Json;
using SocketIOClient;
using UnityEngine;

// joinRoom/createRoom 이벤트 전달할 때 전달되는 정보의 타입
public class RoomData
{
    [JsonProperty("roomId")]
    public string roomId { get; set; }
}

// 상대방이 둔 마커 위치
public class BlockData
{
    [JsonProperty("blockIndex")]
    public int blockIndex { get; set; }
}

public class MultiplayController
{
    private SocketIOUnity _socket;



    public MultiplayController()
    {
        var uri = new Uri(Constants.SocketServerURL);
        _socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        _socket.On("createRoom", CreateRoom);
        _socket.On("joinRoom", JoinRoom);
        _socket.On("startGame", StartGame);
        _socket.On("exitGame", ExitGame);
        _socket.On("endGame", EndGame);
        _socket.On("doOpponent", DoOpponent);
    }

    private void CreateRoom(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();

    }

    private void JoinRoom(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
    }

    private void StartGame(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();

    }

    private void ExitGame(SocketIOResponse response)
    {
    }

    private void EndGame(SocketIOResponse response)
    {
    }

    private void DoOpponent(SocketIOResponse response)
    {
        var data = response.GetValue<BlockData>();
    }
}
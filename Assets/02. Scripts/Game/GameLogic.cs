
using System;
using UnityEngine;

public class GameLogic : IDisposable
{
    public BlockController blockController;         // Block을 처리할 객체

    private Constants.PlayerType[,] _board;         // 보드의 상태 정보

    public BasePlayerState firstPlayerState;        // Player A
    public BasePlayerState secondPlayerState;       // Player B

    public enum GameResult { None, Win, Lose, Draw }

    private BasePlayerState _currentPlayerState;    // 현재 턴의 Player

    private MultiplayController _multiplayController; // 멀티플레이 기능
    private string _roomId;

    public GameLogic(BlockController blockController, Constants.GameType gameType)
    {
        this.blockController = blockController;

        // 보드의 상태 정보 초기화
        _board =
            new Constants.PlayerType[Constants.BlockColumnCount, Constants.BlockColumnCount];

        // Game Type 초기화
        switch (gameType)
        {
            case Constants.GameType.SinglePlay:
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new AIState();
                // 게임 시작
                SetState(firstPlayerState);
                break;
            case Constants.GameType.DualPlay:
                firstPlayerState = new PlayerState(true);
                secondPlayerState = new PlayerState(false);
                // 게임 시작
                SetState(firstPlayerState);
                break;
            case Constants.GameType.MultiPlay:
                _multiplayController = new MultiplayController((state, roomId) =>
                {
                    _roomId = roomId;
                    switch (state)
                    {
                        case Constants.MultiplayControllerState.CreateRoom:
                            Debug.Log("## Create Room ##");
                            // TODO : 대기 화면 UI 표시
                            break;
                        case Constants.MultiplayControllerState.JoinRoom:
                            Debug.Log("## Join Room ##");
                            firstPlayerState = new MultiPlayerState(true, _multiplayController);
                            secondPlayerState = new PlayerState(false, _multiplayController, _roomId);
                            SetState(firstPlayerState);
                            break;
                        case Constants.MultiplayControllerState.StartGame:
                            Debug.Log("## Start Game ##");
                            firstPlayerState = new PlayerState(true, _multiplayController, _roomId);
                            secondPlayerState = new MultiPlayerState(false, _multiplayController);
                            SetState(firstPlayerState);
                            break;
                        case Constants.MultiplayControllerState.ExitRoom:
                            Debug.Log("## Exit Room ##");
                            // TODO : 팝업 띄우고 Main 화면으로 이동
                            break;
                        case Constants.MultiplayControllerState.EndGame:
                            Debug.Log("## End Game ##");
                            // TODO : 팝업 띄우고 Main 화면으로 이동
                            break;
                    }
                });
                break;
        }
    }

    public Constants.PlayerType[,] GetBoard()
    {
        return _board;
    }

    // 턴이 바뀔 때, 기존 진행하던 상태를 Exit 하고
    // 이번 턴의 상태를 _currentPlayerState에 할당하고
    // 이번 턴의 상탱에 Enter 호출
    public void SetState(BasePlayerState state)
    {
        _currentPlayerState?.OnExit(this);
        _currentPlayerState = state;
        _currentPlayerState?.OnEnter(this);
    }

    // _board 배열에 새로운 Marker 값을 할당
    public bool SetNewBoardValue(Constants.PlayerType playerType,
        int row, int col)
    {
        if (_board[row, col] != Constants.PlayerType.None) return false;

        if (playerType == Constants.PlayerType.PlayerA)
        {
            _board[row, col] = playerType;
            blockController.PlaceMaker(Block.MarkerType.O, row, col);
            return true;
        }
        else if (playerType == Constants.PlayerType.PlayerB)
        {
            _board[row, col] = playerType;
            blockController.PlaceMaker(Block.MarkerType.X, row, col);
            return true;
        }
        return false;
    }

    // Game Over 처리
    public void EndGame(GameResult gameResult)
    {
        SetState(null);
        firstPlayerState = null;
        secondPlayerState = null;

        GameManager.Instance.OpenConfirmPanel("게임 오버", () =>
        {
            GameManager.Instance.ChangeToMainScene();
        });
    }

    // 게임의 결과 확인
    public GameResult CheckGameResult()
    {
        if (OmokAI.CheckGameWin(Constants.PlayerType.PlayerA, _board)) { return GameResult.Win; }
        if (OmokAI.CheckGameWin(Constants.PlayerType.PlayerB, _board)) { return GameResult.Lose; }
        if (OmokAI.CheckGameDraw(_board)) { return GameResult.Draw; }
        return GameResult.None;
    }

    public void Dispose()
    {
        _multiplayController?.LeaveRoom(_roomId);
        _multiplayController?.Dispose();
    }
}
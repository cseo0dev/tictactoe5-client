
using System.Diagnostics;

public class PlayerState : BasePlayerState
{
    private bool _isFirstPlayer;
    private Constants.PlayerType _playerType;

    private MultiplayController _multiplayController;
    private string _roomId;
    private bool _isMultiplay;

    public PlayerState(bool isFirstPlayer) // 싱글 플레이용 생성자
    {
        _isFirstPlayer = isFirstPlayer;
        _playerType = _isFirstPlayer ?
            Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;
        _isMultiplay = false;
    }

    public PlayerState(bool isFirstPlayer, MultiplayController multiplayController, string roomId) // 멀티 플레이용 생성자
        :this(isFirstPlayer)
    {
        // :this(isFirstPlayer)가 아래 주석문을 대신 함
        //_isFirstPlayer = isFirstPlayer;
        //_playerType = _isFirstPlayer ?
        //    Constants.PlayerType.PlayerA : Constants.PlayerType.PlayerB;

        _multiplayController = multiplayController;
        _roomId = roomId;
        _isMultiplay = true;
    }

    #region 필수 메서드
    public override void OnEnter(GameLogic gameLogic)
    {
        // 1. First Player인지 확인해서 게임 UI에 현재 턴 표시
        if (_isFirstPlayer)
        {
            GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.ATurn);
        }
        else
        {
            GameManager.Instance.SetGameTurnPanel(GameUIController.GameTurnPanelType.BTurn);
        }

        // 2. Block Controller에게 해야 할 일을 전달
        gameLogic.blockController.OnBlockClickedDelegate = (row, col) =>
        {
            // Block이 터치 될 때까지 기다렸다가 터치 되면 처리할 일
            HandleMove(gameLogic, row, col);
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        gameLogic.blockController.OnBlockClickedDelegate = null;
    }

    public override void HandleMove(GameLogic gameLogic, int row, int col)
    {
        ProcessMove(gameLogic, _playerType, row, col);

        if (_isMultiplay) // 서버에 Marker 정보 전달
        {
            _multiplayController.DoPlayer(_roomId, row * Constants.BlockColumnCount + col);
        }
    }

    protected override void HandleNextTurn(GameLogic gameLogic)
    {
        if (_isFirstPlayer)
        {
            gameLogic.SetState(gameLogic.secondPlayerState);
        }
        else
        {
            gameLogic.SetState(gameLogic.firstPlayerState);
        }
    }
    #endregion
}
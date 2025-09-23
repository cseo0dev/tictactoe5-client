using System;
using System.Threading.Tasks;
using UnityEngine;

public static class OmokAI
{
    private const int MAX_DEPTH = 2;
    
    private const float MAX_SCORE = 100000;
    private const float OPEN_FOUR = 50000;
    private const float CLOSED_FOUR = 10000;
    private const float OPEN_THREE = 5000;
    private const float CLOSED_THREE = 1000;
    private const float OPEN_TWO = 100;
    private const float CLOSED_TWO = 10;
    
    // 방어 가중치
    private const float DEFENSE_MULTIPLIER = 2f;
    
    // 현재 상태를 전달하면 다음 최적의 수를 반환하는 메서드
    public static async Task<(int row, int col)?> GetBestMove(Constants.PlayerType[,] board)
    {
        return await Task.Run(() =>
        {
            float bestScore = -MAX_SCORE;
            (int row, int col)? movePosition = null;
        
            float alpha = -MAX_SCORE;
            float beta = MAX_SCORE;

            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == Constants.PlayerType.None)
                    {
                        board[row, col] = Constants.PlayerType.PlayerB;
                        var score = DoMiniMax(board, 0, false, alpha, beta);
                        // PrintBoard(board);
                        board[row, col] = Constants.PlayerType.None;
                    
                        // 가장 높은 점수와 그 점수의 row, col 정보를 저장한다/
                        if (score > bestScore)
                        {
                            bestScore = score;
                            movePosition = (row, col);
                        }
                    
                        // Max 에서는 기존 alpha와 bestScore 사이에 높은 값을 alpha에 저장한다
                        alpha = Math.Max(alpha, bestScore);
                    }
                }
            }

            return movePosition;
        });
    }

    private static float DoMiniMax(Constants.PlayerType[,] board, int depth, bool isMaximizing, float alpha, float beta)
    {
        // 게임 종료 상태 체크
        if (CheckGameWin(Constants.PlayerType.PlayerA, board))
            return -MAX_SCORE;
        if (CheckGameWin(Constants.PlayerType.PlayerB, board))
            return MAX_SCORE;
        if (CheckGameDraw(board))
            return 0;
        if (depth >= MAX_DEPTH)
            return EvaluateBoard(board);
        
        Debug.Log("depth : " + depth);

        if (isMaximizing)
        {
            var bestScore = -MAX_SCORE;
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == Constants.PlayerType.None)
                    {
                        board[row, col] = Constants.PlayerType.PlayerB;
                        var score = DoMiniMax(board, depth + 1, false, alpha, beta);
                        // PrintBoard(board);
                        board[row, col] = Constants.PlayerType.None;
                        bestScore = Math.Max(score, bestScore);
                        alpha = Math.Max(alpha, bestScore);
                        
                        if (beta <= alpha) break;
                    }
                }
                if (beta <= alpha) break;
            }
            return bestScore;
        }
        else
        {
            var bestScore = MAX_SCORE;
            for (var row = 0; row < board.GetLength(0); row++)
            {
                for (var col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == Constants.PlayerType.None)
                    {
                        board[row, col] = Constants.PlayerType.PlayerA;
                        var score = DoMiniMax(board, depth + 1, true, alpha, beta);
                        // PrintBoard(board);
                        board[row, col] = Constants.PlayerType.None;
                        bestScore = Math.Min(score, bestScore);
                        beta = Math.Min(beta, bestScore);
                        
                        if (beta <= alpha) break;
                    }
                }
                if (beta <= alpha) break;
            }
            return bestScore;
        }
    }
    
    // 비겼는지 확인
    public static bool CheckGameDraw(Constants.PlayerType[,] board)
    {
        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var col = 0; col < board.GetLength(1); col++)
            {
                if (board[row, col] == Constants.PlayerType.None) return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// 15x15 오목판에서 승리한 플레이어를 확인하는 함수
    /// </summary>
    /// <param name="playerType">확인할 플레이어 타입</param>
    /// <param name="board">현재 게임 보드</param>
    /// <returns>해당 플레이어가 승리했으면 true, 아니면 false</returns>
    public static bool CheckGameWin(Constants.PlayerType playerType, Constants.PlayerType[,] board)
    {
        // None 타입은 승리 조건에서 제외
        if (playerType == Constants.PlayerType.None)
            return false;

        // 가로 방향 확인
        for (var row = 0; row < board.GetLength(0); row++)
        {
            for (var col = 0; col <= board.GetLength(1) - 5; col++)
            {
                bool win = true;
                for (var i = 0; i < 5; i++)
                {
                    if (board[row, col + i] != playerType)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) 
                {
                    return true;
                }
            }
        }

        // 세로 방향 확인
        for (var row = 0; row <= board.GetLength(0) - 5; row++)
        {
            for (var col = 0; col < board.GetLength(1); col++)
            {
                bool win = true;
                for (var i = 0; i < 5; i++)
                {
                    if (board[row + i, col] != playerType)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) 
                {
                    return true;
                }
            }
        }

        // 대각선 방향 (좌상단 -> 우하단) 확인
        for (var row = 0; row <= board.GetLength(0) - 5; row++)
        {
            for (var col = 0; col <= board.GetLength(1) - 5; col++)
            {
                bool win = true;
                for (var i = 0; i < 5; i++)
                {
                    if (board[row + i, col + i] != playerType)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) 
                {
                    return true;
                }
            }
        }

        // 대각선 방향 (우상단 -> 좌하단) 확인
        for (var row = 0; row <= board.GetLength(0) - 5; row++)
        {
            for (var col = 4; col < board.GetLength(1); col++)
            {
                bool win = true;
                for (var i = 0; i < 5; i++)
                {
                    if (board[row + i, col - i] != playerType)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) 
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    private static float EvaluateBoard(Constants.PlayerType[,] board)
    {
        float score = 0;
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        // 각 방향에 대한 패턴 평가
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (board[row, col] == Constants.PlayerType.None)
                    continue;

                bool isAI = board[row, col] == Constants.PlayerType.PlayerB;
                float multiplier = isAI ? 1 : -DEFENSE_MULTIPLIER;

                // 가로 방향
                if (col <= cols - 5)
                {
                    var pattern = EvaluatePattern(board, row, col, 0, 1, 5);
                    score += pattern * multiplier;
                }

                // 세로 방향
                if (row <= rows - 5)
                {
                    var pattern = EvaluatePattern(board, row, col, 1, 0, 5);
                    score += pattern * multiplier;
                }

                // 대각선 방향 (우하향)
                if (row <= rows - 5 && col <= cols - 5)
                {
                    var pattern = EvaluatePattern(board, row, col, 1, 1, 5);
                    score += pattern * multiplier;
                }

                // 대각선 방향 (우상향)
                if (row >= 4 && col <= cols - 5)
                {
                    var pattern = EvaluatePattern(board, row, col, -1, 1, 5);
                    score += pattern * multiplier;
                }
            }
        }

        return score;
    }

    private static float EvaluatePattern(Constants.PlayerType[,] board, int startRow, int startCol, int dRow, int dCol, int length)
    {
        var currentPlayer = board[startRow, startCol];
        int count = 1;
        int emptyBefore = 0;
        int emptyAfter = 0;
        bool blocked = false;

        // 연속된 돌 확인
        for (int i = 1; i < length; i++)
        {
            int newRow = startRow + dRow * i;
            int newCol = startCol + dCol * i;

            if (!IsValidPosition(newRow, newCol, board))
            {
                blocked = true;
                break;
            }

            if (board[newRow, newCol] == currentPlayer)
            {
                count++;
            }
            else if (board[newRow, newCol] == Constants.PlayerType.None)
            {
                emptyAfter++;
                break;
            }
            else
            {
                blocked = true;
                break;
            }
        }

        // 반대 방향 빈 공간 확인
        for (int i = 1; i < length; i++)
        {
            int newRow = startRow - dRow * i;
            int newCol = startCol - dCol * i;

            if (!IsValidPosition(newRow, newCol, board))
            {
                blocked = true;
                break;
            }

            if (board[newRow, newCol] == Constants.PlayerType.None)
            {
                emptyBefore++;
                break;
            }
            else if (board[newRow, newCol] != currentPlayer)
            {
                blocked = true;
                break;
            }
        }

        // 패턴 점수 계산
        if (count >= 5) return MAX_SCORE;
        
        bool isOpen = emptyBefore > 0 && emptyAfter > 0;
        
        switch (count)
        {
            case 4:
                return isOpen ? OPEN_FOUR : (blocked ? CLOSED_FOUR / 2 : CLOSED_FOUR);
            case 3:
                return isOpen ? OPEN_THREE : (blocked ? CLOSED_THREE / 2 : CLOSED_THREE);
            case 2:
                return isOpen ? OPEN_TWO : (blocked ? CLOSED_TWO / 2 : CLOSED_TWO);
            default:
                return 0;
        }
    }

    private static bool IsValidPosition(int row, int col, Constants.PlayerType[,] board)
    {
        // row가 0보다 크거나 같고, row가 전체 row 길이보다 작고, col이 0보다 크거나 같고, col이 전체 col 길이보다 작다면
        return row >= 0 && row < board.GetLength(0) && col >= 0 && col < board.GetLength(1);
    }
    
    private static bool IsMinimaxValidPosition(int row, int col, Constants.PlayerType[,] board)
    {
        // row가 0보다 크고, col이 0보다 크고, board[row-1, col-1]이 None 이니거나 [왼쪽 상단]
        // row가 0보다 크고, board[row-1, col]이 None이 아니거나 [윗쪽]
        // row가 0보다 크고, col이 전체 col 길이 보다 1 작고, [오른쪽 상단]
        // [왼쪽]
        // [오른쪽]
        // [왼쪽 하단]
        // [아랫쪽]
        // [우측 하단]
        
        if ((row > 0 && col > 0 && board[row - 1, col - 1] != Constants.PlayerType.None) ||
            (row > 0 && board[row - 1, col] != Constants.PlayerType.None) ||
            (row > 0 && col < board.GetLength(1) - 1 && board[row - 1, col + 1] != Constants.PlayerType.None) ||
            (col > 0 && board[row, col - 1] != Constants.PlayerType.None) ||
            (col < board.GetLength(1) - 1 && board[row, col + 1] != Constants.PlayerType.None) ||
            (row < board.GetLength(0) - 1 && col > 0 && board[row + 1, col - 1] != Constants.PlayerType.None) ||
            (row < board.GetLength(0) - 1 && board[row + 1, col] != Constants.PlayerType.None) ||
            (row < board.GetLength(0) - 1 && col < board.GetLength(1) - 1 && board[row + 1, col + 1] != Constants.PlayerType.None))
        {
            return true;
        }
        return false;
    }
    
    public static void PrintBoard(Constants.PlayerType[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);

        string output = "\n";
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                switch (board[row, col])
                {
                    case Constants.PlayerType.PlayerA:
                        output += "[o]";
                        break;
                    case Constants.PlayerType.PlayerB:
                        output += "[x]";
                        break;
                    default:
                        output += "[ ]";
                        break;
                }
            }
            output += "\n";
        }
        Debug.Log(output);
    }
}
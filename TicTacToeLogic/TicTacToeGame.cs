using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeLogic
{
    public class TicTacToeGame
    {
        public enum eGameStatus { Tie = 0, Player1Win, Player2Win, Player1Turn, Player2Turn};

        private Player[] m_Players;
        private GameBoard m_Board;
        private eGameStatus m_CurrentGameStatus;
        private int m_TurnNumber; //starts from zero
        private static readonly int sr_MinBoardSize = 3;
        private static readonly int sr_MaxBoardSize = 9;

        public TicTacToeGame(int i_BordSize, Player i_Player1, Player i_Player2)
        {
            m_Players = new Player[2];
            m_Players[0] = i_Player1;
            m_Players[1] = i_Player2;
            m_Board = new GameBoard(i_BordSize);
            m_TurnNumber = 0;
            m_CurrentGameStatus = eGameStatus.Player1Turn;
        }

        public GameBoard Board
        {
            get
            {
                return m_Board;
            }
        }

        public eGameStatus CurrentGameStatus
        {
            get
            {
                return m_CurrentGameStatus;
            }
        }

        public Player Player1
        {
            get
            {
                return m_Players[0];
            }
        }

        public Player Player2
        {
            get
            {
                return m_Players[1];
            }
        }

        public void NewRound()
        {
            m_Board.InitBoard();
            m_TurnNumber = 0;
            m_CurrentGameStatus = eGameStatus.Player1Turn;
        }

        public Player WhosTurn()
        {
            Player currentPlayer;

            if(m_CurrentGameStatus == eGameStatus.Player1Turn)
            {
                currentPlayer = m_Players[0];
            }
            else
            {
                currentPlayer = m_Players[1];
            }

            return currentPlayer;
        }

        public static bool IsLegelBoardSize(int i_BoardSize)
        {
            bool isLegelSize = true;

            if (i_BoardSize < sr_MinBoardSize || i_BoardSize > sr_MaxBoardSize)
            {
                isLegelSize = false;
            }

            return isLegelSize;
        }

        private bool checkPlayerNextCell(int i_Row, int i_Col)
        {
            return (m_Board.IsInBoard(i_Row, i_Col) && m_Board.IsVacentCell(i_Row, i_Col));
        }

        public bool SetPlayerchoice(int i_Row, int i_Col, Player i_Player)
        {
            bool isMoveSeted = true;
            if (checkPlayerNextCell(i_Row, i_Col) == true)
            {
                m_Board.SetCell(i_Row, i_Col, i_Player.Sign);
                checkIfGameOver();
                switchTurns(i_Player);
                m_TurnNumber++;
            }
            else
            {
                isMoveSeted = false;
            }

            return isMoveSeted;
        }

        private void switchTurns(Player i_CurrentPlayer)
        {
            if (m_CurrentGameStatus == eGameStatus.Player2Turn || m_CurrentGameStatus == eGameStatus.Player1Turn) //if no one win
            {
                if (i_CurrentPlayer == m_Players[0])
                {
                    m_CurrentGameStatus = eGameStatus.Player2Turn;
                }
                else
                {
                    m_CurrentGameStatus = eGameStatus.Player1Turn;
                }
            }
        }

        private void checkIfGameOver()
        {
            if (m_TurnNumber < m_Board.Board.Length)
            {
                Player justPlayed = WhosTurn();

                if (m_Board.IsSequenceFound(justPlayed.Sign))
                {
                    if (justPlayed.Equals(m_Players[0]))
                    {
                        m_CurrentGameStatus = eGameStatus.Player2Win;
                        Player2.UpdateScore();
                    }
                    else
                    {
                        m_CurrentGameStatus = eGameStatus.Player1Win;
                        Player1.UpdateScore();
                    }
                }
                else if (m_TurnNumber == m_Board.Board.Length - 1)
                {
                    m_CurrentGameStatus = eGameStatus.Tie;
                }
            }
        }

        public bool IsGameOver()
        {
            bool gameOver = false;

            if(m_CurrentGameStatus == eGameStatus.Player1Win || m_CurrentGameStatus == eGameStatus.Player2Win || m_CurrentGameStatus == eGameStatus.Tie)
            {
                gameOver = true;
            }

            return gameOver;
        }

        public void ComputerTurn()
        {
            int row, col;
            bool endOfTurn = false;

            while (endOfTurn == false)
            {
                if (m_Board.Size > 3)
                {
                    Random randomCell = new Random();
                    row = randomCell.Next(0,m_Board.Size);
                    col = randomCell.Next(0, m_Board.Size);
                    if (SetPlayerchoice(row, col, Player2) == true)
                    {
                        endOfTurn = true;
                    }
                }
                else
                {
                    GameBoard AIboard = m_Board.BoardClone();
                    eGameStatus currentStatus = m_CurrentGameStatus;
                    FindBestMove(AIboard, out row, out col, m_TurnNumber);
                    m_CurrentGameStatus = currentStatus;
                    SetPlayerchoice(row, col, Player2);
                    endOfTurn = true;
                }
            }
        }

        public void FindBestMove(GameBoard i_AIBoard, out int o_Row, out int o_Col, int i_Turn)
        {
            int bestScore = -1000;
            bool isCurrentPlayerTurn = true;
            o_Row = 0;
            o_Col = 0;

            for (int i = 0; i < m_Board.Size; i++)
            {
                for(int j=0; j  < m_Board.Size; j++)
                {
                    if(i_AIBoard.IsVacentCell(i,j))
                    {
                        i_AIBoard.SetCell(i, j, Player2.Sign);
                        int score = reverseMinimax(i_AIBoard, isCurrentPlayerTurn, i_Turn + 1);
                        i_AIBoard.SetCell(i, j, ' ');
                        if(score > bestScore)
                        {
                            bestScore = score;
                            o_Row = i;
                            o_Col = j;
                        }
                    }
                }
            }
        }

        private int reverseMinimax(GameBoard i_AIBoard, bool i_IsCurrentPlayerTurn, int i_Turn)
        {
            int bestScore, score;
            if (isAICheckDone(i_AIBoard, i_Turn, out score))
            {
               bestScore = score;
            }
            else if (i_IsCurrentPlayerTurn)
            {
                bestScore = 1000;
                for (int i = 0; i < m_Board.Size; i++)
                {
                    for (int j = 0; j < m_Board.Size; j++)
                    {
                        if (i_AIBoard.IsVacentCell(i, j))
                        {
                            i_AIBoard.SetCell(i, j, WhosTurn().Sign);
                            score = (i_AIBoard.Board.Length - i_Turn + 1)*(reverseMinimax(i_AIBoard, !i_IsCurrentPlayerTurn, i_Turn + 1));
                            i_AIBoard.SetCell(i, j, ' ');
                            bestScore = Math.Min(bestScore, score);
                        }
                    }
                }
            }
            else
            {
                bestScore = -1000;
                for (int i = 0; i < m_Board.Size; i++)
                {
                    for (int j = 0; j < m_Board.Size; j++)
                    {
                        if (i_AIBoard.IsVacentCell(i, j))
                        {
                            i_AIBoard.SetCell(i, j, WhosTurn().Sign);
                            score = (i_AIBoard.Board.Length - i_Turn + 1) * (reverseMinimax(i_AIBoard, i_IsCurrentPlayerTurn, i_Turn + 1));
                            i_AIBoard.SetCell(i, j, ' ');
                            bestScore = Math.Max(bestScore, score);
                        }
                    }
                }
            }
            switchTurns(WhosTurn());
            return bestScore;
        }

        private bool isAICheckDone(GameBoard i_AIBoard, int i_TurnNumber, out int o_MinimaxScore)
        {
            o_MinimaxScore = 0;
            bool checkIsDone = false;
     
            if (i_TurnNumber < m_Board.Board.Length)
            {
                if (i_AIBoard.IsSequenceFound(m_Players[0].Sign))
                {
                    checkIsDone = true;
                    o_MinimaxScore = 1;
                }
                else if (i_AIBoard.IsSequenceFound(m_Players[1].Sign))
                {
                    checkIsDone = true;
                    o_MinimaxScore = -1;
                }
            }

            if (i_TurnNumber == m_Board.Board.Length - 1)
            {
                checkIsDone = true;
            }

            return checkIsDone;
        }
    }
}

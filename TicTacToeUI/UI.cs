using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeLogic;

namespace TicTacToeUI
{
    public static class UI
    {
        public static void StartGame()
        {
            int boardSize, gameType;
            string player1Name, player2Name;

            Console.WriteLine("Hi! Welcome to TicTacToe game!");
            getGameBoardSize(out boardSize);
            getGameTypeInput(out gameType);
            getPlayersName(gameType, out player1Name, out player2Name);
            TicTacToeGame gameRound = new TicTacToeGame(boardSize, new Player('X', 1, player1Name), new Player('O', gameType, player2Name));
            gameRound.Board.InitBoard(); 
            runGame(gameRound);
        }

        private static void getGameBoardSize(out int o_BoardSize)
        {
            Console.WriteLine("Please enter board game size:");
            string input = Console.ReadLine();

            while (isLegalBoardSize(input, out o_BoardSize) == false && input != "Q")
            {
                Console.WriteLine("Wrong Input!");
                Console.WriteLine("Please enter board game size:");
                input = Console.ReadLine();
            }

            if (input == "Q")
            {
                Environment.Exit(1);
            }
        }

        private static void getGameTypeInput(out int o_GameType)
        {
            Console.WriteLine("Press 1 for a two-player game, or 2 to play against the computer");
            string input = Console.ReadLine();

            while (!isLegalGameType(input, out o_GameType) && input != "Q")
            {
                Console.WriteLine("Wrong Input!");
                Console.WriteLine("Press 1 for a two-player game, or 2 to play against the computer");
                input = Console.ReadLine();
            }

            if (input == "Q")
            {
                Environment.Exit(1);
            }
        }

        private static void getPlayersName(int i_GameType, out string o_Player1Name, out string o_Player2Name)
        {
            Console.WriteLine("Player 1: Please enter your name: ");
            o_Player1Name = Console.ReadLine();
            o_Player2Name = "computer";
            if (i_GameType == 1)
            {
                Console.WriteLine("Player 2: Please enter your name: ");
                o_Player2Name = Console.ReadLine();
            }
        }

        private static void runGame(TicTacToeGame i_GameRound)
        {
            bool gameOver = false;

            while (gameOver == false)
            {
                Ex02.ConsoleUtils.Screen.Clear();
                printBoard(i_GameRound.Board);
                if ((i_GameRound.WhosTurn().Sign == i_GameRound.Player2.Sign) && (i_GameRound.Player2.Type == Player.ePlayerType.Computer))
                {
                    i_GameRound.ComputerTurn();
                }
                else
                {
                    playerTurn(i_GameRound, i_GameRound.WhosTurn());
                }

                Ex02.ConsoleUtils.Screen.Clear();
                printBoard(i_GameRound.Board);
                gameOver = i_GameRound.IsGameOver();
            }

            RoundIsOver(i_GameRound);
        }

        private static void playerTurn(TicTacToeGame i_GameRound, Player i_Player)
        {
            int row, col;
            bool playerQuit, endOfTurn = false;

            playerQuit = getNextCellFromPlayer(out row, out col, i_Player.Name);

            while (endOfTurn == false)
            {
                if (playerQuit == true)
                {
                    RoundIsOver(i_GameRound);
                    endOfTurn = true;
                }
                else
                {
                    row--;
                    col--;
                    if (i_GameRound.SetPlayerchoice(row, col, i_Player) == true)
                    {
                        endOfTurn = true;
                    }
                    else
                    {
                        Console.WriteLine("The cell you picked is illegel!");
                        playerQuit = getNextCellFromPlayer(out row, out col, i_Player.Name);
                    }
                }
            }
        }

        public static void RoundIsOver(TicTacToeGame i_GameRound)
        {
            printScore(i_GameRound);
            Console.WriteLine();
            Console.WriteLine("Press 1 to another game, 2 to exit!");
            int input = int.Parse(Console.ReadLine());

            if (input == 1)
            {
                i_GameRound.NewRound();
                runGame(i_GameRound);
            }
            else if (input == 2)
            {
                Environment.Exit(1);
            }
        }

        private static void printScore(TicTacToeGame i_GameRound)
        {
            switch (i_GameRound.CurrentGameStatus)
            {
                case TicTacToeGame.eGameStatus.Tie:
                    {
                        Console.WriteLine("It's a Tie!");
                        break;
                    }
                case TicTacToeGame.eGameStatus.Player1Win:
                    {
                        Console.WriteLine("Player 1 Win!");
                        break;
                    }
                case TicTacToeGame.eGameStatus.Player2Win:
                    {
                        Console.WriteLine("Player 2 Win!");
                        break;
                    }
            }

            Console.WriteLine();
            string scoreTable = String.Format
                (@"{0} score: {1}
{2} score: {3}", i_GameRound.Player1.Name, i_GameRound.Player1.Score, i_GameRound.Player2.Name, i_GameRound.Player2.Score);
            Console.WriteLine(scoreTable);
        }

        private static bool isLegalBoardSize(string i_SizeInput, out int o_BoardSize)
        {
            bool isGoodInput = int.TryParse(i_SizeInput, out o_BoardSize);
            
            if(isGoodInput)
            {
                if (!TicTacToeGame.IsLegelBoardSize(o_BoardSize))
                {
                    isGoodInput = false;
                }
            }

            return isGoodInput;
        }

        private static bool isLegalGameType(string i_ChoiceInput, out int o_GameType)
        {
            bool isLegalInput = int.TryParse(i_ChoiceInput, out o_GameType);

            if (isLegalInput == true)
            {
                if (o_GameType != 1 && o_GameType != 2)
                {
                    isLegalInput = false;
                }
            }

            return isLegalInput;
        }

        private static void printBoard(GameBoard i_Board)
        {
            StringBuilder board = new StringBuilder("  ");

            for (int i = 1; i <= i_Board.Size; i++)
            {
                board.Append(i + "   ");
            }

            board.Append('\n');
            for (int i = 1; i <= i_Board.Size; i++)
            {
                board.Append(i + "|");

                for (int j = 1; j <= i_Board.Size; j++)
                {
                    board.Append(i_Board.GetCell(i - 1, j - 1) + " | ");
                }

                board.Append('\n');
                board.Append(' ');
                board.AppendLine(new string('=', i_Board.Size * 4));
            }

            Console.WriteLine(board);
        }

        private static bool getNextCellFromPlayer(out int o_Row, out int o_Col, string i_PlayerName)
        {
            bool isQPressed = false, isIntegerRow = true, isIntegerCol = true, inputIsNotOk = true;
            string input;
            string[] rowAndColInput;
            string playerTurnMsg = String.Format(@"{0} Turn: 
Please pick a cell - Row and Col separated by, :", i_PlayerName);

            Console.WriteLine(playerTurnMsg);
            o_Row = o_Col = 0;

            while (inputIsNotOk == true && isQPressed == false)
            {
                input = Console.ReadLine();
                rowAndColInput = input.Split(',', ' ');
                if(rowAndColInput.Length == 2)
                {
                    isIntegerRow = int.TryParse(rowAndColInput[0], out o_Row);
                    isIntegerCol = int.TryParse(rowAndColInput[1], out o_Col);

                    if (isIntegerRow == false || isIntegerCol == false)
                    {
                        Console.WriteLine("Wrong Input, Try again: ");
                    }
                    else
                    {
                        inputIsNotOk = false;
                    }
                }
                else
                {
                    if (input == "Q")
                    {
                        isQPressed = true;
                    }
                    else
                    {
                        Console.WriteLine("Wrong Input, Try again: ");
                    }
                }
            }

            return isQPressed;
        }
    }
}

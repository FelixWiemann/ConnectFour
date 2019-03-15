using System;
using VierGewinnt;

namespace ConnectFour
{
    public class Board
    {
        /// <summary>
        /// last played position on the board
        /// !! y,x !! dafuq did i do?
        /// </summary>
        private int[] nLastPlayedPosition = { 0, 0 };
        /// <summary>
        /// Board of current game of Connect four 
        /// played position gets player number
        /// not yet played = 0
        /// use: board[y,x] = player number
        /// </summary>
        private int[,] nPlayBoard = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };

        private static string[] sPlayerRepresentation = {" ","X","O"};

        public Board()
        {

        }

        public Board(Board copy)
        {
            nLastPlayedPosition[0] = copy.nLastPlayedPosition[0];
            nLastPlayedPosition[1] = copy.nLastPlayedPosition[1];
            this.nPlayBoard = copy.createCopyOfField();
        }

        /// <summary>
        /// draws the board of connect four
        /// </summary>
        public void drawBoard()
        {
            drawBoard(nPlayBoard, false);
        }

        public void drawBoard(bool bWriteToLog)
        {
            drawBoard(nPlayBoard, bWriteToLog);
        }

        /// <summary>
        /// draws the given board
        /// </summary>
        /// <param name="nBoard"></param>
        /// <param name="bWriteToLog"></param>
        private void drawBoard(int[,] nBoard, bool bWriteToLog)
        {
            // whether to print on console or write in file
            bool bPrintOnConsole = true;

            // clear console
            Console.Clear();
            // header
            Program.print("connect four:", bPrintOnConsole, bWriteToLog);
            // write colum-header
            Program.print("|0|1|2|3|4|5|6|", bPrintOnConsole, bWriteToLog);
            string sRow = "";
            // write row for row
            for (int y = 0; y < 6; y++)
            {
                // seperator between rows
                Program.print("+-+-+-+-+-+-+-+", bPrintOnConsole, bWriteToLog);
                for (int x = 0; x < 7; x++)
                {
                    Program.printColorful("|", 0);
                    Program.printColorful(sPlayerRepresentation[nBoard[y, x]], nBoard[y, x]);
                    // append each line
                    sRow += "|" + sPlayerRepresentation[nBoard[y, x]];
                }
                // finalizerow
                sRow += "|";
                Program.print("|");
                //print(sRow, bPrintOnConsole, bWriteToLog);
                // reset row
                sRow = "";
            }
            // finally print last row
            Program.print("+-+-+-+-+-+-+-+", bPrintOnConsole, bWriteToLog);
        }

 

        /// <summary>
        /// copy the given board in a new instance
        /// </summary>
        /// <param name="nBoardToCopy"></param>
        /// <returns></returns>
        private int[,] createCopyOfField()
        {
            // create new board
            int[,] board = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };
            // copy all positions to the new board
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    board[y, x] = nPlayBoard[y, x];
                }
            }
            // return board
            return board;
        }

        /// <summary>
        /// clears the board of connect four
        /// </summary>
        public void reset()
        {
            // set every value to zero
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    nPlayBoard[y, x] = 0;
                }
            }
        }

        /// <summary>
        /// Sets the first free value in the given column x to the given player.
        /// WARINING: no check whether play is valid or not
        /// </summary>
        /// <param name="nPlayer">player currently playing</param>
        /// <param name="nColumnToPlay">column the player wants to play</param>
        public void play(int nPlayer, int nColumnToPlay, bool bPrint)
        {
            // store played position fpr further use
            nLastPlayedPosition[0] = getFirstEmpty(nColumnToPlay); //y
            nLastPlayedPosition[1] = nColumnToPlay; //x
            // set the player on the board
            nPlayBoard[nLastPlayedPosition[0], nLastPlayedPosition[1]] = nPlayer;
            // draw the new board
            if (bPrint)
            {
                drawBoard();
            }
        }

        public void play(int nPlayer, int nColumnToPlay)
        {
            play(nPlayer, nColumnToPlay, true);
        }

        #region wincheck
        /// <summary>
        /// checks whether the player won with his last turn (4 in a row donwards)
        /// </summary>
        /// <param name="nPlayer">player who did the last turn</param>
        /// <returns>true if won, false if not</returns>
        private bool checkWinDown(int nPlayer)
        {
            return checkForCountDown(nPlayer, 4);
        }

        public bool checkForCountDown(int nPlayer, int pCount)
        {
            if (nLastPlayedPosition[0] > pCount - 2)
            {
                // can not be won downwards
                return false;
            }
            // all 4 downwards have to match
            for (int i = 0; i < pCount; i++)
            {
                // if one does not; can't be won this way
                if (nPlayBoard[nLastPlayedPosition[0] + i, nLastPlayedPosition[1]] != nPlayer)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// checks whether the player won with his last turn (4 in a row sideways)
        /// </summary>
        /// <param name="nPlayer">player who did the last turn</param>
        /// <returns>true if won, false if not</returns>
        private bool checkWinSideways(int nPlayer)
        {
            return checkForCountSideways(nPlayer, 4);
        }

        public bool checkForCountSideways(int nPlayer, int pCount)
        {
            int lToLeft = 0;
            int lToRight = 0;
            int lCount = 0;
            bool lLoopControl = true;
            // from last play position check all with same to left
            while (lLoopControl)
            {
                // make sure everything is in range
                if (nLastPlayedPosition[1] - lCount < 0 || nLastPlayedPosition[1] - lCount > 6)
                {
                    lLoopControl = false;
                }
                else if (nPlayBoard[nLastPlayedPosition[0], nLastPlayedPosition[1] - lCount] != nPlayer)
                {
                    lLoopControl = false;
                }
                else
                {
                    // count amounts of steps to left
                    lCount++;
                }
            }
            // store amount of same player marks
            lToLeft = lCount;
            // reset counter
            lCount = 0;
            lLoopControl = true;
            while (lLoopControl)
            {
                // make sure everything is in range
                if (nLastPlayedPosition[1] + lCount < 0 || nLastPlayedPosition[1] + lCount > 6)
                {
                    lLoopControl = false;
                }
                else if (nPlayBoard[nLastPlayedPosition[0], nLastPlayedPosition[1] + lCount] != nPlayer)
                {
                    lLoopControl = false;
                }
                else
                {
                    // count amounts of steps to left
                    lCount++;
                }
            }
            lToRight = lCount;
            int lCorrector = -1;
            if (lToLeft == 0 || lToRight == 0)
            {
                lCorrector = 0;
            }
            // if amount >= pCount, then more or equals the amount of played than checked for
            if (lToRight + lToLeft + lCorrector >= pCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// checks whether the player won with his last turn (4 in a row diagonal)
        /// </summary>
        /// <param name="nPlayer">player who did the last turn</param>
        /// <returns>true if won, false if not</returns>
        private bool checkWinDiagonal(int nPlayer)
        {
            // 4 cases: 2 at the end of line, 2 in middle for each diagonal play
            return checkForCountDiagonal(nPlayer, 4);
        }

        public bool checkForCountDiagonal(int nPlayer, int pCount)
        {

            int lToUpperLeft = 0;
            int lToUpperRight = 0;
            int lToLowerLeft = 0;
            int lToLowerRight = 0;
            int lCount = 0;
            bool lLoopControl = true;
            while (lLoopControl)
            {
                // make sure everything is in range
                if (nLastPlayedPosition[1] - lCount < 0 || nLastPlayedPosition[1] - lCount > 6 || nLastPlayedPosition[0] + lCount < 0 || nLastPlayedPosition[0] + lCount > 5)
                {
                    lLoopControl = false;
                }
                else if (nPlayBoard[nLastPlayedPosition[0] + lCount, nLastPlayedPosition[1] - lCount] != nPlayer)
                {
                    lLoopControl = false;
                }
                else
                {
                    // count amounts of steps to left
                    lCount++;
                }
            }

            // store amount of same player marks
            lToUpperRight = lCount;
            // reset counter
            lCount = 0;
            lLoopControl = true;
            while (lLoopControl)
            {
                // make sure everything is in range
                if (nLastPlayedPosition[1] + lCount < 0 || nLastPlayedPosition[1] + lCount > 6 || nLastPlayedPosition[0] + lCount < 0 || nLastPlayedPosition[0] + lCount > 5)
                {
                    lLoopControl = false;
                }
                else if (nPlayBoard[nLastPlayedPosition[0] + lCount, nLastPlayedPosition[1] + lCount] != nPlayer)
                {
                    lLoopControl = false;
                }
                else
                {
                    // count amounts of steps to left
                    lCount++;
                }
            }
            lToLowerRight = lCount;
            // reset counter
            lCount = 0;
            lLoopControl = true;
            while (lLoopControl)
            {
                // make sure everything is in range
                if (nLastPlayedPosition[1] + lCount < 0 || nLastPlayedPosition[1] + lCount > 6 || nLastPlayedPosition[0] - lCount < 0 || nLastPlayedPosition[0] - lCount > 5)
                {
                    lLoopControl = false;
                }
                else if (nPlayBoard[nLastPlayedPosition[0] - lCount, nLastPlayedPosition[1] + lCount] != nPlayer)
                {
                    lLoopControl = false;
                }
                else
                {
                    // count amounts of steps to left
                    lCount++;
                }
            }
            lToLowerLeft = lCount;
            // reset counter
            lCount = 0;
            // do the same but in steps to right
            lLoopControl = true;
            while (lLoopControl)
            {
                // make sure everything is in range
                if (nLastPlayedPosition[1] - lCount < 0 || nLastPlayedPosition[1] - lCount > 6 || nLastPlayedPosition[0] - lCount < 0 || nLastPlayedPosition[0] - lCount > 5)
                {
                    lLoopControl = false;
                }
                else if (nPlayBoard[nLastPlayedPosition[0] - lCount, nLastPlayedPosition[1] - lCount] != nPlayer)
                {
                    lLoopControl = false;
                }
                else
                {
                    // count amounts of steps to left
                    lCount++;
                }
            }
            lToUpperLeft = lCount;
            int lCorrector = -1;
            if (lToLowerLeft == 0 || lToUpperRight == 0)
            {
                lCorrector = 0;
            }
            // if amount >= pCount, then more or equals the amount of played than checked for
            if (lToLowerLeft + lToUpperRight + lCorrector >= pCount)
            {
                return true;
            }
            lCorrector = -1;
            if (lToLowerRight == 0 || lToUpperLeft == 0)
            {
                lCorrector = 0;
            }
            if (lToLowerRight + lToUpperLeft + lCorrector >= pCount)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// checks all win-possibilities for the player on the given board and the given last played position
        /// </summary>
        /// <param name="nPlayer">player who did the last turn</param>
        /// <param name="nBoard">board to check</param>
        /// <param name="nLastPlayedPos">last played position by "player"</param>
        /// <returns>true if won, false if not</returns>
        public bool checkwin(int nPlayer)
        {
            return checkwin(nPlayer, true);
        }

        /// <summary>
        /// checks all win-possibilities for the player on the given board and the given last played position
        /// </summary>
        /// <param name="nPlayer">player who did the last turn</param>
        /// <param name="nBoard">board to check</param>
        /// <param name="nLastPlayedPos">last played position by "player"</param>
        /// <param name="bPrint">whether to print or not</param>
        /// <returns>true if won, false if not</returns>
        public bool checkwin(int nPlayer, bool bPrint)
        {
            // check all possibilities
            if (checkWinDown(nPlayer))
            {
                // print if wanted
                if (bPrint)
                {
                    Program.print("congrats player " + nPlayer + ", you won (down)!");
                }
                return true;
            }
            if (checkWinSideways(nPlayer))
            {// print if wanted
                if (bPrint)
                {
                    Program.print("congrats player " + nPlayer + ", you won (sideways)!");
                }
                return true;
            }
            if (checkWinDiagonal(nPlayer))
            {// print if wanted
                if (bPrint)
                {
                    Program.print("congrats player " + nPlayer + ", you won (diagonal)!");
                }
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// returns the y-value of first empty field on the board in the given column in the given board
        /// </summary>
        /// <param name="x">column</param>
        /// <param name="nBoard">board to check the first free position in</param>
        /// <returns>y-value of the first empty field on the bord in the column, -1 if no empty field</returns>
        public int getFirstEmpty(int x)
        {
            int y = 0;
            // while board empty
            while (nPlayBoard[y, x] == 0)
            {
                y++;
                // if lower limit reached, break
                if (y == 6)
                {
                    break;
                }
            }
            // first non-null value found -> null one above
            return y - 1;
        }

    }
}

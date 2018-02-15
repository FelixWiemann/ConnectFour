using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace VierGewinnt
{

    /// <summary>
    /// TODO:
    /// 1) Command-line arguments, changable during playing to test things
    /// -l; --log
    ///-h; --help
    ///-pc; --PrintConsole
    ///-ww
    ///-lw
    ///--iterationdepth
    ///
    /// 3) enable 2 bots
    /// 5) extract player types to interface -> human player & bot player
    /// 
    /// Implemented TODO's  
    /// 2) comment code
    /// 4) logging to file
    ///  
    /// </summary
    public static class Program
    {
        /// <summary>
        /// Enumeration over Playertypes
        /// </summary>
        enum PlayerType { HUMAN_PLAYER, MACHINE_PLAYER };
        /// <summary>
        /// set Playertype of Player 1
        /// </summary>
        private static Player Player1;
        /// <summary>
        /// set Playertype of Player 2
        /// </summary>
        private static Player Player2;
        /// <summary>
        /// file name of logfile
        /// </summary>
        private static string sFileName = @"log";
        /// <summary>
        /// file path of logfile
        /// </summary>
        private static string sFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Connect4\";
        /// <summary>
        /// connects file name and path
        /// </summary>
        private static string sFullLogFilePath = sFilePath + sFileName;
        /// <summary>
        /// player array. sets sign of player
        /// </summary>
        private static string[] sPlayers = { " ", "X", "O" };
        /// <summary>
        /// last played position on the board
        /// !! y,x !! dafuq did i do?
        /// </summary>
        private static int[] nLastPlayedPos = { 0, 0 };
        /// <summary>
        /// Board of current game of Connect four 
        /// played position gets player number
        /// not yet played = 0
        /// use: board[y,x] = player number
        /// </summary>
        private static int[,] nPlayBoard = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };

        /// <summary>
        /// wether to log accurate or not
        /// </summary>
        private static bool bAccurateLog = false;

        /// <summary>
        /// wether players got asigned via commandline or not
        /// </summary>
        private static bool bPlayersAsignedViaCMD = false;

        /// <summary>
        /// main routine
        /// </summary>
        /// <param name="args">starting arguments of the program</param>
        public static void Main(string[] args)
        {
            // dirty: catch all errors
           // try
            {
                // set defaults
                Player1 = new Human(1, 2, nPlayBoard);
                Player2 = new Human(2, 1, nPlayBoard);
                
                // read player Types
                interpretArgs(args);
                // check whether log exists, if not, create directory
                if (Directory.Exists(sFilePath))
                {
                    
                }
                else
                {
                    Directory.CreateDirectory(sFilePath);
                }
                // intit vals
                bool bWon = false;
                bool bReplayRequested = false;

                int nCurrPlayer = 1;
                // roundcounter for tie
                int nRoundCounter = 0;

                print("");
                print("Hello to Connect Four!");

                if (bPlayersAsignedViaCMD)
                {
                    // if players already asigned, skip menu
                }
                else
                {
                    menu();
                }
                // beginn with drawing the board
                drawBoard(nPlayBoard);
                
                while (!bWon || bReplayRequested)
                {
                    // set won to false
                    bWon = false;
                    // check roundcounter
                    if (nRoundCounter == 6 * 7)
                    {// max amount of rpunds played

                        print("Tie!", true, true);
                        drawBoard(nPlayBoard, true);
                        bWon = true; // exit loop
                        bReplayRequested = playAgain();
                    }
                    // play
                    if (nCurrPlayer == 1)
                    {
                        Player1.LocalBoard = copyBoard(nPlayBoard);
                        Player1.play();
                    }
                    else
                    {
                        Player2.LocalBoard = copyBoard(nPlayBoard);
                        Player2.play();
                    }
                    // one more round
                    nRoundCounter++;
                    bWon = checkwin(nCurrPlayer, nPlayBoard);
                    // swap player
                    if (nCurrPlayer == 1)
                    {
                        nCurrPlayer = 2;
                    }
                    else
                    {
                        nCurrPlayer = 1;
                    }
                    // handle if won
                    if (bWon)
                    {
                        Console.ReadLine();
                        drawBoard(nPlayBoard, true);
                        bReplayRequested = playAgain();
                        if (bReplayRequested)
                        {// stay in loop
                            bReplayRequested = false;
                            bWon = false;
                            // reset roundcounter
                            nRoundCounter = 0;
                        }
                    }
                }
            }
            // at least sending Error-Message to user
            //catch (Exception e)
            //{
            
            //error(e.Message);
           // }
        }


        /// <summary>
        /// prints the given string to spezified locations
        /// </summary>
        /// <param name="sStringToPrint">string to print</param>
        /// <param name="bPrintOnConsole">print on console if true</param>
        /// <param name="bWriteInLog">print in file if true</param>
        internal static void print(string sStringToPrint, bool bPrintOnConsole, bool bWriteInLog)
        {
            if (bPrintOnConsole)
            {
                Console.WriteLine(sStringToPrint);
            }
            if (bWriteInLog)
            {
                File.AppendAllText(sFullLogFilePath, sStringToPrint + Environment.NewLine);
            }
        }

        /// <summary>
        /// prints the given string NOT in log file
        /// you may choose to log into console
        /// </summary>
        /// <param name="sStringToPrint">string to print</param>
        /// <param name="bPrintOnConsole">whether to print on console or not</param>
        public static void print(string sStringToPrint, bool bPrintOnConsole)
        {
            print(sStringToPrint, bPrintOnConsole, false);
        }

        /// <summary>
        /// prints the given string only on console
        /// </summary>
        /// <param name="sStringToPrint">string to print on console</param>
        public static void print(string sStringToPrint)
        {
            print(sStringToPrint, true,false);
        }
       
        /// <summary>
        /// draws the board of connect four
        /// </summary>
        public static void drawBoard(int[,] nBoard)
        {
            drawBoard(nBoard, false);
        }

        /// <summary>
        /// draws the given board
        /// </summary>
        /// <param name="nBoard"></param>
        /// <param name="bWriteToLog"></param>
        public static void drawBoard(int[,] nBoard, bool bWriteToLog){
            // whether to print on console or write in file
            bool bPrintOnConsole = true;
            
            // clear console
            Console.Clear();
            // header
            print("connect four:", bPrintOnConsole, bWriteToLog);
            // write colum-header
            print("|0|1|2|3|4|5|6|", bPrintOnConsole, bWriteToLog);
            string sRow = "";
            // write row for row
            for (int y = 0; y < 6; y++)
            {
                // seperator between rows
                print("+-+-+-+-+-+-+-+", bPrintOnConsole, bWriteToLog);
                for (int x = 0; x < 7; x++)
                {
                    // append each line
                    sRow += "|" + sPlayers[nBoard[y, x]];
                }
                // finalizerow
                sRow += "|";
                print(sRow, bPrintOnConsole, bWriteToLog);
                // reset row
                sRow = "";
            }
            // finally print last row
            print("+-+-+-+-+-+-+-+", bPrintOnConsole, bWriteToLog);
        }
        
        /// <summary>
        /// clears the board of connect four
        /// </summary>
        public static void resetBoard()
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
        public static void play(int nPlayer, int nColumnToPlay)
        {
            // store played position fpr further use
            nLastPlayedPos[0] = getFirstEmpty(nColumnToPlay,nPlayBoard); //y
            nLastPlayedPos[1] = nColumnToPlay; //x
            // set the player on the board
            nPlayBoard[nLastPlayedPos[0], nLastPlayedPos[1]] = nPlayer;
            // draw the new board
            drawBoard(nPlayBoard);
        }

        #region wincheck
        /// <summary>
        /// checks whether the player won with his last turn (4 in a row donwards)
        /// </summary>
        /// <param name="nPlayer">player who did the last turn</param>
        /// <returns>true if won, false if not</returns>
        public static bool checkWinDown(int nPlayer, int[,] nBoard, int[] nLastPlayedPos)
        {
            return checkForCountDown(nPlayer, nBoard, nLastPlayedPos,4);
        }

        public static bool checkForCountDown(int nPlayer, int[,] nBoard, int[] nLastPlayedPos, int pCount)
        {
            if (nLastPlayedPos[0] > pCount-2)
            {
                // can not be won downwards
                return false;
            }
            // all 4 downwards have to match
            for (int i = 0; i < pCount; i++)
            {
                // if one does not; can't be won this way
                if (nBoard[nLastPlayedPos[0] + i, nLastPlayedPos[1]] != nPlayer)
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
        public static bool checkWinSideways(int nPlayer, int[,] nBoard, int[] nLastPlayedPos)
        {
            return checkForCountSideways(nPlayer, nBoard, nLastPlayedPos,4);
        }

        public static bool checkForCountSideways(int nPlayer, int[,] nBoard, int[] nLastPlayedPos, int pCount)
        {
            int lToLeft = 0;
            int lToRight = 0;
            int lCount = 0;
            bool lLoopControl = true;
            // from last play position check all with same to left
            while (lLoopControl)
            {
                // make sure everything is in range
                if (nLastPlayedPos[1] - lCount < 0 || nLastPlayedPos[1] - lCount > 6)
                {
                    lLoopControl = false;
                }
                else if (nBoard[nLastPlayedPos[0], nLastPlayedPos[1] - lCount] != nPlayer)
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
                if (nLastPlayedPos[1] + lCount < 0 || nLastPlayedPos[1] + lCount > 6)
                {
                    lLoopControl = false;
                }
                else if (nBoard[nLastPlayedPos[0], nLastPlayedPos[1] + lCount] != nPlayer)
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
        public static bool checkWinDiagonal(int nPlayer, int[,] nBoard, int[] nLastPlayedPos)
        {
            // 4 cases: 2 at the end of line, 2 in middle for each diagonal play
            return checkForCountDiagonal(nPlayer, nBoard, nLastPlayedPos,4);
        }

        public static bool checkForCountDiagonal(int nPlayer, int[,] nBoard, int[] nLastPlayedPos, int pCount)
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
                if (nLastPlayedPos[1] - lCount < 0 || nLastPlayedPos[1] - lCount > 6 || nLastPlayedPos[0] + lCount < 0 || nLastPlayedPos[0] + lCount > 5)
                {
                    lLoopControl = false;
                }
                else if (nBoard[nLastPlayedPos[0] + lCount, nLastPlayedPos[1] - lCount] != nPlayer)
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
                if (nLastPlayedPos[1] + lCount < 0 || nLastPlayedPos[1] + lCount > 6 || nLastPlayedPos[0] + lCount < 0 || nLastPlayedPos[0] + lCount > 5)
                {
                    lLoopControl = false;
                }
                else if (nBoard[nLastPlayedPos[0]+lCount, nLastPlayedPos[1] + lCount] != nPlayer)
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
                if (nLastPlayedPos[1] + lCount < 0 || nLastPlayedPos[1] + lCount > 6 || nLastPlayedPos[0] - lCount < 0 || nLastPlayedPos[0] - lCount > 5)
                {
                    lLoopControl = false;
                }
                else if (nBoard[nLastPlayedPos[0]-lCount, nLastPlayedPos[1] + lCount]!= nPlayer)
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
                if (nLastPlayedPos[1] - lCount < 0 || nLastPlayedPos[1] - lCount > 6 || nLastPlayedPos[0] - lCount < 0 || nLastPlayedPos[0] - lCount > 5)
                {
                    lLoopControl = false;
                }
                else if (nBoard[nLastPlayedPos[0]-lCount, nLastPlayedPos[1] - lCount] != nPlayer)
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
        public static bool checkwin(int nPlayer, int[,] nBoard, int[] nLastPlayedPos)
        {
            return checkwin(nPlayer, nBoard, nLastPlayedPos, true);
        }

        /// <summary>
        /// checks all win-possibilities for the player on the given board and the given last played position
        /// </summary>
        /// <param name="nPlayer">player who did the last turn</param>
        /// <param name="nBoard">board to check</param>
        /// <param name="nLastPlayedPos">last played position by "player"</param>
        /// <param name="bPrint">whether to print or not</param>
        /// <returns>true if won, false if not</returns>
        public static bool checkwin(int nPlayer, int[,] nBoard, int[] nLastPlayedPos, bool bPrint)
        {
            // check all possibilities
            if (checkWinDown(nPlayer, nBoard, nLastPlayedPos))
            {
                // print if wanted
                if (bPrint)
                {
                    print("congrats player " + nPlayer + ", you won (down)!");
                }
                return true;
            }
            if (checkWinSideways(nPlayer, nBoard, nLastPlayedPos))
            {// print if wanted
                if (bPrint)
                {
                    print("congrats player " + nPlayer + ", you won (sideways)!");
                }
                return true;
            }
            if (checkWinDiagonal(nPlayer, nBoard, nLastPlayedPos))
            {// print if wanted
                if (bPrint)
                {
                    print("congrats player " + nPlayer + ", you won (diagonal)!");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// checks all win-possibilities for the player on the given board
        /// </summary>
        /// <param name="player">player who did the last turn</param>
        /// <param name="board">board to check</param>
        /// <returns>true if won, false if not</returns>
        public static bool checkwin(int player, int[,] board)
        {
            return checkwin(player, board, nLastPlayedPos);
        }
        #endregion

        /// <summary>
        /// returns the y-value of first empty field on the board in the given column in the given board
        /// </summary>
        /// <param name="x">column</param>
        /// <param name="nBoard">board to check the first free position in</param>
        /// <returns>y-value of the first empty field on the bord in the column, -1 if no empty field</returns>
        public static int getFirstEmpty(int x,int[,] nBoard)
        {
            int y = 0;
            // while board empty
            while (nBoard[y, x] == 0)
            {
                y++;
                // if lower limit reached, break
                if (y == 6)
                {
                    break;
                }
            }
            // first non-null value found -> null one above
            return y-1;
        }

        /// <summary>
        /// returns the y-value of first empty field on the board in the given column
        /// </summary>
        /// <param name="x">column</param>
        /// <returns>y-value of the first empty field on the bord in the column, -1 if no empty field</returns>
        public static int getFirstEmpty(int x)
        {
            return getFirstEmpty(x, nPlayBoard);
        }


        /// <summary>
        /// asks players whether to play again
        /// </summary>
        /// <returns>true if replay, false if not</returns>
        public static bool playAgain()
        {
            print("Do you want to play again? - Y/N");
            string res = Console.ReadLine();
            // loop to wait for correct user input
            while (true){
                // yes to replay
                if (res.ToLower().Equals("y"))
                {
                    // reset board
                    resetBoard();

                    return true;
                }
                // no to replay
                if (res.ToLower().Equals("n"))
                {
                    Console.Clear();
                    print("Bye!");
                    Thread.Sleep(1000);
                    return false;
                }
                Console.Clear();
                // retour if nothing worked
                print("Sorry, did not understand you");
                print("Do you want to play again? - Y/N");
                res = Console.ReadLine();
            }
        }

        /// <summary>
        /// copy the given board in a new instance
        /// </summary>
        /// <param name="nBoardToCopy"></param>
        /// <returns></returns>
        public static int[,] copyBoard(int[,] nBoardToCopy)
        {
            // create new board
            int[,] board = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };
            // copy all positions to the new board
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    board[y, x] = nBoardToCopy[y, x];
                }
            }
            // return board
            return board;
        }


        #region menu
        // constants for menu options
        const int MENU_OPT_TWO_HUM = 1;
        const int MENU_OPT_ONE_HUM = 2;
        const int MENU_OPT_TWO_COM = 3;
        const int MENU_OPT_HELP= 4;
        const int MENU_OPT_EXIT_MENU = 5;
        const int MENU_OPT_EXIT_GAME = 6;


        /// <summary>
        /// displaying and handling menu of game
        /// </summary>
        public static void menu()
        {
            // init vals
            int nChoice = 0;
            string sChoice = "";
            // menu loop
            while (true)
            {
                print("");
                print("What do you want to do?");
                print("");
                print("");
                // options
                print(MENU_OPT_TWO_HUM +" - 2 player");
                print(MENU_OPT_ONE_HUM+" - play against COM");
                print(MENU_OPT_TWO_COM + " - battle of COMs");
                print(MENU_OPT_HELP + " - help");
                print(MENU_OPT_EXIT_MENU +" - exit menu");
                print(MENU_OPT_EXIT_GAME+ " - exit game");
                // user input
                print("");
                print("You are always able to come back here by pressing m while playing");
                print("");
                print("");
                // get user selection
                sChoice = Console.ReadLine();
                Int32.TryParse(sChoice, out nChoice);
                // do over coice of user
                switch (nChoice)
                {
                    case MENU_OPT_TWO_HUM:
                        // two human players
                        resetBoard();
                        Player1 = new Human(1, 2, nPlayBoard);
                        Player2 = new Human(2, 1, nPlayBoard);
                        return;

                    case MENU_OPT_ONE_HUM:
                        // one COM, one human
                        resetBoard();
                        // difficulty
                        print("difficulty of bot? - (1..6)");
                        string res = Console.ReadLine();
                        int nDiff = 0;
                        if (Int32.TryParse(res, out nDiff))
                        {//if valid value for difficulty
                            print("do you want to be player 1? - (Y/N)");
                            res = Console.ReadLine();
                            if (res.ToLower().Equals("y"))
                            {
                                Player1 = new Human(1, 2, nPlayBoard);
                                Player2 = new Con4Bot(2, nDiff, 1, nPlayBoard);
                            }
                            else if (res.ToLower().Equals("n"))
                            {
                                Player2 = new Human(2, 1, nPlayBoard);
                                Player1 = new Con4Bot(1, nDiff, 2, nPlayBoard);
                            }
                            else
                            {
                                error("you don't want to be neither player 1 or 2 - what do you want?");
                            }
                        }
                        else
                        {//no valid value for difficulty
                            error("difficulty must be of type integer");
                        }
                        return;
                    case MENU_OPT_TWO_COM:
                        // battle of COMs
                        resetBoard();
                        print("difficulty of bot1? - (1..6)");
                        res = Console.ReadLine();
                        nDiff = 0;
                        if (Int32.TryParse(res, out nDiff))
                        {
                           Player1 = new Con4Bot(1, nDiff, 2, nPlayBoard);
                        }
                        else
                        {
                            error("difficulty must be of type integer");
                        }
                        print("difficulty of bot1? - (1..6)");
                        res = Console.ReadLine();
                        nDiff = 0;
                        if (Int32.TryParse(res, out nDiff))
                        {
                           Player2 = new Con4Bot(2, nDiff, 1, nPlayBoard);
                        }
                        else
                        {
                            error("difficulty must be of type integer");
                        }
                        return;
                    case MENU_OPT_HELP:
                        // help without exiting game
                        help(false);
                        break;
                    case MENU_OPT_EXIT_MENU:
                        // exit menu loop -> return to game
                        return;
                    case MENU_OPT_EXIT_GAME:
                        // exit game
                        Console.Clear();
                        print("Bye!");
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;
                    default:
                        print("unknown option, please try again");
                        break;
                }
                Console.ReadLine();
                Console.Clear();
            }
        }

        #endregion


        /// <summary>
        /// prints the help of the game
        /// </summary>
        /// <param name="bExitGameAfterwards">choose whether to exit afterwards or not</param>
        public static void help(bool bExitGameAfterwards)
        {
            print("You are in the help menu");
            print("\nObject: Connect four of your checkers in a row while preventing your opponent \n     from doing the same. \n\n         - Milton Bradley, Connect Four \"Pretty Sneaky, Sis\" \n            television commercial, 1977");
            print("\nDo so by typing the number of the column you want to play next");
            print("\n");
            print("you are able to start the game with different commands via commandline");
            print("arguments are:");
            print("-p[playerNO] [Type] [difficulty]  sets player with the given No [1/2] as ");
            print("                                  Type [COM/HUM] with the given difficulty [1..6]");
            print("                                  (only relevant for COM)");
            print("--logfile                         logs the result of the game in your ");
            print("                                  Appdata/local/Connect4 directory ");
            print("                                  (path validity not checked)");
            print("--logpath                         logs the result of the game in the given ");
            print("                                  directory (include filename)");
            print("-h                                opens this help-window");
            print("--help                            opens this help-window");
            
            Console.ReadKey();
            if (bExitGameAfterwards)
            {
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// interpret arguments given at start of game
        /// </summary>
        /// <param name="args">args given</param>
        public static void interpretArgs(string[] args)
        {
            //possible input:
            // Player:      -p[playerNO] [Type] [difficulty]
            // Help:        -h  
            //              --help
            // Logfile      --logfile
            // Log to path  --logpath
           

            // init vars for detection of player input
            bool bP1 = false;
            bool bP2 = false;

            for (int i=0; i < args.Length; i++)
            {
                // player -p[playerNO] [Type] [difficulty]
                // detect player 1
                if (args[i].Equals("-p1"))
                {
                    if (args[i + 1].ToLower().Equals("com"))
                    {
                        if (Regex.IsMatch(args[i + 2], @"^\d+$"))
                        {
                            Player1 = new Con4Bot(1, Convert.ToInt32(args[i + 2]), 2, nPlayBoard);
                        }
                        else
                        {
                            error("playerarguments must be of type integer");
                        }
                    }
                    else if (args[i + 1].ToLower().Equals("hum"))
                    {
                        Player1 = new Human(1, 2, nPlayBoard);
                    }
                    else
                    {
                        error("player type " + args[i + 1] + " unknown");
                    }
                    print("player 1 is " + args[i+1]);
                    // player one init done
                    bP1 = true;
                }
                // detect player 2
                if (args[i].Equals("-p2"))
                {
                    if (args[i + 1].ToLower().Equals("com"))
                    {
                        if (Regex.IsMatch(args[i + 2], @"^\d+$"))
                        {
                            Player2 = new Con4Bot(2, Convert.ToInt32(args[i + 2]), 1, nPlayBoard);
                        }
                        else
                        {
                            error("playerarguments must be of type integer");
                        }
                    }
                    else if (args[i + 1].ToLower().Equals("hum"))
                    {
                        Player2 = new Human(2, 1, nPlayBoard);
                    }
                    else
                    {
                        error("player type " + args[i + 1] + " unknown");
                    }
                    print("player 2 is " + args[i + 1]);
                    //player two init done
                    bP2 = true;
                }
                
                // help
                if (args[i].Equals("-h") || args[i].Equals("--help"))
                {
                    // display help, exit afterwards
                    help(true);
                }       
         
                // logfile
                if (args[i].Equals("--logfile"))
                {
                    // accurate log everything
                    bAccurateLog = true;
                    setLog();
                    // set logpath incl file
                    sFullLogFilePath = sFilePath + args[i + 1];
                    // check for special chars so file may not exist
                    if (args[i + 1].Contains("\\") || args[i + 1].Contains("/"))
                    {
                        // error
                        error("filename may not have path in it");
                    }
                    else
                    {//no error
                        Player1.setLogPath(sFullLogFilePath);
                        Player2.setLogPath(sFullLogFilePath);
                    }
                }
                // logpath
                if (args[i].Equals("--logpath"))
                {
                    // accurate log everything
                    bAccurateLog = true;
                    setLog();
                    //set path
                    sFullLogFilePath = args[i + 1];
                    // both / or \ possible
                    int inBack = sFullLogFilePath.LastIndexOf("\\");
                    int inSlash = sFullLogFilePath.LastIndexOf("/");
                    // detect last char
                    if (inBack>inSlash){
                        inSlash  = inBack;
                    }
                    // seperate path from filename
                    string path = sFullLogFilePath.Substring(0, sFullLogFilePath.Length - (sFullLogFilePath.Length - inSlash));
                    // check if path exists
                    if (Directory.Exists(path))
                    {
                        // yes, use it
                        Player1.setLogPath(sFullLogFilePath);
                        Player2.setLogPath(sFullLogFilePath);
                        
                    }
                    else
                    {
                        //ask user wether create or not
                        print("path not exsitant");
                        print("create path? - Y/N");
                        string res = Console.ReadLine().ToLower();
                        switch (res)
                        {
                            case "y":// create dir
                                Directory.CreateDirectory(path);
                                Player1.setLogPath(sFullLogFilePath);
                                Player2.setLogPath(sFullLogFilePath);
                                break;
                            case "n":
                                error("path not existant");
                                break;
                            default:
                                error("unknown input");
                                break;

                        }
                    }
                }
            }
            // if all players assigned, skip menu, else show menu
            bPlayersAsignedViaCMD = bP1 && bP2;
        }

        /// <summary>
        /// displays an error with the given message to the user. Exits game afterwards
        /// </summary>
        /// <param name="errorMsg">message to display</param>
        public static void error(string errorMsg)
        {
            print("\noups - an error occured:\n");
            print(errorMsg);
            print("\nexiting game\n");
            print("Press any key to acknowledge");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void setLog()
        {
            Player1.bAccurateLog = bAccurateLog;
            Player2.bAccurateLog = bAccurateLog;
        }

    }
}

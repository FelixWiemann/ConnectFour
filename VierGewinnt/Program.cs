using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

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
        private static string sFileName = @"log.txt";
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
        private static string[] players = { " ", "X", "O" };
        /// <summary>
        /// last played position on the board
        /// !! y,x !! dafuq did i do?
        /// </summary>
        private static int[] lastPlayedPos = { 0, 0 };
        /// <summary>
        /// Board of current game of Connect four 
        /// played position gets player number
        /// not yet played = 0
        /// use: board[y,x] = player number
        /// </summary>
        private static int[,] playBoard = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };

        


        /// <summary>
        /// main routine
        /// </summary>
        /// <param name="args">starting arguments of the program</param>
        public static void Main(string[] args)
        {
            Player1 = new Human(1,2,playBoard);
            Player1 = new Human(2, 1, playBoard);
            // read player Types
            interpretArgs(args);

            // check whether log exists, if not, create directory
            if (File.Exists(sFullLogFilePath))
            {
                File.Delete(sFullLogFilePath);
            }
            else
            {
                Directory.CreateDirectory(sFilePath);
            }
            bool won = false;
            bool replay = false;
            int currPlayer = 1;
            
            print("");
            print("Hello to Connect Four!");
            //menu();
            // beginn with drawing the board
            drawBoard(playBoard);
            // roundcounter for tie
            int roundcounter = 0;
            while (!won || replay)
            {
                // set won to false
                won = false;
                // check roundcounter
                if (roundcounter == 6 * 7)
                {
                    print("Tie!",true,true);
                    drawBoard(playBoard,true);
                    won = true;
                    return;
                    //replay = playAgain();
                }

                if (currPlayer == 1)
                {
                    Player1.LocalBoard = copyBoard(playBoard);
                    Player1.play();
                }
                else
                {
                    Player2.LocalBoard = copyBoard(playBoard);
                    Player2.play();
                }
                
                roundcounter++;
                won = checkwin(currPlayer, playBoard);
                // swap player
                if (currPlayer == 1)
                {
                    currPlayer = 2;
                }
                else
                {
                    currPlayer = 1;
                }
                // handle if won
                if (won)
                {
                    drawBoard(playBoard,true);
                    return;
                    //replay = playAgain();
                    if (replay)
                    {
                        //File.Delete(logPath);
                        // reset to stay in loop
                        replay = false;
                        won = false;
                        roundcounter = 0;
                    }
                }
            }
        }


        /// <summary>
        /// prints the given string to spezified locations
        /// </summary>
        /// <param name="s">string to print</param>
        /// <param name="bprintConsole">print on console if true</param>
        /// <param name="bwriteFile">print in file if true</param>
        internal static void print(string s, bool bprintConsole, bool bwriteFile)
        {
            if (bprintConsole)
            {
                Console.WriteLine(s);
            }
            if (bwriteFile)
            {
                File.AppendAllText(sFullLogFilePath, s + Environment.NewLine);
            }
        }

        /// <summary>
        /// prints the given string NOT in the log file
        /// you may choose to log into console
        /// </summary>
        /// <param name="s">string to print</param>
        /// <param name="bprintConsole">whether to print on console or not</param>
        public static void print(string s, bool bprintConsole)
        {
            print(s, bprintConsole, false);
        }

        /// <summary>
        /// prints the given string on the console
        /// </summary>
        /// <param name="s">string to print on console</param>
        public static void print(string s)
        {
            print(s, true,false);
        }
       
        /// <summary>
        /// draws the board of connect four
        /// </summary>
        public static void drawBoard(int[,] board)
        {
            drawBoard(board, false);
        }

        public static void drawBoard(int[,] board, bool bwf){
            // whether to print on console or write in file
            bool bpc = true;
            
            // clear console
            Console.Clear();
            // header
            print("connect four:", bpc, bwf);
            // write colum-header
            print("|0|1|2|3|4|5|6|", bpc, bwf);
            string row = "";
            // write row for row
            for (int y = 0; y < 6; y++)
            {
                // seperator between rows
                print("+-+-+-+-+-+-+-+", bpc, bwf);
                for (int x = 0; x < 7; x++)
                {
                    // append each line
                    row += "|" + players[board[y, x]];
                }
                // finalizerow
                row += "|";
                print(row, bpc, bwf);
                // reset row
                row = "";
            }
            // finally print last row
            print("+-+-+-+-+-+-+-+", bpc, bwf);
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
                    playBoard[y, x] = 0;
                }
            }
        }

        /// <summary>
        /// Sets the first free value in the given column x to the given player.
        /// WARINING: no check whether play is valid or not
        /// </summary>
        /// <param name="player">player currently playing</param>
        /// <param name="x">column the player wants to play</param>
        public static void play(int player, int x)
        {
            // store played position fpr further use
            lastPlayedPos[0] = getFirstEmpty(x,playBoard); //y
            lastPlayedPos[1] = x; //x
            // set the player on the board
            playBoard[lastPlayedPos[0], lastPlayedPos[1]] = player;
            // draw the new board
            drawBoard(playBoard);
        }
        #region wincheck
        /// <summary>
        /// checks whether the player won with his last turn (4 in a row donwards)
        /// </summary>
        /// <param name="player">player who did the last turn</param>
        /// <returns>true if won, false if not</returns>
        public static bool checkWinDown(int player, int[,] board, int[] lastPlayedPos)
        {
            if (lastPlayedPos[0] > 2)
            {
                // can not be won downwards
                return false;
            }
            // all 4 downwards have to match
            for (int i = 0; i < 4; i++)
            {
                // if one does not; can't be won this way
                if (board[lastPlayedPos[0] + i, lastPlayedPos[1]] != player)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// checks whether the player won with his last turn (4 in a row sideways)
        /// </summary>
        /// <param name="player">player who did the last turn</param>
        /// <returns>true if won, false if not</returns>
        public static bool checkWinSideways(int player, int[,] board, int[] lastPlayedPos)
        {
            // 4 cases: 2 at the end of line, 2 in middle
            for (int ncase = 1; ncase < 5; ncase++)
            {
                switch (ncase)
                {
                    case 1:
                        // pos at left most corner
                        if (lastPlayedPos[1] > 3)
                        {
                            // can't be
                        }
                        else
                        {
                            if (board[lastPlayedPos[0], lastPlayedPos[1]+1] == player && board[lastPlayedPos[0], lastPlayedPos[1]+2] == player && board[lastPlayedPos[0], lastPlayedPos[1]+3] == player)
                            {
                                return true;
                            }
                        }
                        break;
                    case 2:
                        // right most
                        if (lastPlayedPos[1] < 3)
                        {

                        }
                        else
                        {
                            if (board[lastPlayedPos[0], lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0], lastPlayedPos[1] - 2] == player && board[lastPlayedPos[0], lastPlayedPos[1] - 3] == player)
                            {
                                return true;
                            }
                        }
                        break;
                    case 3:
                        //middle left
                        if (lastPlayedPos[1] == 0 || lastPlayedPos[1] >4)
                        {
                        }
                        else
                        {
                            if (board[lastPlayedPos[0], lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0], lastPlayedPos[1] +1] == player && board[lastPlayedPos[0], lastPlayedPos[1] +2] == player)
                            {
                                return true;
                            }
                        }
                        break;
                    case 4:
                        // middle right
                        if (lastPlayedPos[1] == 6 || lastPlayedPos[1] < 2)
                        {
                        }
                        else
                        {
                            if (board[lastPlayedPos[0], lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0], lastPlayedPos[1] + 1] == player && board[lastPlayedPos[0], lastPlayedPos[1] - 2] == player)
                            {
                                return true;
                            }
                        }
                        break;
                }

                
            }
            return false;
        }

        /// <summary>
        /// checks whether the player won with his last turn (4 in a row diagonal)
        /// </summary>
        /// <param name="player">player who did the last turn</param>
        /// <returns>true if won, false if not</returns>
        public static bool checkWinDiagonal(int player, int[,] board, int[] lastPlayedPos)
        {
            // 4 cases: 2 at the end of line, 2 in middle for each diagonal play
            for (int ncase = 1; ncase < 9; ncase++)
            {
                switch (ncase)
                {
                    case 1:
                        // pos at left most corner diagonal:\
                        if (lastPlayedPos[1] > 3 || lastPlayedPos[0]>2)
                        {

                        }
                        else
                        {
                            if (board[lastPlayedPos[0]+1, lastPlayedPos[1] + 1] == player && board[lastPlayedPos[0]+2, lastPlayedPos[1] + 2] == player && board[lastPlayedPos[0]+3, lastPlayedPos[1] + 3] == player)
                            {
                                return true;
                            }
                            
                        }
                        break;
                    case 2:
                        // right most \
                        if (lastPlayedPos[1] < 3 || lastPlayedPos[0] < 3)
                        {

                        }
                        else
                        {
                            if (board[lastPlayedPos[0]-1, lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0]-2, lastPlayedPos[1] - 2] == player && board[lastPlayedPos[0]-3, lastPlayedPos[1] - 3] == player)
                            {
                                return true;
                            }
                        }
                        break;
                    case 3:
                        //middle left \
                        if (lastPlayedPos[1] == 0 || lastPlayedPos[1] > 4 || lastPlayedPos[0] == 0 || lastPlayedPos[0] > 3)
                        {
                        }
                        else
                        {
                            if (board[lastPlayedPos[0]-1, lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0]+1, lastPlayedPos[1] + 1] == player && board[lastPlayedPos[0]+2, lastPlayedPos[1] + 2] == player)
                            {
                                return true;
                            }
                        }
                        break;
                    case 4:
                        // middle right \
                        if (lastPlayedPos[1] == 6 || lastPlayedPos[1] < 2 || lastPlayedPos[0] == 5 || lastPlayedPos[0] < 2)
                        {
                        }
                        else
                        {
                            if (board[lastPlayedPos[0]-1, lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0]+1, lastPlayedPos[1] + 1] == player && board[lastPlayedPos[0]-2, lastPlayedPos[1] - 2] == player)
                            {
                                return true;
                            }
                        }
                        break;
                    case 5:
                        // pos at left most corner /
                        if (lastPlayedPos[1] > 3 || lastPlayedPos[0] < 3)
                        {

                        }
                        else
                        {
                            if (board[lastPlayedPos[0] - 1, lastPlayedPos[1] + 1] == player && board[lastPlayedPos[0] - 2, lastPlayedPos[1] + 2] == player && board[lastPlayedPos[0] - 3, lastPlayedPos[1] + 3] == player)
                            {
                                return true;
                            }

                        }
                        break;
                    case 6:
                        // right most /
                        if (lastPlayedPos[1] < 3 || lastPlayedPos[0] > 2)
                        {

                        }
                        else
                        {
                            if (board[lastPlayedPos[0] + 1, lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0] + 2, lastPlayedPos[1] - 2] == player && board[lastPlayedPos[0] + 3, lastPlayedPos[1] - 3] == player)
                            {
                                return true;
                            }
                        }
                        break;
                    case 7:
                        //middle left /
                        if (lastPlayedPos[1] == 0 || lastPlayedPos[1] > 4 || lastPlayedPos[0] == 5 || lastPlayedPos[0] < 2)
                        {
                        }
                        else
                        {
                            if (board[lastPlayedPos[0] + 1, lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0] - 1, lastPlayedPos[1] + 1] == player && board[lastPlayedPos[0] - 2, lastPlayedPos[1] + 2] == player)
                            {
                                return true;
                            }
                        }
                        break;
                    case 8:
                        // middle right /
                        if (lastPlayedPos[1] == 6 || lastPlayedPos[1] < 2 || lastPlayedPos[0] == 0 || lastPlayedPos[0] > 3)
                        {
                        }
                        else
                        {
                            if (board[lastPlayedPos[0] + 1, lastPlayedPos[1] - 1] == player && board[lastPlayedPos[0] - 1, lastPlayedPos[1] + 1] == player && board[lastPlayedPos[0] + 2, lastPlayedPos[1] - 2] == player)
                            {
                                return true;
                            }
                        }
                        break;
                }


            }
            return false;
        }

        /// <summary>
        /// checks all win-possibilities for the player on the given board and the given last played position
        /// </summary>
        /// <param name="player">player who did the last turn</param>
        /// <param name="board">board to check</param>
        /// <param name="lastPlayedPos">last played position by "player"</param>
        /// <returns>true if won, false if not</returns>
        public static bool checkwin(int player, int[,] board, int[] lastPlayedPos)
        {
            return checkwin(player, board, lastPlayedPos, true);

        }

        public static bool checkwin(int player, int[,] board, int[] lastPlayedPos, bool bPrint)
        {
            // check all possibilities
            if (checkWinDown(player, board, lastPlayedPos))
            {
                if (bPrint)
                {
                    print("congrats player " + player + ", you won (down)!");
                }
                return true;
            }
            if (checkWinSideways(player, board, lastPlayedPos))
            {
                if (bPrint)
                {
                    print("congrats player " + player + ", you won (sideways)!");
                }
                return true;
            }
            if (checkWinDiagonal(player, board, lastPlayedPos))
            {
                if (bPrint)
                {
                    print("congrats player " + player + ", you won (diagonal)!");
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
            return checkwin(player, board, lastPlayedPos);
        }
        #endregion

        /// <summary>
        /// returns the y-value of first empty field on the board in the given column in the given board
        /// </summary>
        /// <param name="x">column</param>
        /// <param name="board">board to check the first free position in</param>
        /// <returns>y-value of the first empty field on the bord in the column, -1 if no empty field</returns>
        public static int getFirstEmpty(int x,int[,] board)
        {
            int y = 0;
            // while board empty
            while (board[y, x] == 0)
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
            return getFirstEmpty(x, playBoard);
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
                print("Sorry, did not understand you");
                print("Do you want to play again? - Y/N");
                res = Console.ReadLine();
            }
        }

        /// <summary>
        /// user input for the column he wants to play in his turn
        /// </summary>
        /// <param name="player">current player</param>
        /// <returns>column number of the column he wants to play</returns>
        public static int readColumn(int player)
        {
            int column = -1;
            while (column ==-1){
                drawBoard(playBoard);
                print("it is your turn, player " +player +"! \nwhere do you want to play?");
                string s = Console.ReadLine();
                if (s.Equals("m"))
                {
                    Console.Clear();
                    menu();
                    column = -1;
                    Console.ReadLine();
                }
                if (s.Equals("")) 
                {
                    print("you have to enter a Number between 0 and 6 and not nothing!");
                    print("press Enter to acknowledge");
                    column = -1;
                    Console.ReadLine();
                }
                else
                {
                    if (Int32.TryParse(s, out column))
                    {
                        if (column > 6 || column < 0)
                        {
                            print("you have to enter a Number between 0 and 6!");
                            print("press Enter to acknowledge");
                            column = -1;
                            Console.ReadLine();
                        }
                        else if (getFirstEmpty(column,playBoard) == -1)
                        {
                            print("can't play here, column full");
                            print("press Enter to acknowledge");
                            column = -1;
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        print("you have to enter a Number and not some nonsense!");
                        print("press Enter to acknowledge");
                        column = -1;
                        Console.ReadLine();
                    }
                }
            }
            return column;
        }

        /// <summary>
        /// copy the given board in a new instance
        /// </summary>
        /// <param name="boardToCopy"></param>
        /// <returns></returns>
        public static int[,] copyBoard(int[,] boardToCopy)
        {
            // create new board
            int[,] board = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };
            // copy all positions to the new board
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    board[y, x] = boardToCopy[y, x];
                }
            }
            // return board
            return board;
        }


        /// <summary>
        /// playing the menu of the game
        /// </summary>
        public static void menu()
        {
            int nChoice = 0;
            string sChoice = "";
            while (true)
            {
                print("");
                print("What do you want to do?");
                print("");
                print("");
                print("1 - 2 player");
                print("2 - play against COM - not yet implemented");
                print("3 - help");
                print("4 - exit menu");
                print("5 - exit game");
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
                    case 1:
                        Player1.PType = Player.PlayerType.HUMAN_PLAYER;
                        Player2.PType = Player.PlayerType.HUMAN_PLAYER;
                        return;
                        
                    case 2:
                        Player1.PType = Player.PlayerType.HUMAN_PLAYER;
                        Player2.PType = Player.PlayerType.MACHINE_PLAYER;
                        return;
                    case 3:
                        Console.Clear();
                        print("\n\nObject: Connect four of your checkers in a row while preventing your opponent \n     from doing the same. \n\n\n         - Milton Bradley, Connect Four \"Pretty Sneaky, Sis\" \n            television commercial, 1977");
                        print("\n\nDo so by typing the number of the column you want to play next");
                        print("and confirm your selection by pressing enter");
                        break;
                    case 4:
                        return;
                    case 5:
                        Console.Clear();
                        print("Bye!");
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
                Console.ReadLine();
                Console.Clear();
            }
        }

        public static void interpretArgs(string[] args)
        {
            /// -l; --log
            ///-h; --help
            ///-pc; --PrintConsole
            ///-ww
            ///-lw
            ///--iterationdepth
            for (int i=0; i < args.Length; i++)
            {

                if (args[i].Equals("-p1"))
                {
                    if (args[i+1].Equals("COM")){
                        Player1 = new Con4Bot(1, Convert.ToInt32(args[i + 4]), 2, playBoard);
                        Player1.WinWeight =  Convert.ToInt32(args[i + 2]);
                        Player1.LooseWeight = Convert.ToInt32(args[i + 3]);
                    }
                    else if (args[i + 1].Equals("HUM"))
                    {
                        Player1 = new Human(1, 2, playBoard);
                    }
                    else
                    {
                        print("player type " + args[i + 1] + " unknown");
                    }
                    print("player 1 is " + args[i+1]);
                }
                if (args[i].Equals("-p2"))
                {
                    if (args[i + 1].Equals("COM"))
                    {
                        Player2 = new Con4Bot(2, Convert.ToInt32(args[i + 4]), 1, playBoard);
                        Player2.WinWeight = Convert.ToInt32(args[i + 2]);
                        Player2.LooseWeight = Convert.ToInt32(args[i + 3]);
                    }
                    else if (args[i + 1].Equals("HUM"))
                    {
                        Player2 = new Human(2, 1, playBoard);
                    }
                    else
                    {
                        print("player type " + args[i + 1] + " unknown");
                    }
                    print("player 1 is " + args[i + 1]);
                }


                // detect logging

                if (args[i].Equals("-l") || args[i].Equals("--log"))
                {
                    print("now logging");
                }
                if (args[i].Equals("-h") || args[i].Equals("--help"))
                {
                    print("now helping");
                }
                if (args[i].Equals("-pc") || args[i].Equals("--PrintConsole"))
                {
                    print("printing console");
                }
                if (args[i].Equals("-ww"))
                {

                    print("Win Weigt = " + args[i+1]);
                }
                if (args[i].Equals("-lw"))
                {
                    print("loose weight" + args[i + 1]);
                }
                if (args[i].Equals("--iterationdepth"))
                {
                    print("it depth = " + args[i + 1]);
                }
            }
            //Console.ReadKey();
        }

    }
}

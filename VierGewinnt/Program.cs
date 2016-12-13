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
    /// 2) comment code
    /// 3) enable 2 bots  
    /// 4) logging to file
    /// </summary>
    public static class Program
    {

        /**
         * Playboard
         *   -> x 
         * | y
         * \/
         * 
         */
        private static int[,] playBoard = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };

        internal static void print(string s, bool bprintConsole, bool bwriteFile)
        {
            if (bprintConsole)
            {
                Console.WriteLine(s);
            }
            if (bwriteFile)
            {
                //File.AppendAllText(logPath, s + Environment.NewLine);
            }
        }

        private static bool p1IsBot = false;
        private static bool p2IsBot = false;
        private static string logPath = @"C:\Users\DEU216269\Desktop\log.txt";

        // playerarray
        private static string[] players = {" ","x","o"};

        // y,x dafuq did i do?
        private static int[] lastPlayedPos = { 0, 0 };

        public static void print(string s, bool bprintConsole)
        {
            print(s, bprintConsole, false);
        }

        public static void print(string s)
        {
            print(s, true,false);
        }



        /// <summary>
        /// draws the board of connect four
        /// </summary>
        public static void drawBoard(int[,] board)
        {
            bool bpc = true;
            bool bwf= true;
            // clear
            Console.Clear();
            print("connect four:", bpc, bwf);
            // write colum-header
            print("|0|1|2|3|4|5|6|", bpc, bwf);
            string row = "";
            // write row for row
            for (int y = 0; y < 6; y++) { 
                print("+-+-+-+-+-+-+-+", bpc, bwf);
                for (int x = 0; x < 7; x++)
                {
                    row += "|" + players[board[y, x]];
                    //Console.Write("|" + players[playBoard[y, x]]);
                }
                row += "|";
                print(row, bpc, bwf);
                row = "";
            }
            print("+-+-+-+-+-+-+-+", bpc, bwf);
        }
        
        /// <summary>
        /// clears the board of connect four
        /// </summary>
        public static void resetBoard()
        {
            // set every value to zero
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    playBoard[x, y] = 0;
                }
            }
        }

        /// <summary>
        /// sets the first free value in the given column x to the given player
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
            bool b1;
            bool b2;
            bool b3;
            bool b4;
            // 4 cases: 2 at the end of line, 2 in middle
            for (int ncase = 1; ncase < 9; ncase++)
            {
                switch (ncase)
                {
                    case 1:
                        b1 = lastPlayedPos[1] > 3;
                        b2 = lastPlayedPos[0] > 2;
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
                        b1 = lastPlayedPos[1] < 3;
                        b2 = lastPlayedPos[0] < 3;
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
                        b1 = lastPlayedPos[1] == 0;
                        b2 = lastPlayedPos[1] > 4;
                        b3 = lastPlayedPos[0] == 0;
                        b4 = lastPlayedPos[0] > 3;
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
                        b1 = lastPlayedPos[1] == 6;
                        b2 = lastPlayedPos[1] < 2;
                        b3 = lastPlayedPos[0] == 5;
                        b4 = lastPlayedPos[0] < 2;
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
                        b1 = lastPlayedPos[1] > 3;
                        b2 = lastPlayedPos[0] < 3;
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
                        b1 = lastPlayedPos[1] < 3;
                        b2 = lastPlayedPos[0] > 2;
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
                        b1 = lastPlayedPos[1] == 0;
                            b2 = lastPlayedPos[1] > 4;
                            b3 = lastPlayedPos[0] == 5;
                            b4 = lastPlayedPos[0] < 2;
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
                        b1 = lastPlayedPos[1] == 6;
                        b2 = lastPlayedPos[1] < 2;
                        b3 = lastPlayedPos[0] == 0;
                        b4 = lastPlayedPos[0] > 3;
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
        /// checks all win-possibilities for the player
        /// </summary>
        /// <param name="player">player who did the last turn</param>
        /// <returns>true if won, false if not</returns>
        public static bool checkwin(int player, int[,] board, int[] lastPlayedPos)
        {
            // check all possibilities
            if (checkWinDown(player,board,lastPlayedPos))
            {
                //print("congrats player " + player + ", you won (down)!");

                return true ;
            }
            if (checkWinSideways(player, board, lastPlayedPos))
            {
                
                //print("congrats player " + player + ", you won (sideways)!");
                return true;
            }
            if (checkWinDiagonal(player, board, lastPlayedPos))
            {

                //print("congrats player " + player + ", you won (diagonal)!");
                return true;
            }
            return false;
        }

        public static bool checkwin(int player, int[,] board)
        {
            return checkwin(player, board, lastPlayedPos);
        }
        #endregion

        /// <summary>
        /// returns the y-value of first empty field on the board in the given column
        /// </summary>
        /// <param name="x">column</param>
        /// <returns>y-value of the first empty field on the bord in the column</returns>
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
        /// <returns>y-value of the first empty field on the bord in the column</returns>
        public static int getFirstEmpty(int x)
        {
            return getFirstEmpty(x, playBoard);
        }


        /// <summary>
        /// main routine
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //File.Delete(logPath);
            Con4Bot bot = null ;
            bool won = false;
            bool replay = false;
            int currPlayer = 1;
            int difficulty = 5;
            print("");
            print("Hello to Connect Four!");
            menu();
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
                    print("Tie!");
                    won = true;
                    replay = playAgain();
                }
                // play
                if (p2IsBot && currPlayer == 2)
                {
                    bot = null;
                    bot = new Con4Bot(currPlayer, difficulty, 1, playBoard);
                    bot.play();

                }
                else
                {
                    play(currPlayer, readColumn(currPlayer));
                }
                roundcounter++;
                won = checkwin(currPlayer,playBoard);
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
                    replay = playAgain();
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
                sChoice = Console.ReadLine();
                Int32.TryParse(sChoice, out nChoice);
                switch (nChoice)
                {
                    case 1:
                        p1IsBot = false;
                        p2IsBot = false;
                        return;
                        
                    case 2:
                        p1IsBot = false;
                        p2IsBot = true;
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






    }
    
}

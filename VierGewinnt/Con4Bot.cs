using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VierGewinnt
{
    public class Con4Bot
    {
        private static bool bprint = true;
        private static bool writeFile = true;
        public static bool bAccurateLog = false;
        /// <summary>
        /// current board stored by bot
        /// </summary>
        private int[,] localBoard = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };
        private int playerNO;
        private int opponent;
        private int winWeight = 1;
        private int looseWeight = -100;
        private int difficulty = 5;
        private static int[] lastPlayedPos = { 0, 0 };
        private int best_column = 0;

        /// <summary>
        /// constructor for the bot 
        /// </summary>
        /// <param name="player">player the bot shall be</param>
        /// <param name="difficulty">difficulty the bot should have</param>
        /// <param name="opponent">opponent of the bot</param>
        /// <param name="board">board to play on</param>
        public Con4Bot(int player, int difficulty, int opponent, int[,] board)
        {
            playerNO = player;
            this.difficulty = difficulty;
            this.opponent = opponent;
            localBoard = copyBoard(board);
        }
        /// <summary>
        /// Constructor for the bot
        /// </summary>
        /// <param name="player">player the bot shall be</param>
        /// <param name="difficulty">difficulty the bot should have</param>
        /// <param name="opponent">opponent of the bot</param>
        /// <param name="board">board to play on</param>
        /// <param name="looseWeight">loosing weight of the bot</param>
        /// <param name="winWeight">winning weight of the bot</param>
        public Con4Bot(int player, int difficulty, int opponent, int[,] board, int looseWeight, int winWeight)
        {
            // set all local vars
            playerNO = player;
            this.difficulty = difficulty;
            this.opponent = opponent;
            localBoard = copyBoard(board);
            this.looseWeight = looseWeight;
            this.winWeight = winWeight;
        }

        /// <summary>
        /// prints the given string, using <seealso cref="Program.print(string)"/>
        /// </summary>
        private static void print(string s)
        {
            Program.print(s, bprint, writeFile);
        }
        /// <summary>
        /// prints the given string, using <seealso cref="Program.print(string, bool, bool)"/>
        /// </summary>
        private static void print(string s, bool bprint, bool writeFile)
        {
            Program.print(s, bprint, writeFile);
        }

        /// <summary>
        /// copy the given board in a new instance
        /// </summary>
        /// <param name="boardToCopy"></param>
        /// <returns></returns>
        private int[,] copyBoard(int[,] boardToCopy)
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
        /// play of the bot
        /// </summary>
        public void play()
        {
            // init array of scores for different possibilities
            int[] scores = { 0, 0, 0, 0, 0, 0, 0 };
            // copy the board
            int[,] board = copyBoard(localBoard);

            Console.Write("bot is thinking.");
            // check each possibility
            for (int y = 0; y < 7; y++)
            {
                board = copyBoard(localBoard);
                // check reseted board
                scores[y] = checkBest(board, y, difficulty, playerNO);
            }
            // check the best possibility. if valid, do it, otherwise change column
            while (Program.getFirstEmpty(best_column) == -1)
            {
                best_column += 1;
                if (best_column == 6)
                {
                    best_column = 0;
                }
            }
            // play the game 
            Program.play(playerNO, best_column);
        }

        /// <summary>
        /// sets the first free value in the given column x to the given player
        /// </summary>
        /// <param name="player">player currently playing</param>
        /// <param name="x">column the player wants to play</param>
        public int[,] playLocal(int player, int x, int[,] board)
        {
            int[,] pBoard = copyBoard(board);
            if (bAccurateLog) {
                print("playLocal: p= " + player + "; x= " + x);
            }
            // store played position fpr further use
            lastPlayedPos[0] = Program.getFirstEmpty(x, pBoard); //y
            lastPlayedPos[1] = x; //x
            // set the player on the board
            pBoard[lastPlayedPos[0], lastPlayedPos[1]] = player;
            //Program.drawBoard(pBoard);
            return pBoard;

        }

        /// <summary>
        /// prints the given score formatted to the depth
        /// </summary>
        /// <param name="score">score to print</param>
        /// <param name="depth">depth</param>
        private void printScore(int score, int depth)
        {
            // |(-)n:score
            string s = "|";
            for (int i = difficulty; i > depth; i--)
            {
                s += "-";
            }
            s += ":" +score;
            print(s,false,false);
        }

        /// <summary>
        /// checks the best solution for a play by the bot
        /// </summary>
        /// <param name="board">board to play on</param>
        /// <param name="column">column to play</param>
        /// <param name="depth">how many more iterations should be done</param>
        /// <param name="currPlayer">player whose turn it is</param>
        /// <returns></returns>
        private int checkBest(int[,] board, int column, int depth, int currPlayer)
        {

            // init score for node
            int score = -100; // no move is pretty bad
            // store different things at beginning of testing node
            int[,] preBoard = copyBoard(board);
            int prePlayer = currPlayer;
            int[,] virtBoard = copyBoard(board);
            int[] preLastPlayed = { lastPlayedPos[0], lastPlayedPos[1] };

            // depth not reached, otherwise return default score; do not play
            if (depth == 0)
            {
                return score;
            }
            // if no valid play, return default score; do not play
            if (Program.getFirstEmpty(column, board) == -1)
            {
                return score;
            }
            // log
            if (bAccurateLog) { 
                print("checkBest: p= " + currPlayer + "; x= " + column + "; d= " + depth);
            }
            // valid -> play
            virtBoard = playLocal(prePlayer, column, virtBoard);
            // check for win or loose
            if (Program.checkwin(playerNO, virtBoard, lastPlayedPos))
            {
                //Program.drawBoard(board);
                score = winWeight*depth;
                printScore(score, depth);
                //print("BOTWIN");
                return score;
            }
            else if (Program.checkwin(opponent, virtBoard, lastPlayedPos))
            {
                //Program.drawBoard(board);
                score = looseWeight*depth;
                printScore(score, depth);
                // make sure to enter here if loss imminent
                best_column = column;
                //print("BOTLOOSE");
                return score;
            }
            int nply;
            // swap playerNo
            if (currPlayer == opponent)
            {
                nply = playerNO;
            }
            else
            {
                nply = opponent;
            }
            // play the other variants
            for (int i = 0; i < 7; i++)
            {
                // check the score of the different plays
                int thisscore = checkBest(virtBoard, i, depth - 1, nply);
                //
                if (thisscore > score && nply == opponent)
                {
                    score = thisscore;
                    
                }
                if (thisscore < score && nply == playerNO)
                {
                    score = thisscore;
                    best_column = i;
                }
            }

            // reset board
            board = copyBoard(preBoard);
            currPlayer = prePlayer;
            lastPlayedPos[0] = preLastPlayed[0];
            lastPlayedPos[1] = preLastPlayed[1];
            printScore(score, depth);
            // return score of node
            return score;
        }
    }
}

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

        private static void print(string s)
        {
            Program.print(s, bprint, writeFile);
        }
        private static void print(string s, bool bprint, bool writeFile)
        {
            Program.print(s, bprint, writeFile);
        }


        private int[,] copyBoard(int[,] boardToCopy)
        {
            int[,] board = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };
            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    board[x, y] = boardToCopy[x, y];
                }
            }
            return board;
        }

        private int[,] localBoard = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };
        private int playerNO;
        private int opponent;
        private int winWeight = 1;
        private int looseWeight = -100;
        private int difficulty = 5;
        private static int[] lastPlayedPos = { 0, 0 };

        public Con4Bot(int player, int difficulty, int opponent, int[,] board)
        {
            playerNO = player;
            this.difficulty = difficulty;
            this.opponent = opponent;
            localBoard = copyBoard(board);
        }
        public Con4Bot(int player, int difficulty, int opponent, int[,] board, int looseWeight, int winWeight)
        {
            playerNO = player;
            this.difficulty = difficulty;
            this.opponent = opponent;
            localBoard = copyBoard(board);
            this.looseWeight = looseWeight;
            this.winWeight = winWeight;
        }




        public void play()
        {
            int[] scores = { 0, 0, 0, 0, 0, 0, 0 };
            int[,] board = copyBoard(localBoard);
            //int rand = new Random().Next(0, 6);
            Console.Write("bot is thinking.");
            for (int i = 0; i < 7; i++)
            {
                board = copyBoard(localBoard);
                // check reseted board
                scores[i] = checkBest(board, i, difficulty, playerNO);
            }
            while (Program.getFirstEmpty(best_column) == -1)
            {
                best_column += 1;
                if (best_column == 6)
                {
                    best_column = 0;
                }
            }
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
            //print("pl, p: " + player + " x: " + x);
            // store played position fpr further use
            lastPlayedPos[0] = Program.getFirstEmpty(x, pBoard); //y
            lastPlayedPos[1] = x; //x
            // set the player on the board
            pBoard[lastPlayedPos[0], lastPlayedPos[1]] = player;
            //Program.drawBoard(pBoard);
            return pBoard;

        }


        private int best_column = 0;

        private void printScore(int score, int depth)
        {
            string s = "|-";
            for (int i = difficulty; i > depth; i--)
            {
                s += "-";

            }
            s += ":" +score;
            print(s,false,false);
        }


        private int checkBest(int[,] board, int column, int depth, int currPlayer)
        {
            
            // init score for node
            int score = -100; // no move worse than loose move
            // store board at beginning of testing node
            int[,] preBoard = copyBoard(board);
            int prePlayer = currPlayer;
            int[,] virtBoard = copyBoard(board);
            int[] preLastPlayed = { lastPlayedPos[0], lastPlayedPos[1] };

            // depth not reached, otherwise return default score; do not play
            if (depth == 0)
            {
                return score;
            }
            // if no valid play, return 0
            if (Program.getFirstEmpty(column, board) == -1)
            {
                return score;
            }
            // log
            //print("cb, p: " + currPlayer + " x: " + column + " d: " + depth);
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

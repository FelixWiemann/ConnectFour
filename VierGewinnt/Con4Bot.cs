using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VierGewinnt
{
    public class Con4Bot:Player
    {
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
        public Con4Bot(int nPlayerNo, int difficulty, int nOpponentNo, int[,] board) : base(nPlayerNo, nOpponentNo, board)
        {
            this.difficulty = difficulty;
            LocalBoard = Program.copyBoard(board);
            PType = PlayerType.MACHINE_PLAYER;
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
        public Con4Bot(int nPlayerNo, int difficulty, int nOpponentNo, int[,] board, int looseWeight, int winWeight) : base(nPlayerNo, nOpponentNo, board)
        {
            this.difficulty = difficulty;
            LocalBoard = Program.copyBoard(board);
            this.looseWeight = looseWeight;
            this.winWeight = winWeight;
            PType = PlayerType.MACHINE_PLAYER;
        }
        
        
        /// <summary>
        /// play of the bot
        /// </summary>
        public override void play()
        {
            // init array of scores for different possibilities
            int[] scores = { 0, 0, 0, 0, 0, 0, 0 };
            // copy the board
            int[,] board = Program.copyBoard(LocalBoard);

            Console.Write("bot is thinking");
            // check each possibility
            for (int y = 0; y < 7; y++)
            {
                board = Program.copyBoard(LocalBoard);
                // check reseted board
                scores[y] = checkBest(board, y, difficulty, nPlayerNo);
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
            Program.play(nPlayerNo, best_column);
        }

        /// <summary>
        /// sets the first free value in the given column x to the given player
        /// </summary>
        /// <param name="player">player currently playing</param>
        /// <param name="x">column the player wants to play</param>
        public int[,] playLocal(int player, int x, int[,] board)
        {
            int[,] pBoard = Program.copyBoard(board);
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
            int[,] preBoard = Program.copyBoard(board);
            int prePlayer = currPlayer;
            int[,] virtBoard = Program.copyBoard(board);
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
            if (Program.checkwin(nPlayerNo, virtBoard, lastPlayedPos,false))
            {
                //Program.drawBoard(board);
                score = winWeight*depth;
                printScore(score, depth);
                //print("BOTWIN");
                return score;
            }
            else if (Program.checkwin(nOpponentNo, virtBoard, lastPlayedPos,false))
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
            if (currPlayer == nOpponentNo)
            {
                nply = nPlayerNo;
            }
            else
            {
                nply = nOpponentNo;
            }
            // play the other variants
            for (int i = 0; i < 7; i++)
            {
                // check the score of the different plays
                int thisscore = checkBest(virtBoard, i, depth - 1, nply);
                //
                if (thisscore > score && nply == nOpponentNo)
                {
                    score = thisscore;
                    
                }
                if (thisscore < score && nply == nPlayerNo)
                {
                    score = thisscore;
                    best_column = i;
                }
            }

            // reset board
            board = Program.copyBoard(preBoard);
            currPlayer = prePlayer;
            lastPlayedPos[0] = preLastPlayed[0];
            lastPlayedPos[1] = preLastPlayed[1];
            printScore(score, depth);
            // return score of node
            return score;
        }
    }
}

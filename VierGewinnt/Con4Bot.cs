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
        private int loseWeight = -10;
        private int difficulty = 0;
        private static int[] lastPlayedPos = { 0, 0 };

        public Con4Bot(int player, int difficulty, int opponent, int[,] board)
        {
            playerNO = player;
            this.difficulty = difficulty;
            this.opponent = opponent;
            localBoard = copyBoard(board);
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
            /*
            int maxValue;
            int maxIndex;
            int count = 0;
            do
            {
                maxValue = scores.Max();
                maxIndex = scores.ToList().IndexOf(maxValue);
                scores[maxIndex] = -33000000;
                count++;
            } while (Program.getFirstEmpty(maxIndex, board) == -1 && count < 50);*/



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


        private int best_column = 6;

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

            if (depth ==  1)
            {
                //Console.Write(".");
            }

            // depth not reached, otherwise return 0; do not play
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
                score = loseWeight*depth*10;
                printScore(score, depth);
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


        /*
        private int checkBesta(int[,] board,int column, int depth, int currPlayer)
        {
            print("cb, p: "+currPlayer +" x: "+column+" d: "+depth);
            // play
            int[,] preboard = board;
            // swap players for next step
            if (currPlayer == opponent)
            {
                currPlayer = playerNO;
            }
            else
            {
                currPlayer = opponent;
            }

            int score = 0;
            // check every possibility
            for (int i = 0; i < 7; i++)
            {
                board = copyBoard(preboard);
                board = playLocal(currPlayer, column, board);
                // only if valid entry
                if (Program.getFirstEmpty(i, board) != -1)
                {
                    
                    // return 0, if depth of recursion is reached and no winner of current 
                    if (depth != 0)
                    {
                        // check whether bot would win or opponent
                        if (Program.checkwin(playerNO, board, lastPlayedPos))
                        {
                           
                            score += winWeight;
                            print("BOTWIN");
                            return score;
                        }
                        else if (Program.checkwin(opponent, board,lastPlayedPos))
                        {
                            // lost score -1
                            score += loseWeight;
                            print("BOTLOOSE");
                            return score;
                        }
                        
                        // not won or lost -> next player
                        score  += checkBest(board,i, depth - 1, currPlayer);
                    }
                    else
                    {
                        
                    }
                }
                
            }
            //print("s: "+score +" d: "+ depth);
            return score; 
            }*/

    }
}

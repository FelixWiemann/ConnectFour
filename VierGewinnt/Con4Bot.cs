﻿using ConnectFour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VierGewinnt
{
    public class Con4Bot:Player
    {
        private int winWeight = 500;
        private int looseWeight = -500;
        public int nDifficulty = 5;
        private int[] aSCORES_NOT_WIN = { 0, 1, 5, 10 };
        private int best_column = 0;
        /// <summary>
        /// constructor for the bot 
        /// </summary>
        /// <param name="player">player the bot shall be</param>
        /// <param name="difficulty">difficulty the bot should have</param>
        /// <param name="opponent">opponent of the bot</param>
        /// <param name="board">board to play on</param>
        public Con4Bot(int nPlayerNo, int difficulty, int nOpponentNo, Board board) : base(nPlayerNo, nOpponentNo, board)
        {
            this.nDifficulty = difficulty;
            LocalBoard = new Board(board);
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
        public Con4Bot(int nPlayerNo, int difficulty, int nOpponentNo, Board board, int looseWeight, int winWeight) : base(nPlayerNo, nOpponentNo, board)
        {
            this.nDifficulty = difficulty;
            LocalBoard = new Board(board);
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
            int[] lScores = { 0, 0, 0, 0, 0, 0, 0 };
            // copy the board
            Board board = new Board(LocalBoard);

            Console.Write("bot is thinking");
            // check each possibility
            for (int y = 0; y < 7; y++)
            {
                board = new Board(LocalBoard);
                // check reseted board
                // TODO run each in own thread to speed things up
                lScores[y] = checkBest(board, y, nDifficulty, nPlayerNo);
            }
            //Program.print("p " + nPlayerNo, false, true);
            //Program.print("0" + ": " + lScores[0] + "|" + lScores[1] + "|" + lScores[2] + "|" + lScores[3] + "|" + lScores[4] + "|" + lScores[5] + "|" + lScores[6] + "|", false, true);
            int maxValue = lScores.Max();
            int maxIndex = lScores.ToList().IndexOf(maxValue);
            best_column = maxIndex;
            // check the best possibility. if valid, do it, otherwise change column
            // TODO multiple same values -> choose random one to make things more intersting
            while (LocalBoard.getFirstEmpty(best_column) == -1)
            {
                best_column += 1;
                if (best_column == 7)
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
        public Board playLocal(int player, int x, Board board)
        {
            Board pBoard = new Board(board);
            if (bAccurateLog) {
                //print("playLocal: p= " + player + "; x= " + x);
            }
            // set the player on the board
            pBoard.play(player, x, false);
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
            for (int i = nDifficulty; i > depth; i--)
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
        private int checkBest(Board board, int column, int depth, int currPlayer)
        {

            // init score for node
            int score = 0; // no move is pretty bad
            // store different things at beginning of testing node
            Board preBoard = new Board(board);
            int prePlayer = currPlayer;
            Board virtBoard = new Board(board);

            // depth not reached, otherwise return default score; do not play
            if (depth == 0)
            {
                return score;
            }
            // if no valid play, return default score; do not play
            if (board.getFirstEmpty(column) == -1)
            {
                return score;
            }
            
            // valid -> play
            virtBoard = playLocal(prePlayer, column, virtBoard);
            // check for win or loose
            if (virtBoard.checkwin(nPlayerNo,false))
            {
                //Program.drawBoard(board);
                score = winWeight*depth;
                printScore(score, depth);
                if (bAccurateLog)
                {
                    print("found possible win for " + nPlayerNo);
                }
                //print("BOTWIN");
                return score;
            }
            else if (virtBoard.checkwin(nOpponentNo,false))
            {
                //Program.drawBoard(board);
                score = looseWeight*depth;
                printScore(score, depth);
                // make sure to enter here if loss imminent
                best_column = column;
                if (bAccurateLog)
                {
                    print("found possible win for " + nOpponentNo);
                }
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
            int playerFac = 0;
            if (currPlayer == nOpponentNo)
            {
                playerFac = -1;
            }
            else
            {
                playerFac = 1;
            }

            for (int i = 3; i > 1; i--)
            {
                if (LocalBoard.checkForCountDown(currPlayer, i))
                {
                    //Program.drawBoard(board);
                    // the deeper in the win is the less acceptable is a win
                    score = aSCORES_NOT_WIN[i] * depth * currPlayer;
                    printScore(score, depth);
                    //print("BOTWIN");
                    i = 1;
                }
                if (LocalBoard.checkForCountDiagonal(currPlayer, i))
                {
                    //Program.drawBoard(board);
                    // the deeper in the win is the less acceptable is a win
                    score = aSCORES_NOT_WIN[i] * depth * currPlayer;
                    printScore(score, depth);
                    //print("BOTWIN");
                    i = 1;
                }
                if (LocalBoard.checkForCountSideways(currPlayer, i))
                {
                    //Program.drawBoard(board);
                    // the deeper in the win is the less acceptable is a win
                    score = aSCORES_NOT_WIN[i] * depth * currPlayer;
                    printScore(score, depth);
                    //print("BOTWIN");
                    i = 1;
                }
            }
            int[] scores = { 0, 0, 0, 0, 0, 0, 0 };
            int lDeeperScore = 0;
            // play the other variants
            for (int i = 0; i < 7; i++)
            {
                // check the score of the different plays 
                // TODO if depth > threshold, run in async in different thread to speed up the process
                scores[i] = checkBest(virtBoard, i, depth - 1, nply) * depth;
            }
            if (nply == nOpponentNo)
            {
                lDeeperScore = scores.Min() * depth;  
            }
            if (nply == nPlayerNo)
            {
                lDeeperScore = scores.Max() * depth;  
            }

            score = lDeeperScore + score;
            // log
            if (bAccurateLog)
            {
                print(indent(depth) + "CB: p= " + currPlayer + "; x= " + column + "; d= " + depth + "; score= " + score + "; deep= "+ lDeeperScore);
            }
            //          Program.print("p "+ currPlayer,false,true);
            //          Program.print(depth + ": " + scores[0] + "|" + scores[1] + "|" + scores[2] + "|" + scores[3] + "|" + scores[4] + "|" + scores[5] + "|" + scores[6] + "|", false, true);
            // reset board
            board = new Board(preBoard);
            currPlayer = prePlayer;
            printScore(score, depth);
            // return score of node
            return score;
        }

        private string indent(int depth)
        {
            string s = "";
            for (int i = this.nDifficulty; i > nDifficulty-depth; i--)
            {
                s += "  ";
            }
            return s;
        }
    }
}

﻿using System;
using System.IO;
using ConnectFour;

namespace VierGewinnt
{
    public abstract class Player
    {
        /// <summary>
        /// file name of logfile
        /// </summary>
        private string sFileName = @"log.txt";
        /// <summary>
        /// file path of logfile
        /// </summary>
        private string sFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Connect4\";
        /// <summary>
        /// connects file name and path
        /// </summary>
        private string sFullLogFilePath; 
        // datas for logging
        public bool bPrint = true;
        public bool bWriteFile = true;
        public bool bAccurateLog = false;

        // player&opponent info
        public int nPlayerNo = 0;
        public int nOpponentNo = 0;
        private PlayerType pType;

        //
        private int nIterationDepth = 0;
        private int nWinWeight = 0;
        private int nLooseWeight = 0;

        public int IterationDepth
        {
            get
            {
                return nIterationDepth;
            }
            set
            {
                nIterationDepth = value;
            }
        }
        public int LooseWeight
        {
            get
            {
                return nLooseWeight;
            }
            set
            {
                nLooseWeight = value;
            }
        }
        public int WinWeight
        {
            get
            {
                return nWinWeight;
            }
            set
            {
                nWinWeight = value;
            }
        }



        /// <summary>
        /// current board stored by bot
        /// </summary>
        private Board localBoard = new Board();

        public Board LocalBoard
        {
            get
            {
                return localBoard;
            }

            set
            {
                localBoard = value;
            }
        }

        public PlayerType PType
        {
            get
            {
                return pType;
            }

            set
            {
                pType = value;
            }
        }

        public void setLoggingData(bool bPrint, bool bWriteFile, bool bAccurateLog)
        {
            this.bPrint = bPrint;
            this.bWriteFile = bWriteFile;
            this.bAccurateLog = bAccurateLog;
        }

        public void setLogPath(string path)
        {
            sFullLogFilePath = path;
        }
       
     


        public enum PlayerType { HUMAN_PLAYER, MACHINE_PLAYER };
        public Player(int nPlayerNo, int nOpponentNo, bool bPrint, bool bWriteFile)
        {
            sFullLogFilePath = sFilePath + sFileName;
            this.nPlayerNo = nPlayerNo;
            this.bPrint = bPrint;
            this.bWriteFile = bWriteFile;
            this.nOpponentNo = nOpponentNo;
        }
        public Player(int nPlayerNo, int nOpponentNo, Board board)
        {
            sFullLogFilePath = sFilePath + sFileName;
            this.nPlayerNo = nPlayerNo;
            this.bPrint = true;
            this.bWriteFile = true;
            this.nOpponentNo = nOpponentNo;
            this.LocalBoard = new Board(board);
        }



        public void print(string s)
        {
            print(s, bPrint, bWriteFile);
        }
        public void print(string s, bool bPrintOnConsole)
        {
            print(s, bPrintOnConsole, bWriteFile);
        }
        object __lockObj = new object();
        public void print(string s, bool bPrintOnConsole, bool bWriteFile)
        {
            if (bPrintOnConsole)
            {
                Console.WriteLine(s);
            }
            if (bWriteFile)
            {
                lock (__lockObj)
                {
                    File.AppendAllText(sFullLogFilePath, s + Environment.NewLine);
                }
            }
        }

        public abstract void play();
    }
}

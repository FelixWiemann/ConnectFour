using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// current board stored by bot
        /// </summary>
        private int[,] localBoard = { { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0 } };

        public int[,] LocalBoard
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

       

        public enum PlayerType { HUMAN_PLAYER, MACHINE_PLAYER };
        public Player(int nPlayerNo, int nOpponentNo, bool bPrint, bool bWriteFile)
        {
            sFullLogFilePath = sFilePath + sFileName;
            this.nPlayerNo = nPlayerNo;
            this.bPrint = bPrint;
            this.bWriteFile = bWriteFile;
            this.nOpponentNo = nOpponentNo;
        }
        public Player(int nPlayerNo, int nOpponentNo, int[,] board)
        {
            sFullLogFilePath = sFilePath + sFileName;
            this.nPlayerNo = nPlayerNo;
            this.bPrint = true;
            this.bWriteFile = true;
            this.nOpponentNo = nOpponentNo;
            this.LocalBoard = Program.copyBoard(board);
        }



        public void print(string s)
        {
            print(s, bPrint, bWriteFile);
        }
        public void print(string s, bool bPrintOnConsole)
        {
            print(s, bPrintOnConsole, bWriteFile);
        }
        public void print(string s, bool bPrintOnConsole, bool bWriteFile)
        {
            if (bPrintOnConsole)
            {
                Console.WriteLine(s);
            }
            if (bWriteFile)
            {
                File.AppendAllText(sFullLogFilePath, s + Environment.NewLine);
            }
        }

        public abstract void play();
    }
}

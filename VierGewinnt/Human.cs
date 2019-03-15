using ConnectFour;
using System;

namespace VierGewinnt
{
    public class Human:Player
    {
        
        public Human(int nPlayerNo, int nOpponentNo, Board board) : base(nPlayerNo, nOpponentNo, board)
        {
            PType = PlayerType.HUMAN_PLAYER;
        }

        public override void play()
        {
            Program.play(nPlayerNo, readColumn(nPlayerNo));
        }

        /// <summary>
        /// user input for the column he wants to play in his turn
        /// </summary>
        /// <param name="player">current player</param>
        /// <returns>column number of the column he wants to play</returns>
        public int readColumn(int player)
        {
            int column = -1;
            while (column == -1)
            {
                LocalBoard.drawBoard();
                print("it is your turn, player " + player + "! \nwhere do you want to play?");
                string s = Console.ReadLine();
                if (s.Equals("m"))
                {
                    Console.Clear();
                    Program.menu();
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
                        else if (LocalBoard.getFirstEmpty(column) == -1)
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
    }
}

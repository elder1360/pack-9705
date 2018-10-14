using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Figgle;
using System.IO;

namespace XO
{
    class Program
    {
        static void Main(string[] args)
        {
            string runGame = "y";
            while (runGame=="y")
            {
                bool choosePlayer = true;
                GameBoard gameBoard = new GameBoard();
                int ax = 0, ay = 0;
                gameBoard.DrawGameBoard(ax, ay);
                while (!draw(gameBoard))
                {
                    ConsoleKey key = Console.ReadKey().Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            if (ay > 0 && ay < 3)
                            {
                                gameBoard.DrawGameBoard(ax, --ay);
                            }
                            continue;
                        case ConsoleKey.DownArrow:
                            if (ay >= 0 && ay < 2)
                            {
                                gameBoard.DrawGameBoard(ax, ++ay);
                            }
                            continue;
                        case ConsoleKey.LeftArrow:
                            if (ax > 0 && ax < 3)
                            {
                                gameBoard.DrawGameBoard(--ax, ay);
                            }
                            continue;
                        case ConsoleKey.RightArrow:
                            if (ax >= 0 && ax < 2)
                            {
                                gameBoard.DrawGameBoard(++ax, ay);
                            }
                            continue;
                        case ConsoleKey.Enter:
                            if (choosePlayer)
                            {
                                gameBoard.DrawGameBoard(ax, ay, '1');
                                if (Winner(gameBoard, "x"))
                                {
                                    break;
                                }
                                choosePlayer = !choosePlayer;
                                continue;
                            }
                            else
                            {
                                gameBoard.DrawGameBoard(ax, ay, '2');
                                if (Winner(gameBoard, "o"))
                                {
                                    break;
                                }
                                choosePlayer = !choosePlayer;
                                continue;
                            }
                    }
                    Console.Clear();
                    Console.WriteLine(FiggleFonts.Doom.Render($"                    {(choosePlayer?"X":"Y")}  WON The Game!!"));
                    Thread.Sleep(3000);
                    break;
                }
                if (draw(gameBoard))
                {
                    Console.Clear();
                    Console.WriteLine(FiggleFonts.Doom.Render($"                    Game Is Draw!"));
                    Thread.Sleep(3000);
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("Do you want to continue gaming?Y/N");
                runGame = Console.ReadLine().ToLower();
                Console.Clear();
            }
        }
        static bool Winner(GameBoard gameBoard,string player)
        {
            /*Horizontal*/
            if (gameBoard.occupied[0,0]==player && gameBoard.occupied[1, 0] == player && gameBoard.occupied[2, 0] == player)
            {
                return true;
            }
            /*Horizontal*/
            else if (gameBoard.occupied[0, 1] == player && gameBoard.occupied[1, 1] == player && gameBoard.occupied[2, 1] == player)
            {
                return true;
            }
            /*Horizontal*/
            else if (gameBoard.occupied[0, 2] == player && gameBoard.occupied[1, 2] == player && gameBoard.occupied[2, 2] == player)
            {
                return true;
            }
            /*Diagonal*/
            else if (gameBoard.occupied[0, 0] == player && gameBoard.occupied[1, 1] == player && gameBoard.occupied[2, 2] == player)
            {
                return true;
            }
            /*Diagonal*/
            else if (gameBoard.occupied[2, 0] == player && gameBoard.occupied[1, 1] == player && gameBoard.occupied[0, 2] == player)
            {
                return true;
            }
            /*Vertical*/
            else if (gameBoard.occupied[0, 0] == player && gameBoard.occupied[0, 1] == player && gameBoard.occupied[0, 2] == player)
            {
                return true;
            }
            /*Vertical*/
            else if (gameBoard.occupied[1, 0] == player && gameBoard.occupied[1, 1] == player && gameBoard.occupied[1, 2] == player)
            {
                return true;
            }
            /*Vertical*/
            else if (gameBoard.occupied[2, 0] == player && gameBoard.occupied[2, 1] == player && gameBoard.occupied[2, 2] == player)
            {
                return true;
            }
            return false;
        }
        static bool draw(GameBoard gameBoard)
        {
            foreach (var item in gameBoard.occupied)
            {
                if (item==null)
                {
                    return false;
                }
            }
            return true;
        }

        class GameBoard
        {
            internal string[,] occupied = new string[3,3];

            /// <summary>
            /// Draws a colored square in desired position of console.
            /// </summary>
            /// <param name="x">start position to draw from left.</param>
            /// <param name="y">start position to draw from up.</param>
            /// <param name="l">lenght of sides of sqaure</param>
            /// <param name="color">color of the drawed square</param>
            private void DrawSquare(int x, int y, int l,string s=" ", ConsoleColor color = ConsoleColor.Cyan)
            {                
                Console.CursorVisible = false;
                Console.BackgroundColor = color;
                for (int i = x; i < x + l+5; i++)
                {
                    for (int j = y; j < y + l; j++)
                    {
                        switch (s)
                        {
                            case " ":
                                Console.SetCursorPosition(i, j);
                                Console.WriteLine(" ");
                                break;
                            case "X":
                                if (j == y + (l / 2)-2 && i == x + ((l + 5) / 2)-5)
                                {
                                  using (StreamReader Ascii = new StreamReader("X.txt"))
                                  {
                                      while (Ascii.Peek()>-1)
                                      {
                                          Console.SetCursorPosition(i, j++);
                                          Console.Write(Ascii.ReadLine());
                                      }
                                  }                                     
                                }
                                break;
                            case "O":
                                if (j == y + (l / 2)-2 && i == x + ((l + 5) / 2)-5)
                                {
                                    using (StreamReader Ascii = new StreamReader("O.txt"))
                                    {
                                        while (Ascii.Peek() > -1)
                                        {
                                            Console.SetCursorPosition(i, j++);
                                            Console.Write(Ascii.ReadLine());
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            internal void DrawGameBoard(int ax, int ay,char player='0')
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (ax == i && ay == j)
                        {
                            switch (player)
                            {
                                case '0':
                                    if (occupied[i, j] == "" | occupied[i, j] == null)
                                    {
                                        DrawSquare(11 * i + 40, 6 * j + 5, 5, color: ConsoleColor.DarkBlue);
                                    }
                                    break;
                                case '1':
                                    if (occupied[i,j]==""| occupied[i, j] ==null)
                                    {
                                        DrawSquare(11 * i + 40, 6 * j + 5, 5, s: "X", color: ConsoleColor.Yellow);
                                        occupied[i, j] = "x";
                                    }
                                    break;
                                case '2':
                                    if (occupied[i, j] == "" | occupied[i, j] == null)
                                    {
                                        DrawSquare(11 * i + 40, 6 * j + 5, 5, s: "O", color: ConsoleColor.Yellow);
                                        occupied[i, j] = "o";
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (occupied[i, j] == "" | occupied[i, j] == null)
                            {
                                DrawSquare(11 * i + 40, 6 * j + 5, 5);
                            }                                
                        }
                    }
                }
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Figgle;

namespace Tennis
{
    class Program
    {
        static string[] player =new string[] {"",""};
        static byte[] GameP1History = new byte[byte.MaxValue];
        static byte[] GameP2History = new byte[byte.MaxValue];
        
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "TENNIS SCOREBOARD";
            Console.WindowHeight = 25;
            Console.WindowWidth = 130;
            Console.SetWindowPosition(0, 0);
            Console.SetCursorPosition(10, 30);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Clear();
            Console.WriteLine(FiggleFonts.Doom.Render("         WELCOME TO TENNIS!"));
            Thread.Sleep(3000);
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            while (player[0]=="")
            {
                Console.SetCursorPosition(Console.WindowWidth/2-20, Console.WindowHeight/2);
                Console.ForegroundColor = ConsoleColor.Red;                
                Console.Write("Please Enter a name for Player1 : ");                
                player[0] = Console.ReadLine().ToUpper();
                Console.Clear();
                if (player[0] == "")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2);                    
                    Console.WriteLine("You Must Enter a name for Player 1!!");
                    continue;
                }
            }
            while (player[1]=="")
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2);                
                Console.Write("Please Enter a name for Player2 : ");                
                player[1] = Console.ReadLine().ToUpper();
                Console.Clear();
                if (player[1]=="")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2);                    
                    Console.WriteLine("You Must Enter a name for Player 2!!");
                    continue;
                }
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight / 2);
            Console.Write($"{player[0]} is left --");
            Console.ForegroundColor = ConsoleColor.Blue;            
            Console.Write($"--{player[1]} is right");
            Random rnd = new Random();
            int nobat = rnd.Next(1000, 6000);
            Thread.Sleep(3000);
            if (nobat<3500)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 30, Console.WindowHeight / 2);                
                Console.WriteLine($"{player[0]} Starts The Game!    Press LEFT Arrow key to start!");
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 30, Console.WindowHeight / 2);                
                Console.WriteLine($"{player[1]} Starts The Game!    Press RIGHT Arrow key to start!");
            }
            bool match = false;
            int i = 0;
            byte setP1 = 0;
            byte setP2 = 0;
            bool set = false;
            while (!match)
            {
                byte gameP1 = 0;
                byte gameP2 = 0;

                while (!set)
                {
                    byte pointP1 = 0;
                    byte pointP2 = 0;
                    while (true)
                    {
                        ConsoleKey pressedKey = Console.ReadKey().Key;
                        if (pressedKey==ConsoleKey.LeftArrow)
                        {
                            Console.Beep(1000, 100);
                            Console.ForegroundColor = ConsoleColor.Red;
                            pointP1++;
                        }
                        else if (pressedKey==ConsoleKey.RightArrow)
                        {
                            Console.Beep(2000, 100);
                            Console.ForegroundColor = ConsoleColor.Blue;
                            pointP2++;
                        }
                        else
                        {
                            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, Console.WindowHeight / 2);
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine("please enter left or right arrow key!");
                            
                            continue;
                        }
                        if ((pointP1 > pointP2 + 1) && pointP1 > 3)
                        {
                            gameP1++;
                            break;
                        }
                        else if ((pointP2 > pointP1 + 1) && pointP2 > 3)
                        {
                            gameP2++;
                            break;
                        }
                        ScoreBoard(pointP1, pointP2);                        
                    }
                    GameP1History[i] = gameP1;
                    GameP2History[i] = gameP2;                    
                    ScoreBoard(pointP1, pointP2);
                    if (gameP1>gameP2+1 && gameP1>=6)
                    {
                        setP1++;
                        i++;
                        gameP1 = 0;
                        gameP2 = 0;
                    }
                    else if (gameP2>gameP1+1 && gameP2>=6)
                    {
                        setP2++;
                        i++;
                        gameP2 = 0;
                        gameP1 = 0;
                    }
                    if (setP1 + setP2 == 5 || (setP1 >= 3 && setP1 + setP2 >= 3) || (setP2 >= 3 && setP2 + setP1 >= 3))
                    {
                        set = true;
                    }
                }
                if (setP1>setP2)
                {
                    match = true;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    PrintWinner(player[0]);
                }
                else
                {
                    match = true;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    PrintWinner(player[1]);
                }
            }            
        }
        static void PrintWinner(string player)
        {            
            Console.SetCursorPosition(0, 10);
            Console.WriteLine(FiggleFonts.Small.Render($"{player} WINS THE GAME!"));
            string[] color = new string[] { "Green", "Yellow", "Red" };
            Console.ReadLine();
        }

        static void ScoreBoard(byte pointP1,byte pointP2)
        {               
            Console.Clear();
            switch (pointP1)
            {
                case 1:
                    pointP1 = 15;  
                    break;
                case 2:
                    pointP1 = 30;
                    break;
                case 3:
                    pointP1 = 40;
                    break;
                case 4:
                    pointP1 = 0;
                    break;
            }
            switch (pointP2)
            {
                case 1:
                    pointP2 = 15;
                    break;
                case 2:
                    pointP2 = 30;
                    break;
                case 3:
                    pointP2 = 40;
                    break;
                case 4:
                    pointP2 = 0;
                    break;
            }
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------\n");
            Console.WriteLine(FiggleFonts.Basic.Render($"| {player[0].Substring(0,2).ToUpper()} | {GameP1History[0]} | {GameP1History[1]} | {GameP1History[2]} | {GameP1History[3]} | {GameP1History[4]} | {pointP1}"));
            Console.WriteLine(FiggleFonts.Basic.Render($"| {player[1].Substring(0,2).ToUpper()} | {GameP2History[0]} | {GameP2History[1]} | {GameP2History[2]} | {GameP2History[3]} | {GameP2History[4]} | {pointP2}"));
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------");
        }
    }
}
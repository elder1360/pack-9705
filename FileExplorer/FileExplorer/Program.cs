using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;


namespace FileExplorer
{
    class Program
    {
        static DriveInfo[] drives = DriveInfo.GetDrives();
        static int closeExplorer = 1;        
        static void Main(string[] args)
        {            
            while (closeExplorer==1)
            {
                PrintDrives();
                ExplorDrives();
            }
        }
        static void PrintDrives()
        {
            Console.Clear();
            List<string> infos = new List<string>();
            List<int> infosLengths = new List<int>();
            List<string> hajmhas = new List<string>();
            List<int> hajmhasLengths = new List<int>();
            int infosMaxLength = 0;
            int hajmhasMaxLength = 0;
            for (int i = 0; i < drives.Length; i++)
            {
                if (drives[i].IsReady)
                {
                    infos.Add(String.Format($"{i + 1}.{drives[i].Name} Label:({drives[i].VolumeLabel}){drives[i].DriveType}Type"));
                    hajmhas.Add(string.Format($"FreeSpace:{ drives[i].AvailableFreeSpace / 1024 / 1024 / 1024}GB TotalSize:{ drives[i].TotalSize / 1024 / 1024 / 1024}GB"));
                }
            }
            foreach (var item in infos)
            {
                infosLengths.Add(item.Length);
            }
            foreach (var item in hajmhas)
            {
                hajmhasLengths.Add(item.Length);
            }
            foreach (var item in infosLengths)
            {
                if (item > infosMaxLength)
                {
                    infosMaxLength = item;
                }
            }
            foreach (var item in hajmhasLengths)
            {
                if (item>hajmhasMaxLength)
                {
                    hajmhasMaxLength = item;
                }
            }
            for (int j = 0, k = 0; j < drives.Length; j++)
            {
                if (drives[j].IsReady)
                {
                    string info = (String.Format($"{j + 1}.{drives[j].Name} Label:({drives[j].VolumeLabel}){drives[j].DriveType}Type"));
                    string hajmha = string.Format($"FreeSpace:{ drives[j].AvailableFreeSpace / 1024 / 1024 / 1024}GB TotalSize:{ drives[j].TotalSize / 1024 / 1024 / 1024}GB");
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(info);
                    stringBuilder.Insert(infos[k].Length, " ", infosMaxLength - infos[k++].Length + 5);
                    stringBuilder.Append(hajmha);
                    Console.WriteLine(stringBuilder);
                }
                else
                {
                    Console.WriteLine("{0}.{1} {2} Drive Not Ready", j + 1, drives[j].Name, drives[j].DriveType);
                }
            }
            StringBuilder dashes = new StringBuilder();
            Console.WriteLine(dashes.Insert(0,"-",infosMaxLength+hajmhasMaxLength+5));           
        }
        static void ExplorDrives()
        {
            Console.Write("Please enter a drive number:(0 to Exit)");
            int choosedDrive;
            bool choosedDriveCorrect = int.TryParse(Console.ReadLine(), out choosedDrive);
            if (choosedDrive==0)
            {
                closeExplorer = 0;                
            }
            else if(choosedDrive>0&&choosedDrive<=drives.Length)
            {
                if (drives[choosedDrive-1].IsReady)
                {
                    StringBuilder path = new StringBuilder(drives[choosedDrive - 1].Name);
                    while (true)
                    {
                        Console.Clear();                 
                        DirectoryInfo directory = new DirectoryInfo(path.ToString());
                        DirectoryInfo[] directories = directory.GetDirectories();
                        FileInfo[] files = directory.GetFiles();
                        for (int i = 0; i < directories.Length; i++)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"{i + 1}-{directories[i]}");                            
                        }
                        for (int j = 0; j < files.Length; j++)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write($"{j + 1 + directories.Length}-{files[j]}");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("[File]");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        if (files.Length==0&&directories.Length==0)
                        {
                            Console.WriteLine("Directory is Empty.Enter 0 to go back.");
                        }
                        else
                        {
                            Console.WriteLine(@"select desired directory number to open or file number to execute.""Enter 0 to go back""");
                        }
                        int choosedDirectory;
                        bool choosedDirectoryCorrect = int.TryParse(Console.ReadLine(), out choosedDirectory);
                        if (choosedDirectory > 0&&choosedDirectory<=directories.Length)
                        {
                            directory = directories[choosedDirectory - 1];
                            path.Append("\\" + directory.ToString());
                        }
                        else if(choosedDirectory>directories.Length)
                        {
                            Process.Start(path.ToString() + "\\" + files[choosedDirectory - 1 - directories.Length]);
                        }
                        else
                        {
                            path.Clear();
                            try
                            {
                                path.Append(directory.Parent.FullName.ToString());
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Drive is not ready.");
                    Thread.Sleep(2000);
                }
            }
            else
            {
                Console.WriteLine("Please enter correct drive number.");
                Thread.Sleep(2000);
            }
        }        
    }
}

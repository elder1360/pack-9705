using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PhoneBook
{
    class Program
    {
        static string[] numbers = new string[] {"0912","0935"};
        static string[] names = new string[] {"ali","mohammad" };
        static string[] familyNames = new string[] { "naghi","taghi" };

        static void Main(string[] args)
        {
            bool run = true;
            while (run)
            {
                ShowPhoneBook();
                Console.WriteLine("1.ADD");
                Console.WriteLine("2.REMOVE");
                Console.WriteLine("3.EDIT");
                Console.WriteLine("4.EXIT");
                switch (Console.ReadLine())
                {
                    case "1":
                        char addAnother = 'Y';
                        while (addAnother == 'Y')
                        {
                            Console.Clear();
                            Console.WriteLine("Please Enter Phone Number,Name and Family name.Put a Space between them!");
                            string[] userInputAdd = Console.ReadLine().Split(' ');
                            try
                            {
                                Console.WriteLine($"NO: {userInputAdd[0]}\tName: {userInputAdd[1]}\tFamily:{userInputAdd[2]} is ready to add.Continue?Y/N");
                            }
                            catch (Exception)
                            {
                                Console.Beep(4000,800);
                                for (int i = 0; i < 4; i++)
                                {
                                    Console.CursorVisible = false;
                                    Console.Clear();
                                    Console.WriteLine("Please enter new contact in correct format (Put space between number name and family name)");
                                    Thread.Sleep(600);
                                    Console.Clear();
                                    Thread.Sleep(500);
                                }
                                
                                continue;
                            }
                            char okToAdd = Convert.ToChar(Console.ReadLine().ToUpper());
                            if (okToAdd == 'Y')
                            {
                                Add(ref numbers, userInputAdd[0]);
                                Add(ref names, userInputAdd[1]);
                                Add(ref familyNames, userInputAdd[2]);
                            }
                            else
                            {
                                Console.WriteLine("Adding to PhoneBook discarded by user.");
                            }
                            Console.WriteLine("Add Another?Y/N");
                            addAnother = Convert.ToChar(Console.ReadLine().ToUpper());
                        }
                        continue;
                    case "2":
                        char removeAnother = 'Y';
                        while (removeAnother=='Y')
                        {
                            if (numbers.Length==0)
                            {
                                Console.WriteLine("No More contact to Remove !");
                                Console.Beep(4000, 1000);
                                Thread.Sleep(2000);
                                break;
                            }
                            Console.Clear();
                            ShowPhoneBook();
                            Console.WriteLine("Which contact do you want to remove? Enter the corresponding number.");
                            try
                            {
                                int userInputRemove = int.Parse(Console.ReadLine());
                                ShowPhoneBook(userInputRemove);
                                Console.WriteLine("Are you sure you wnat to remove:Y/N");
                                char yesToRemove = Convert.ToChar(Console.ReadLine().ToUpper());
                                if (yesToRemove == 'Y')
                                {
                                    remove(ref numbers, userInputRemove);
                                    remove(ref names, userInputRemove);
                                    remove(ref familyNames, userInputRemove);
                                }
                                else
                                {
                                    Console.WriteLine("Remove contact discarded by user.");
                                    Thread.Sleep(1500);
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Please enter contact row number correctly !");
                                Console.Beep(4000, 1000);
                                Thread.Sleep(1500);
                                continue;
                            }
                            Console.WriteLine("Do you want to remove another contact?Y/N");
                            removeAnother = Convert.ToChar(Console.ReadLine().ToUpper());
                        }
                        break;
                    case "3":
                        ShowPhoneBook();
                        Console.WriteLine("Which contact do you want to edit?");
                        int userInputEdit = int.Parse(Console.ReadLine());
                        ShowPhoneBook(userInputEdit);
                        Console.WriteLine("Enter phone number, name and family name to edit.PUT SPACE BETWEEN THEME");
                        string[] userInputContactEdit = Console.ReadLine().Split(' ');
                        Console.WriteLine($"{userInputEdit}- Phone: {userInputContactEdit[0]}\tName: {userInputContactEdit[1]}\tFamily: {userInputContactEdit[2]}");
                        Console.WriteLine("Are you sure You want to apply Edit?Y/N");
                        char okTpApplyEdit = Convert.ToChar(Console.ReadLine().ToUpper());
                        if (okTpApplyEdit=='Y')
                        {
                            numbers[userInputEdit] = userInputContactEdit[0];
                            names[userInputEdit] = userInputContactEdit[1];
                            familyNames[userInputEdit] = userInputContactEdit[2];
                        }
                        else
                        {
                            Console.WriteLine("Contact edit discarded by user");
                            Thread.Sleep(1500);
                        }
                        break;
                    case "4":
                        run = false;
                        break;
                    default:
                        Console.WriteLine("Please Enter a Number Between 1 to 4");
                        break;
                }
            }
        }
        static void ShowPhoneBook()
        {
            Console.Clear();
            for (int i=0; i < numbers.Length; i++)
            {
                Console.WriteLine($"{i + 1}- Phone: {numbers[i]}\tName: {names[i]}\tFamily: {familyNames[i]}");
            }
            Console.WriteLine("--------------------------------------------------");            
        }
        static void ShowPhoneBook(int i)
        {
            Console.WriteLine($"{i}- Phone: {numbers[i-1]}\tName: {names[i-1]}\tFamily: {familyNames[i-1]}");
        }
        static void Add(ref string[] array,string input)
        {
            string[] newArray = new string[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }
            newArray[newArray.Length - 1] = input;
            array = newArray;
        }
        static void remove(ref string[] array,int input)
        {
            string[] newArray = new string[array.Length - 1];
            for (int i = 0,j=0; i < array.Length; i++)
            {
                if (input==i+1)
                {
                    continue;
                }
                newArray[j++] = array[i];
            }
            array = newArray;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace PhoneBookOOP
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Contact> contacts = new List<Contact>();
            GetData(contacts);
            char work = 'Y';
            while (work=='Y')
            {
                PrintPhoneBook(contacts);
                PrintOptions();

                char choice = Convert.ToChar(Console.ReadLine());
                switch (choice)
                {
                    case '1':
                        PrintPhoneBook(contacts);
                        Add(contacts);
                        break;
                    case '2':
                        Remove(contacts);
                        break;
                    case '3':
                        Edit(contacts);
                        break;
                    case '4':
                        Console.WriteLine("Are You Sure you want to exit?Y/N");
                        char exit = Convert.ToChar(Console.ReadLine().ToUpper());                        
                        if (exit=='Y')
                        {
                            work= ' ';                            
                        }                        
                        break;
                    default:
                        Console.WriteLine("Please enter correct number!");
                        Thread.Sleep(2000);
                        continue;
                }
            }
        }
        private static void PrintPhoneBook(List<Contact> contacts)
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("|--PhoneNumber---Name----FamilyName----Email----|\n|\t\t\t\t\t        |");
            for (int i = 0; i < contacts.Count; i++)
            {
                Console.WriteLine($"| {i+1}- {contacts[i].PhoneNumber}\t{contacts[i].Name}\t{contacts[i].FamilyName}\t{contacts[i].Email}   |");
            }
            Console.WriteLine("|\t\t\t\t\t        |\n|-----------------------------------------------|\n");            
        }

        private static void PrintPhoneBook(List<Contact> contacts,int choosed)
        {
            Console.Clear();
            Console.WriteLine("|--PhoneNumber---Name----FamilyName----Email----|\n|\t\t\t\t\t        |");
            Console.WriteLine($"| {choosed}- {contacts[choosed-1].PhoneNumber}\t{contacts[choosed - 1].Name}\t{contacts[choosed - 1].FamilyName}\t{contacts[choosed - 1].Email}   |");
            Console.WriteLine("|\t\t\t\t\t        |\n|-----------------------------------------------|\n");
        }

        private static void Add(List<Contact> contacts)
        {
            Console.Clear();
            PrintPhoneBook(contacts);
            Console.WriteLine("Please enter PhoneNumber Name FamilyName Email.(Put Space between each entry)");
            Console.CursorVisible = true;
            string[] userInputAdd = Console.ReadLine().Split(' ');
            try
            {
                Contact newContact = new Contact
                {
                    PhoneNumber = userInputAdd[0],
                    Name = userInputAdd[1].Replace(userInputAdd[1].Substring(0, 1), userInputAdd[1].Substring(0, 1).ToUpper()),
                    FamilyName = userInputAdd[2].Replace(userInputAdd[2].Substring(0, 1), userInputAdd[2].Substring(0, 1).ToUpper()),
                    Email = userInputAdd[3]
                };
                contacts.Add(newContact);
            }
            catch (Exception)
            {
                Console.WriteLine("Please Enter contact correctly! put space between each entry.");
                Thread.Sleep(2000);
            }
            SaveData(contacts);
        }

        private static void Remove(List<Contact> contacts)
        {
            Console.Clear();
            PrintPhoneBook(contacts);
            Console.WriteLine("Please enter the contact's number to Remove.");
            int choosedRemove = Convert.ToInt32(Console.ReadLine());
            PrintPhoneBook(contacts, choosedRemove);
            Console.WriteLine("Are You Sure to Delet This contact?Y/N");
            string sureToRemove = Console.ReadLine().ToLower();
            if (sureToRemove=="y")
            {
                contacts.RemoveAt(choosedRemove - 1);
                SaveData(contacts);
                Console.WriteLine("Contact Removed Succesfully.");
                Thread.Sleep(1000);
                return;
            }
            Console.WriteLine("Remove Cancelled by user.");
            Thread.Sleep(1000);
        }

        private static void Edit(List<Contact> contacts)
        {
            Console.CursorVisible = true;
            Console.Clear();
            PrintPhoneBook(contacts);
            Console.WriteLine("Please enter the contact's number to Edit.");
            int choosedEdit = Convert.ToInt32(Console.ReadLine());
            PrintPhoneBook(contacts, choosedEdit);
            Console.WriteLine("Which part do you want to edit?");
            Console.WriteLine("1- PhoneNumber 2-Name 3-FamilyName 4-EmailAddress 0-All");
            int choosedPart = Convert.ToInt32(Console.ReadLine());
            switch (choosedPart)
            {
                case 1:
                    Console.WriteLine("Please enter new PhoneNumber");
                    contacts[choosedEdit - 1].PhoneNumber = Console.ReadLine();
                    break;
                case 2:
                    Console.WriteLine("Please enter new Name");
                    contacts[choosedEdit - 1].Name = Console.ReadLine();
                    break;
                case 3:
                    Console.WriteLine("Please enter new FamilyName");
                    contacts[choosedEdit - 1].FamilyName = Console.ReadLine();
                    break;
                case 4:
                    Console.WriteLine("Please enter new EmailAddress");
                    contacts[choosedEdit - 1].Email = Console.ReadLine();
                    break;
                case 0:
                    Console.WriteLine("Please enter new PhoneNumber,Name,FamilyName,EmailAddress(PUT SPACE BETWEEN EACH OF THEM)");
                    string[] allEnterdForEdit = Console.ReadLine().Split(' ');
                    contacts[choosedEdit - 1].PhoneNumber = allEnterdForEdit[0];
                    contacts[choosedEdit - 1].Name = allEnterdForEdit[1];
                    contacts[choosedEdit - 1].FamilyName = allEnterdForEdit[2];
                    contacts[choosedEdit - 1].Email = allEnterdForEdit[3];
                    break;
                default:
                    Console.WriteLine("Please enter numbers correctly.");
                    break;
            }
            SaveData(contacts);
        }

        private static void GetData(List<Contact> contacts)
        {
            using (StreamReader sr = new StreamReader("PhoneBook.txt"))
            {
                while (sr.Peek()>-1)
                {
                    string[] dataFromFile = sr.ReadLine().Split(',');
                    Contact contact = new Contact
                    {
                        PhoneNumber = dataFromFile[0],
                        Name = dataFromFile[1],
                        FamilyName = dataFromFile[2],
                        Email = dataFromFile[3]
                    };
                    contacts.Add(contact);
                }
            }
        }

        private static void PrintOptions()
        {
            Console.WriteLine("1- ADD");
            Console.WriteLine("2- Remove");
            Console.WriteLine("3- Edit");
            Console.WriteLine("4- Exit");
        }

        private static void SaveData(List<Contact> contacts)
        {
            using (StreamWriter sw = new StreamWriter("PhoneBook.txt"))
            {
                foreach (var item in contacts)
                {
                    sw.WriteLine($"{item.PhoneNumber},{item.Name},{item.FamilyName},{item.Email}");
                }
            }

        }
    }
}
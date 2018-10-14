using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PhoneBookOOP
{
    class Contact
    {        
        private string phoneNumber;
        private string name;
        private string familyName;
        private string email;

        public string Email
        {
            get { return email; }
            set
            {
                if (Regex.IsMatch(value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                {
                    email = value;
                }
                else
                {
                    Console.WriteLine("Invalid Email Address entered.");
                    email = " ";
                }
            }
        }

        public string FamilyName
        {
            get { return familyName; }
            set { familyName = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

    }
}

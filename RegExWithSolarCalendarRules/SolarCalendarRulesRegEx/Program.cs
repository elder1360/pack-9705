using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace SolarCalendarRulesRegEx
{
    class Program
    {
        static void Main(string[] args)
        {
            var pattern = @"^((\d{4}\/[0-1][0-9]\/[0-3][0-9]) | (\d{4}\/[0-1][0-9]\/[0-3][0-9]))$";
            Regex re = new Regex(pattern);

            while (true)
            {
                Console.WriteLine("Text?");
                var text = Console.ReadLine();
                if (re.IsMatch(text))
                {
                    Console.WriteLine("Match!");
                    continue;
                }
                else
                {
                    Console.WriteLine("NotMatch!");
                    continue;
                }
            }
        }
    }
}

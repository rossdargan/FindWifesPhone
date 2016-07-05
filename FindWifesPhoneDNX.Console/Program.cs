using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindWifesPhoneDNX.Console
{
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    using Console = System.Console;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter your Apple acction details:");
            Console.WriteLine("Username:");
            var username = Console.ReadLine();
            Console.WriteLine("Password:");
            var password = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Please enter your device name:");
            var device = Console.ReadLine();

            var phoneService = new FindWifesPhoneDNX.Library.FindPhoneService("213-123-123");
            bool result = phoneService.FindPhone(username, password, device).Result;

            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}

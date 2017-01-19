using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApexComm
{
    class LogMsg
    {
        public static  void log(string str)
        {
            Console.WriteLine(str);
        }
        public static void log_error(string str)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}

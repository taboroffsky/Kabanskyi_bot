using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Controller controller = new Controller();
         
            while(true)
            {
                try
                {
                    controller.GetUpdates();
                }
                catch
                {
                    Console.WriteLine("Network error.");
                }

                Thread.Sleep(1000);
            }
        }
    }
}

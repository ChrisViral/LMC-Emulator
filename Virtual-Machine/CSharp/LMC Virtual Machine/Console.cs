using System;
using System.Collections.Generic;

namespace LMC_Virtual_Machine
{
    public class VirtualMachine
    {
        #region Main
        private static void Main()
        {
            Console.SetWindowSize(Console.WindowWidth * 2, Console.WindowHeight * 2);
            bool correct = false;
            List<string> code = new List<string>();
            Machine machine = null;

            while (!correct)
            {
                Console.WriteLine("Please enter your LMC commands one line at a time. When the program is complete, enter a blank line.\n");

                //Code entering
                while (true)
                {
                    string line = Console.ReadLine();
                    if (line == string.Empty) { break; }
                    code.Add(line);
                }

                //If empty restart
                if (code.Count == 0) { Console.Clear(); continue; }

                Console.WriteLine("Is this program correct? Y/N");

                //Check for Y and N key presses
                bool cont = true;
                while (true)
                {
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.Y) { Console.WriteLine("\n"); break; }
                    if (key == ConsoleKey.N)
                    {
                        Console.Clear();
                        code = new List<string>();
                        cont = false;
                        break;
                    }
                    Console.Write("\b \b");
                }
                if (!cont) { continue; }

                //Verify if code is valid
                machine = new Machine(code);
                if (!machine.Compile())
                {
                    Console.WriteLine("Code is invalid, please reenter a valid LMC command code. Press nter to restart.");
                    Console.ReadLine();
                    Console.Clear();
                    code = new List<string>();
                }
                else
                {
                    Console.WriteLine("\nCode is verified and valid, proceeding.");
                    correct = true;
                }
            }

            //Run virtual machine
            machine.Run();
        }
        #endregion
    }
}

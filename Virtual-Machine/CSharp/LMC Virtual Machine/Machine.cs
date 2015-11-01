using System;
using System.Collections.Generic;

namespace LMC_Virtual_Machine
{
    public class Machine
    {
        #region Fields
        private List<string> lines = new List<string>();
        private Compiler compiler = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Machine
        /// </summary>
        /// <param name="lines">Codeline instructions</param>
        public Machine(List<string> lines)
        {
            this.lines = lines;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Tries to compile the codelines and returns if it worked
        /// </summary>
        public bool Compile()
        {
            this.compiler = new Compiler(this.lines);
            return compiler.Compile();
        }

        /// <summary>
        /// Runs the compiled code
        /// </summary>
        public void Run()
        {
            Console.WriteLine("Delabeled code:");
            Console.WriteLine(Utils.PrintCode(compiler.code));
            Console.WriteLine("\nInstructions:");
            Console.WriteLine(Utils.PrintInstructions(compiler.instructions));
            Console.WriteLine("Press enter to execute.");
            Console.ReadLine();

            Processor processor = new Processor(compiler.instructions);
            processor.Execute();

            Console.WriteLine("\nPress enter to exit.");
            Console.ReadLine();
        }
        #endregion
    }
}

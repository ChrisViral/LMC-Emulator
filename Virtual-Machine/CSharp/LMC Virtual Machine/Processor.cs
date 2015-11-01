using System;

namespace LMC_Virtual_Machine
{
    public class Processor
    {
        //The virtual machine's state
        private struct State
        {
            #region Fields
            public UNum[] mem;      //Memory
            public UNum acc, pc;    //Accumulator, Program Counter
            #endregion

            #region Properties
            /// <summary>
            /// Length of the memory array
            /// </summary>
            public int length
            {
                get { return this.mem.Length; }
            }

            /// <summary>
            /// Instruction being read by the program counter
            /// </summary>
            public UNum current
            {
                get { return this.mem[this.pc]; }
            }
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new State from an initial memory array
            /// </summary>
            /// <param name="mem"></param>
            public State(UNum[] mem)
            {
                this.mem = mem;
                this.acc = 0;
                this.pc = 0;
            }
            #endregion
        }

        #region Fields
        private State state = new State();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new processor with an initial memory array of instructions
        /// </summary>
        /// <param name="instructions">Initial memory state</param>
        public Processor(UNum[] instructions)
        {
            this.state = new State(instructions);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the program
        /// </summary>
        public void Execute()
        {
            while (this.state.pc < this.state.length)
            {
                UNum c = this.state.current;
                if (c < 100 || (c >= 400 && c < 500) || c > 902) { break; }
                this.state.pc++;
                this.state = ExecuteInstruction(c, this.state);
            }

            Console.WriteLine("\nProgram execution halted.");
            Console.WriteLine("Accumulator: " + this.state.acc);
            Console.WriteLine("Program counter: " + this.state.pc);
            Console.WriteLine("Memory state:\n" + Utils.PrintInstructions(this.state.mem));
        }

        /// <summary>
        /// Executes a specific instruction and returns the resulting state
        /// </summary>
        /// <param name="c">Current instruction</param>
        /// <param name="state">Current state of the machine</param>
        private State ExecuteInstruction(UNum c, State state)
        {
            //Identifies which instruction to execute
            Instruction instruction;
            UNum index = c % 100;
            if (c == 901 || c == 902) { instruction = (Instruction)c; }
            else { instruction = (Instruction)((c - index) / 100); }

            //Executes current instruction
            switch (instruction)
            {
                //Addition
                case Instruction.ADD:
                    {
                        state.acc = state.acc + state.mem[index];
                        break;
                    }

                // Substraction
                case Instruction.SUB:
                    {
                        state.acc = state.acc - state.mem[index];
                        break;
                    }

                //Store
                case Instruction.STA:
                    {
                        state.mem[index] = state.acc;
                        break;
                    }

                //Load
                case Instruction.LDA:
                    {
                        state.acc = state.mem[index];
                        break;
                    }

                //Branch
                case Instruction.BRA:
                    {
                        state.pc = index;
                        break;
                    }

                //Branch if zero
                case Instruction.BRZ:
                    {
                        if (state.acc == 0) { state.pc = index; }
                        break;
                    }

                //Branch if positive
                case Instruction.BRP:
                    {
                        if (state.acc >= 0) { state.pc = index; }
                        break;
                    }

                //Input
                case Instruction.INP:
                    {
                        Console.WriteLine("\nEnter a number from -500 to 499:");
                        string s = Console.ReadLine();
                        Num n = 0;
                        while (!Num.TryParse(s, ref n))
                        {
                            Console.WriteLine("\nNumber invalid. Enter a number from -500 to 499:");
                            s = Console.ReadLine();
                        }
                        state.acc = (UNum)n;
                        break;
                    }

                //Output
                case Instruction.OUT:
                    {
                        Console.WriteLine("\nOutput: " + (Num)state.acc);
                        break;
                    }
            }
            return state;
        }
        #endregion
    }
}

using System.Collections.Generic;

namespace LMC_Virtual_Machine
{
    //Instruction members
    public enum Instruction
    {
        ADD = 100,      //Addition
        SUB = 200,      //Substraction
        STA = 300,      //Store
        LDA = 500,      //Load
        BRA = 600,      //Branch
        BRZ = 700,      //Branch if zero
        BRP = 800,      //Branch if positive
        INP = 901,      //Input
        OUT = 902,      //Output
        HLT = 0,        //Halt
        DAT = 0         //Data
    }

    public class Compiler
    {
        private struct Label
        {
            #region Fields
            public string name;
            public UNum line;
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new Label
            /// </summary>
            /// <param name="name">Name of the label</param>
            /// <param name="line">Line number of the label</param>
            public Label(string name, UNum line)
            {
                this.name = name.Remove(name.Length - 1);
                this.line = line;
            }
            #endregion

            #region Methods
            /// <summary>
            /// Returns if a given string is a label or not (finishes by ':')
            /// </summary>
            /// <param name="label">String to verify</param>
            public static bool IsLabel(string label)
            {
                return label[label.Length - 1] == ':';
            }
            #endregion
        }

        //Quick string->Instruction parsing dictionary
        private static readonly Dictionary<string, Instruction> charCodes = new Dictionary<string, Instruction>(11)
        #region Values
        {
            { "ADD", Instruction.ADD },
            { "SUB", Instruction.SUB },
            { "STA", Instruction.STA },
            { "LDA", Instruction.LDA },
            { "BRA", Instruction.BRA },
            { "BRZ", Instruction.BRZ },
            { "BRP", Instruction.BRP },
            { "INP", Instruction.INP },
            { "OUT", Instruction.OUT },
            { "HLT", Instruction.HLT },
            { "DAT", Instruction.DAT },
        };
        #endregion

        #region Fields
        private List<string> lines = new List<string>();
        #endregion

        #region Properties
        private List<string[]> _code = new List<string[]>();
        /// <summary>
        /// Delabeled code
        /// </summary>
        public List<string[]> code
        {
            get { return this._code; }
        }

        private UNum[] _instructions = new UNum[0];
        /// <summary>
        /// Numeric code instructions
        /// </summary>
        public UNum[] instructions
        {
            get { return this._instructions; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new compiler
        /// </summary>
        /// <param name="lines">Original lines of code</param>
        public Compiler(List<string> lines)
        {
            this.lines = lines;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Compiles the code and verifies if it is valid
        /// </summary>
        public bool Compile()
        {
            List<string[]> labeled = SplitLines(this.lines);
            this._code = Delabel(labeled);
            return TryParseInstructions(this._code, ref this._instructions);
        }

        /// <summary>
        /// Splits each line of code into a string array of each word
        /// </summary>
        /// <param name="lines">Lines of code to split</param>
        private List<string[]> SplitLines(List<string> lines)
        {
            List<string[]> code = new List<string[]>();
            lines.ForEach(l => code.Add(l.ToUpper().Split(' ')));
            return code;
        }

        /// <summary>
        /// Delabels each line of code and replaces labels by their correct line number
        /// </summary>
        /// <param name="labeled"></param>
        private List<string[]> Delabel(List<string[]> labeled)
        {
            List<Label> labels = new List<Label>();
            List<string[]> code = new List<string[]>();

            for (int i = 0; i < labeled.Count; i++)
            {
                string[] line = labeled[i];
                string label = line[0];
                if (Label.IsLabel(label))
                {
                    labels.Add(new Label(label, i));
                    string[] removed = new string[line.Length - 1];
                    for (int j = 1; j < line.Length; j++)
                    {
                        removed[j - 1] = line[j];
                    }
                    code.Add(removed);
                }
                else { code.Add(line); }
            }

            for (int i = 0; i < code.Count; i++)
            {
                string[] line = code[i];
                if (line.Length == 2)
                {
                    foreach (Label label in labels)
                    {
                        if (line[1] == label.name) { line[1] = label.line.ToString(); break; }
                    }
                }
            }

            return code;
        }

        /// <summary>
        /// Tries to parse each delabeled line of code into instruction code and returns the resulting memory array
        /// </summary>
        /// <param name="code">Delabeled code to parse</param>
        /// <param name="instructions">Value to store the result into</param>
        private bool TryParseInstructions(List<string[]> code, ref UNum[] instructions)
        {
            UNum[] temp = new UNum[code.Count];
            for(int i = 0; i < code.Count; i++)
            {
                string[] line = code[i];
                if (line.Length <= 0 || line.Length > 2) { return false; }

                Instruction instruction;
                if (!charCodes.TryGetValue(line[0], out instruction)) { return false; }
                int address = 0;
                if (line.Length == 2 && !int.TryParse(line[1], out address) && address >= 0 && address <= 99) { return false; }
                temp[i] = (int)instruction + address;
            }
            instructions = temp;
            return true;
        }
        #endregion
    }
}

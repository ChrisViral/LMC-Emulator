using System;
using System.Collections.Generic;
using System.Linq;

namespace LMC_Virtual_Machine
{
    public static class Utils
    {
        #region Methods
        /// <summary>
        /// Prints the memory number array
        /// </summary>
        /// <param name="instructions">Memory to print</param>
        public static string PrintInstructions(UNum[] instructions)
        {
            return "[ " + String.Join(",\n  ", instructions.Select(i => i.ToString())) + " ]";
        }

        /// <summary>
        /// Prints the code lines
        /// </summary>
        /// <param name="lines">Lines to print</param>
        public static string PrintCode(List<string[]> lines)
        {
            return String.Join("\n", lines.Select(s => String.Join(" ", s)));
        }
        #endregion
    }
}

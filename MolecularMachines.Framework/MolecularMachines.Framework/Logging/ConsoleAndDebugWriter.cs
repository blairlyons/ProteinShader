using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Logging
{
    /// <summary>
    /// <see cref="ILogWriter"/> that writes the messages to the <see cref="Console"/> and <see cref="Debug"/> output.
    /// </summary>
    class ConsoleAndDebugWriter : ILogWriter
    {
        public void Info(string message)
        {
            WriteColoredLine(ConsoleColor.White, message);
        }

        public void Warn(string message)
        {
            WriteColoredLine(ConsoleColor.DarkYellow, message);
        }

        public void Error(string message)
        {
            WriteColoredLine(ConsoleColor.Red, message);
        }

        private void WriteColoredLine(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();

            Debug.WriteLine(message);
        }
    }
}

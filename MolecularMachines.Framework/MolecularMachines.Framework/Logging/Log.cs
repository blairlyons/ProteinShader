using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Logging
{
    /// <summary>
    /// Static logging class.
    /// Use <see cref="Info(string)"/>, <see cref="Warn(string)"/>, <see cref="Error(string)"/> to log messages.
    /// Replace the log output by setting a new <see cref="ILogWriter"/> to the <see cref="Writer"/> property.
    /// When not changes, the log messages will be written to the <see cref="Console"/> and <see cref="System.Diagnostics.Debug"/> output using the <see cref="ConsoleAndDebugWriter"/>.
    /// </summary>
    public static class Log
    {
        static Log()
        {
            Writer = new ConsoleAndDebugWriter();
        }

        /// <summary>
        /// Writer that writes the messages to the log. When set to null, the logging is disabled.
        /// </summary>
        public static ILogWriter Writer { get; set; }

        /// <summary>
        /// Log info message
        /// </summary>
        /// <param name="message">message</param>
        public static void Info(string message)
        {
            Writer?.Info(message);
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="message">message</param>
        public static void Warn(string message)
        {
            Writer?.Warn(message);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="message">message</param>
        public static void Error(string message)
        {
            Writer?.Error(message);
        }
    }
}

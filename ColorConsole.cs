using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace smsync
{
    static class ColorConsole
    {
        /// <summary>
        /// Writes a message to the console.
        /// Use the "@" character to change foreground colours, eg. "@Red;This is red."
        /// Use the "@@" sequence to change background colours, eg. "@@Red; This has a red background."
        /// Available colours are listed in the system.ConsoleColor enum
        /// To escape this sequence, use the backslash, eg. "\@", you may need to use a double backslash if not using a string literal
        /// </summary>
        /// <param name="message">The message to display</param>
        public static void Write(string message) {
            var rx = new Regex(@"(?<!\\)(@@?)(\w+?);", RegexOptions.Compiled);
            int offset = 0;
            foreach (Match match in rx.Matches(message)) {
                string m = message.Substring(offset, match.Index - offset);
                offset += match.Index + match.Length;
                Console.Write(m);
                var consoleColor = Enum.Parse<ConsoleColor>(match.Groups[1].Value, true);
                if (match.Groups[0].Value == "@")
                    Console.ForegroundColor = consoleColor;
                else
                    Console.BackgroundColor = consoleColor;
            }

            Console.Write(message.Substring(offset));
        }
        /// <summary>
        /// Writes a message to the console followed by a line break.
        /// Use the "@" character to change foreground colours, eg. "@Red;This is red."
        /// Use the "@@" sequence to change background colours, eg. "@@Red; This has a red background."
        /// Available colours are listed in the system.ConsoleColor enum
        /// To escape this sequence, use the backslash, eg. "\@", you may need to use a double backslash if not using a string literal
        /// </summary>
        /// <param name="message">The message to display</param>
        public static void WriteLine(string message)
        {
            Write(message + "\n");
        }
    }
}

using System;
using System.CommandLine;

namespace smsync
{
    class Program
    {
        static void Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand(
              description: "Converts an image file from one format to another."
              , treatUnmatchedTokensAsErrors: true);
        }
    }
}

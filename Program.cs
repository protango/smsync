using CommandLine;
using LibGit2Sharp;
using System;
using System.IO;

namespace smsync
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser(with => with.EnableDashDash = true);
            parser.ParseArguments<Options>(args).WithParsed(Smsync);
        }

        static void Smsync(Options options) 
        {
            if (options.Path == null)
                options.Path = Directory.GetCurrentDirectory();

            string repoPath = Repository.Discover(options.Path);
            if (repoPath == null)
                throw new Exception("This is not a valid git repository");

            using (var repo = new Repository(repoPath))
            {
            }
        }
    }
}

using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace smsync
{
    class Options
    {
        [Option('p', "path", HelpText = "Path to git repository. Defaults to current working directory.")]
        public string Path { get; set; }

        [Option('b', "branch", HelpText = "Branch to checkout, master will be used if specified branch does not exist.", Default = "master")]
        public string Branch { get; set; }
    }
}

using CommandLine;
using LibGit2Sharp;
using System;
using System.IO;
using System.Linq;

namespace smsync
{
    class Program
    {
        static private PowershellTerminal psTerm;

        static void Main(string[] args)
        {
            try
            {
                psTerm = new PowershellTerminal();
                var parser = new Parser(with => with.EnableDashDash = true);
                parser.ParseArguments<Options>(args).WithParsed(Smsync);
            }
            finally 
            {
                psTerm.Dispose();
            }
        }

        static void Smsync(Options options) 
        {
            if (options.Path == null)
                options.Path = Directory.GetCurrentDirectory();

            var baseRepoDir = FindBaseRepo(new DirectoryInfo(options.Path));




            updateSubmodules(baseRepoDir, options);
        }

        static void updateSubmodules(DirectoryInfo dir, Options options, ObjectId commit = null) {
            if (dir.Name == ".git") dir = dir.Parent;

            using (var repo = new Repository(dir.FullName))
            {
                var branches = repo.Branches.ToArray();
                var currentBranch = branches.FirstOrDefault(x => x.IsCurrentRepositoryHead);

                // try to guess branch
                if (commit != null) { 
                    
                }

                var a = (commit != null ? branches.Where(x => x.Commits.Take(1000).Any(y => y.Id.Sha == commit.Sha)) : null);
                Branch targetBranch =
                    (commit != null ? branches.FirstOrDefault(x => x.Commits.Take(1000).Any(y => y.Id.Sha == commit.Sha)) : null) ??
                    (options.Branch != null ? branches.FirstOrDefault(x => x.FriendlyName == options.Branch) : null) ??
                    (branches.FirstOrDefault(x => x.IsCurrentRepositoryHead)) ??
                    (branches.FirstOrDefault(x => x.FriendlyName == "master"));
                if (targetBranch == null) throw new Exception($"Could not establish a branch to checkout for repository \"{dir.Name}\"");

                foreach (var sm in repo.Submodules) {
                    updateSubmodules(new DirectoryInfo(Path.Combine(dir.FullName, sm.Path)), options, sm.HeadCommitId);
                }

                

                Console.WriteLine(targetBranch.FriendlyName);
            }
        }

        static DirectoryInfo FindBaseRepo(DirectoryInfo dir) {
            string repoPath = Repository.Discover(dir.FullName);
            if (repoPath == null)
                throw new Exception("This is not a valid git repository");

            DirectoryInfo repoDir = new DirectoryInfo(repoPath);
            while (repoDir.Name != ".git" && repoDir != null) repoDir = repoDir.Parent;

            if (repoDir == null)
                throw new Exception("Failed to find to base repository");

            return repoDir.Parent;
        }
    }
}

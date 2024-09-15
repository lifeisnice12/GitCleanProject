using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* This program clean a project unsing .gitignore file and git functions.
 * No need to push the project to clean it.
 * Usage: gitcleanproject.exe projectPath
 * Error codes
 * 0: Success
 * 1: Missing argument
 * 2: Path not found
 * 3: Git repository already present
 * 4: repository path is not owned by current user
 * 5: Unknown error
 * 
 * https://www.digitec.ch/fr/page/astuce-windows-integrer-nimporte-quelle-commande-dans-le-menu-contextuel-30628
 * 
 */

namespace GitCleanProject
{
    internal class Program
    {
        static int nbFiles = 0;
        static int nbFolders = 0;

        static void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        static void EndsProgram(string msg)
        {
            Console.WriteLine(msg);
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        static int Main(string[] args)
        {
            string projectPath;

            if (args.Length == 0)
            {
                EndsProgram("Usage: gitcleandirectory projectPath");
                return 1;
            }

            projectPath = args[0];

            // Check if path doesn't exists
            if (!Directory.Exists(projectPath))
            {
                EndsProgram($"directory {projectPath} doesn't exist.");
                return 2;
            }

            // Verify if path contains à git repository
            if (!Repository.IsValid(projectPath))
            {
                // Initialiser un nouveau dépôt Git
                try
                {
                    Repository.Init(projectPath);
                    //Console.WriteLine("Git repository successfully created.");
                }
                catch (Exception ex)
                {
                    //Remove .git folder if created before exception
                    string gitDirectory = Path.Combine(projectPath, ".git\\");
                    DeleteDirectory(gitDirectory);

                    EndsProgram(ex.Message);
                    return 4;
                }
            }
            else
            {
                EndsProgram("A git repository already exists. It would be dangerous to modify it.");
                return 3;
            }

            // Open Git repository
            using (var repo = new Repository(projectPath))
            {
                //Console.WriteLine("Git repository open.");

                // Get repository status
                var status = repo.RetrieveStatus();

                // Makes an ititial commit if needed
                if (status.IsDirty)
                {
                    Commands.Stage(repo, "*");
                    Signature author = new Signature("Olivier Bel", "obel@bluewin.ch", DateTime.Now);
                    repo.Commit("Initial commit", author, author);
                }

                // Get status again after commit
                status = repo.RetrieveStatus();

                // Browse repository informations after last commit
                try
                {
                    foreach (var item in status)
                    {
                        if (item.FilePath.EndsWith("/")) // A folder ?
                        {
                            string itemFilePath = item.FilePath.Replace("/", "\\");
                            string directory = Path.Combine(projectPath, itemFilePath.Substring(0, itemFilePath.Length - 1));
                            //Console.WriteLine(directory);
                            Directory.Delete(directory, true);
                            nbFolders++;
                        }
                        else
                        {
                            string file = Path.Combine(projectPath, item.FilePath);
                            //Console.WriteLine(file);
                            File.Delete(file);
                            nbFiles++;
                        }
                    }
                    Console.WriteLine($"{nbFolders} folder(s) were removed\n{nbFiles} file(s) were deleted");
                }
                catch (Exception ex)
                {
                    //Remove .git folder since it was created before exception
                    string gitDirectory = Path.Combine(projectPath, ".git\\");
                    DeleteDirectory(gitDirectory);

                    EndsProgram(ex.Message);
                    return 5;
                }
            }

            //Remove .git folder
            string gitDir = Path.Combine(projectPath, ".git\\");
            DeleteDirectory(gitDir);

            // Alternatives to delete this folder, not working:
            //Directory.Delete(gitDir, true);
            //Process.Start("cmd.exe", "/c " + @"rmdir /s /q " + gitDir);

            EndsProgram("");
            return 0;
        }
    }
}

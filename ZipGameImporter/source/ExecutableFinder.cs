using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipGameImporter
{
    public class ExecutableFinder
    {
        // EXEs that are almost never the game
        private readonly string[] IgnoreNames =
        {
            "unins",
            "uninstall",
            "setup",
            "crashreport",
            "crashhandler",
            "launcherhelper",
            "dxsetup",
            "vc_redist",
            "redist",
            "eula",
            "config",
            "settings",
            "updater",
            "update"
        };

        public string FindExecutable(string gameFolder, string gameName)
        {
            if (!Directory.Exists(gameFolder))
                return null;

            var executables = Directory
                .GetFiles(gameFolder, "*.exe", SearchOption.AllDirectories)
                .Select(file => new FileInfo(file))
                .ToList();

            if (!executables.Any())
                return null;

            var ranked = executables
                .Select(exe => new
                {
                    File = exe,
                    Score = CalculateScore(exe, gameFolder, gameName)
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.File.Length)
                .ToList();

            return ranked.First().File.FullName;
        }

        private int CalculateScore(FileInfo exe, string rootFolder, string gameName)
        {
            int score = 0;

            string exeName = Path.GetFileNameWithoutExtension(exe.Name).ToLowerInvariant();
            string game = gameName.ToLowerInvariant();

            // Exact game name
            if (exeName == game)
                score += 1000;

            // Contains game name
            if (exeName.Contains(game))
                score += 500;

            // Root folder
            if (exe.DirectoryName == rootFolder)
                score += 300;

            // Ignore unwanted EXEs
            foreach (var ignore in IgnoreNames)
            {
                if (exeName.Contains(ignore))
                    score -= 1000;
            }

            // Prefer larger EXEs
            score += (int)(exe.Length / 1048576);

            // Prefer shorter paths
            int depth = exe.FullName.Replace(rootFolder, "")
                                    .Split(Path.DirectorySeparatorChar)
                                    .Length;

            score -= depth * 5;

            return score;
        }
    }
}

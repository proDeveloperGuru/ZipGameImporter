using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZipGameImporter
{
    internal class VersionParser
    {
        private Logger logger;
        public VersionParser(Logger logger) {
            this.logger = logger; 
        }

        public class ParsedGame
        {
            public string Name { get; set; }
            public Version Version { get; set; }
            public string VersionString { get; set; }
            public bool Success { get; set; }
        }

        private static readonly Regex VersionRegex =
            new Regex(@"(?i)\bv?(?<version>\d+(?:\.\d+){1,3})(?:[-_](?:beta|alpha|rc\d*))?\b", RegexOptions.Compiled);
                      

        private static readonly string[] JunkWords =
{
            "portable",
            "hotfix",
            "update",
            "patched",
            "release",
            "repack",
            "pc",
            "mac"
        };

        private static string NormalizeGameName(string gameName)
        {
            foreach (string word in JunkWords)
            {
                gameName = Regex.Replace(
                    gameName,
                    $@"\b{Regex.Escape(word)}\b",
                    "",
                    RegexOptions.IgnoreCase);
            }

            gameName = Regex.Replace(gameName, @"[\-_]+", " ");

            gameName = Regex.Replace(gameName, @"\s+", " ");

            gameName = gameName.Trim();

            return gameName;
        }

        private Version NormalizeVersion(string version)
        {
            string[] parts = version.Split('.');

            switch (parts.Length)
            {
                case 1:
                    return new Version(
                        int.Parse(parts[0]),
                        0);

                case 2:
                    return new Version(
                        int.Parse(parts[0]),
                        int.Parse(parts[1]));

                case 3:
                    return new Version(
                        int.Parse(parts[0]),
                        int.Parse(parts[1]),
                        int.Parse(parts[2]));

                default:
                    return new Version(
                        int.Parse(parts[0]),
                        int.Parse(parts[1]),
                        int.Parse(parts[2]),
                        int.Parse(parts[3]));
            }
        }

        public ParsedGame Parse(string zipFile)
        {
            var result = new ParsedGame();
            string fileName = Path.GetFileNameWithoutExtension(zipFile);

            Match match = VersionRegex.Match(fileName);

            if (!match.Success)
            {
                result.Success = false;
                return result;
            }

            result.VersionString = match.Groups["version"].Value.Trim();

            this.logger.Info("Version number: " + result.VersionString);
            result.Name = NormalizeGameName(fileName.Remove(match.Index, match.Length));

            try
            {
                result.Version = NormalizeVersion(result.VersionString);
                result.Success = true;
            }
            catch
            {
                result.Success = false;
            }

            return result;
        }


        public bool IsNewer(string zipVersion, string installedVersion)
        {
            Version zip = NormalizeVersion(zipVersion);

            Version installed;

            if (!Version.TryParse(installedVersion, out installed))
                installed = new Version(0, 0);

            return zip > installed;
        }
    }
}

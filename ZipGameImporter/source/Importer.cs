using Playnite.SDK.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZipGameImporter
{
    internal class Importer
    {
        private readonly VersionParser versionParser;
        private readonly ZipExtractor zipExtractor;
        private readonly ExecutableFinder executableFinder;
        private readonly PlayniteUpdater playniteUpdater;
        private readonly Logger logger;
        private readonly Action<Action> runOnMainThread;

        public Importer(PlayniteUpdater updater, Action<Action> runOnMainThread)
        {
            logger = new Logger();
            versionParser = new VersionParser(logger);
            executableFinder = new ExecutableFinder();
            zipExtractor = new ZipExtractor(logger);

            playniteUpdater = updater;
            this.runOnMainThread = runOnMainThread;
        }

        private (bool, string) ExtractAndFindExecutable(string zipFile, string name, string installFolder)
        {
            bool extracted =  
                zipExtractor.ExtractSafely(
                    zipFile,
                    installFolder);

            if (!extracted)
            {
                logger.Error(
                     "Extraction failed.");

                return (false,null);
            }

            string executable =
                executableFinder.FindExecutable(
                    installFolder,
                    name);

            if (executable == null)
            {
                logger.Error(
                    "Executable not found.");

                return (false, null);
            }

            return (extracted, executable);
        }

        private void ProcessZip(
            string zipFile,
            string gamesFolder)
        {
            try
            {
                logger.Info(
                    $"Processing {Path.GetFileName(zipFile)}");


                var parsed =
                    versionParser.Parse(zipFile);

                if (!parsed.Success)
                {
                    logger.Error(
                        "Could not parse version.");

                    return;
                }

                logger.Info($"Game: {parsed.Name}");

                logger.Info($"Version: {parsed.VersionString}");

                string installFolder =
                    Path.Combine(
                        gamesFolder,
                        parsed.Name);

                runOnMainThread(() =>
                {
                    UpdatePlaynite(
                        parsed.Name,
                        parsed.VersionString,
                        installFolder,
                        zipFile);
                });
            }
            catch (Exception ex) {
                logger.Error(
                    "Processing failed: " + ex.Message);
            }
        }

        private void UpdatePlaynite(string name,
            string version,
            string installFolder,
            string zipFile)
        {
            try
            {
                Game playniteGame = playniteUpdater.FindGame(name);

                if (playniteGame == null)
                {
                    logger.Info(
                        "Game not found in Playnite. Adding game to database");

                    var result = ExtractAndFindExecutable(zipFile, name, installFolder);

                    bool added =
                        playniteUpdater.AddGame(
                            name,
                            version,
                            installFolder,
                            result.Item2);

                    if (added)
                        logger.Info(name + " (" + version + ") added");
                    else
                        logger.Error("Playnite add failed.");
                }
                else
                {
                    if (!ShouldUpdate(
                        playniteGame,
                        installFolder,
                        version))
                    {
                        logger.Warning(
                            "Already latest version.");

                        return;
                    }

                    var result = ExtractAndFindExecutable(zipFile, name, installFolder);

                    bool updated =
                        playniteUpdater.UpdateGame(
                            name,
                            version,
                            installFolder,
                            result.Item2);

                    if (updated)
                        logger.Info("Playnite updated.");
                    else
                        logger.Error("Playnite update failed.");
                }
            }
            catch (Exception ex) {
                logger.Error(
                    "Playnite update failed: " + ex.Message);
            }
        }

        private bool ShouldUpdate(
            Game game,
            string installFolder,
            string zipVersion)
        {
            if (string.IsNullOrEmpty(game.Version))
                return true;

            string executable =
                executableFinder.FindExecutable(
                installFolder,
                game.Name);

            if (string.IsNullOrEmpty(executable))
                return true;

            return versionParser.IsNewer(
                zipVersion,
                game.Version);
        }

        public void ImportFolder(
            string zipFolder,
            string gamesFolder)
        {
            if (!Directory.Exists(zipFolder))
            {
                Console.WriteLine("ZIP folder not found.");
                return;
            }


            var zipFiles = Directory
                .GetFiles(
                    zipFolder,
                    "*.zip",
                    SearchOption.TopDirectoryOnly);


            foreach (string zip in zipFiles)
            {
                ProcessZip(zip, gamesFolder);
            }
        }
    }
}

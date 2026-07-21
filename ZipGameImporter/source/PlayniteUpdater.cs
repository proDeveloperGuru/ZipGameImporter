using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using ZipGameImporter.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ZipGameImporter
{
    internal class PlayniteUpdater
    {
        private readonly IPlayniteAPI api;
        private readonly Logger logger;

        public PlayniteUpdater(IPlayniteAPI playniteApi, Logger logger)
        {
            api = playniteApi;
            this.logger = logger;
        }

        public Game FindGame(string gameName)
        {
            return api.Database.Games
                .FirstOrDefault(g =>
                    string.Equals(
                        g.Name.NoSpace(),
                        gameName.NoSpace(),
                        StringComparison.OrdinalIgnoreCase));
        }

        private void UpdateCategories(Game game, ImportOption option)
        {
            foreach (var category in option.SelectedCategories)
            {
                if (!game.Categories.Any(x => x.Id == category.Id))
                {
                    game.Categories.Add(category);
                }
            }
        }

        public void DebugSource(Game game)
        {
            logger.Info(JsonConvert.SerializeObject(game));
        }

        public bool UpdateGame(
            string gameName,
            string version,
            string installDirectory,
            string executable,
            ImportOption option)
        {
            try
            {
                Game game = FindGame(gameName);

                if (game == null)
                    throw new Exception("Game not found: " + gameName);

                game.Version = version;
                game.IsInstalled = true;
                game.InstallDirectory = installDirectory;

                game.Hidden = option.Hidden;

                if (option.SourceId != null)
                    game.SourceId = option.SourceId.Value;

                UpdateCategories(game, option);
                UpdatePlayAction(game, executable);

                DebugSource(game);

                api.Database.Games.Update(game);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Failed to update game: " + ex.Message);

                return false;
            }
        }

        public bool AddGame(
            string gameName,
            string version,
            string installDirectory,
            string executable,
            ImportOption option)
        {
            try
            {
                Game game = FindGame(gameName);

                if (game != null)
                    throw new Exception("Game is already created: " + gameName);

                game = new Game(gameName);

                game.Version = version;
                game.IsInstalled = true;
                game.InstallDirectory = installDirectory;

                game.Hidden = option.Hidden;

                if(option.SourceId != null)
                    game.SourceId = option.SourceId.Value;

                UpdatePlayAction(game, executable);

                api.Database.Games.Add(game);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Failed to add game: " + ex.Message);

                return false;
            }
        }

        private void UpdatePlayAction(Game game, string executable)
        {
            if (game.GameActions == null)
            {
                game.GameActions =
                    new ObservableCollection<GameAction>();
            }

            GameAction playAction =
                game.GameActions
                    .FirstOrDefault(a => a.IsPlayAction);

            if (playAction == null)
            {
                playAction = new GameAction
                {
                    Name = "Play",
                    IsPlayAction = true,
                    Type = GameActionType.File
                };

                game.GameActions.Add(playAction);
            }

            playAction.Path = executable;

            playAction.WorkingDir =
                System.IO.Path.GetDirectoryName(executable);

            playAction.TrackingPath =
                System.IO.Path.GetDirectoryName(executable);
        }

        public bool UpdateVersionOnly(string gameName, string version)
        {
            Game game = FindGame(gameName);

            if (game == null)
                return false;

            game.Version = version;

            api.Database.Games.Update(game);

            return true;
        }

        public bool UpdateInstallDirectory(string gameName, string directory)
        {
            Game game = FindGame(gameName);

            if (game == null)
                return false;

            game.InstallDirectory = directory;

            api.Database.Games.Update(game);

            return true;
        }

        public bool UpdateExecutable(string gameName, string executable)
        {
            Game game = FindGame(gameName);

            if (game == null)
                return false;

            UpdatePlayAction(game, executable);

            api.Database.Games.Update(game);

            return true;
        }
    }
}

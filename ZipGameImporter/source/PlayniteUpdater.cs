using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ZipGameImporter
{
    internal class PlayniteUpdater
    {
        private readonly IPlayniteAPI api;

        public PlayniteUpdater(IPlayniteAPI playniteApi)
        {
            api = playniteApi;
        }

        public Game FindGame(string gameName)
        {
            return api.Database.Games
                .FirstOrDefault(g =>
                    string.Equals(
                        g.Name,
                        gameName,
                        StringComparison.OrdinalIgnoreCase));
        }

        public bool UpdateGame(
            string gameName,
            string version,
            string installDirectory,
            string executable)
        {
            try
            {
                Game game = FindGame(gameName);

                if (game == null)
                    return false;

                game.Version = version;
                game.Hidden = true;
                game.IsInstalled = true;
                game.InstallDirectory = installDirectory;

                UpdatePlayAction(game, executable);

                api.Database.Games.Update(game);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddGame(
            string gameName,
            string version,
            string installDirectory,
            string executable)
        {
            try
            {
                Game game = FindGame(gameName);

                if (game != null)
                    return false;

                game = new Game(gameName);

                game.Version = version;
                game.Hidden = true;
                game.IsInstalled = true;
                game.InstallDirectory = installDirectory;

                UpdatePlayAction(game, executable);

                api.Database.Games.Add(game);

                return true;
            }
            catch (Exception ex)
            {
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

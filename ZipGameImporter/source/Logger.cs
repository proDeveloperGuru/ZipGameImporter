using System;
using System.IO;

namespace ZipGameImporter
{
    internal class Logger
    {
        private readonly string logFile;
        private readonly object lockObject = new object();

        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        public Logger()
        {
            string folder =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData),
                    "Playnite",
                    "Extensions",
                    "ZipGameImporter",
                    "Logs");


            Directory.CreateDirectory(folder);


            string filename =
                $"Import_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";


            logFile =
                Path.Combine(
                    folder,
                    filename);


            Write(
                "Logger started",
                LogLevel.Info);
        }



        public void Info(string message)
        {
            Write(message, LogLevel.Info);
        }



        public void Warning(string message)
        {
            Write(message, LogLevel.Warning);
        }



        public void Error(string message)
        {
            Write(message, LogLevel.Error);
        }



        public void Exception(Exception ex)
        {
            Write(
                $"{ex.Message}\n{ex.StackTrace}",
                LogLevel.Error);
        }



        private void Write(
            string message,
            LogLevel level)
        {
            try
            {
                lock (lockObject)
                {
                    string line =
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                        $"[{level}] {message}";


                    File.AppendAllText(
                        logFile,
                        line + Environment.NewLine);
                }
            }
            catch
            {
                // Logging must never crash the importer
            }
        }



        public string GetLogFile()
        {
            return logFile;
        }
    }
}

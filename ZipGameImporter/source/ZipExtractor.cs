using System;
using System.IO;
using System.IO.Compression;

namespace ZipGameImporter
{
    internal class ZipExtractor
    {
        private readonly Logger logger;
        public ZipExtractor(Logger loger)
        {
            logger = loger;
        }
        public bool Extract(string zipFile, string destinationFolder, bool overwrite)
        {
            try
            {
                if (!File.Exists(zipFile))
                {
                    logger.Warning($"ZIP file not found: {zipFile}");
                    return false;
                }

                if (Directory.Exists(destinationFolder))
                {
                    if (!overwrite)
                    {
                        logger.Warning($"Destination already exists: {destinationFolder}");
                        return false;
                    }

                    Directory.Delete(destinationFolder, true);
                }

                Directory.CreateDirectory(destinationFolder);

                ZipFile.ExtractToDirectory(zipFile, destinationFolder);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"Extraction failed: {ex.Message}");
                return false;
            }
        }

        // ToDo: Must find a way to get rid top level folder on extraction, since we already create a folder for each game
        public bool ExtractSafely(string zipFile, string destinationFolder)
        {
            string tempFolder = destinationFolder + "_temp";

            try
            {
                if (!File.Exists(zipFile))
                    return false;

                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);

                Directory.CreateDirectory(tempFolder);

                ZipFile.ExtractToDirectory(zipFile, tempFolder);

                if (Directory.Exists(destinationFolder))
                    Directory.Delete(destinationFolder, true);

                Directory.Move(tempFolder, destinationFolder);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);

                return false;
            }
        }
    }
}

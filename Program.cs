using NLog;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace DonationDowloader
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    bool failedToRead = false;
                    bool failedToWrite = false;
                    bool failedToReadCurrent = false;

                    string[] config = File.ReadAllLines(@"config.txt");
                    string inPath = config?[0];
                    if (File.Exists(inPath))
                    {
                        string[] lines = new string[0];
                        try
                        {
                            lines = File.ReadAllLines(inPath);
                            failedToRead = false;
                            logger.Info(DateTime.Now + " New File Received");
                            Console.WriteLine(DateTime.Now + " New File Received");
                        }
                        catch
                        {
                            failedToRead = true;
                            logger.Error(DateTime.Now + " Failed to Read");
                            Console.WriteLine(DateTime.Now + " Failed to Read");
                        }

                        foreach (var line in lines)
                        {
                            if (line.Contains("https://cravatar.eu/helmavatar/"))
                            {
                                string rx = @"helmavatar/(?<name>.*)/([0-9])";

                                var newNames = Regex.Matches(line, rx)
                                                .OfType<Match>()
                                                .Select(m => m.Groups["name"].Value)
                                                .ToArray();

                                string outPath = @"c:\out\capes.txt";
                                if (!File.Exists(outPath))
                                {
                                    File.CreateText(outPath);
                                }

                                string[] currentNames = new string[0];
                                try
                                {
                                    currentNames = File.ReadAllLines(outPath);
                                    failedToReadCurrent = false;
                                }
                                catch
                                {
                                    failedToReadCurrent = true;
                                    logger.Error(DateTime.Now + " Failed to Read Current");
                                    Console.WriteLine(DateTime.Now + " Failed to Read Current");
                                }
                                foreach (var currentName in currentNames)
                                {
                                    newNames = newNames.Where(o => o != currentName.Split(':')[0]).ToArray();
                                }

                                try
                                {
                                    using (StreamWriter sw = File.AppendText(outPath))
                                    {
                                        foreach (var newName in newNames)
                                        {
                                            sw.WriteLine(newName + ":capedonor");
                                            logger.Info(DateTime.Now + " Added " + newName);
                                            Console.WriteLine(DateTime.Now + " Added " + newName);
                                        }
                                    }
                                    failedToWrite = false;
                                }
                                catch
                                {
                                    failedToWrite = true;
                                    logger.Error(DateTime.Now + " Failed to Write");
                                    Console.WriteLine(DateTime.Now + " Failed to Write");
                                }
                            }
                        }
                        try
                        {
                            if (!failedToRead && !failedToWrite && !failedToReadCurrent)
                                File.Delete(inPath);
                        }
                        catch { }
                    }
                    Thread.Sleep(30000);
                }
            }
            finally
            {
                Console.Write("Press Enter to close window ...");
                Console.Read();
            }
        }
    }
}

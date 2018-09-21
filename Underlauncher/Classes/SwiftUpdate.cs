using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Underlauncher
{
    public static class SwiftUpdate
    {
        private static string _Application;
        private static string _ServerFolder;
        private static string _UpdateFolder;
        private static string _TempFolder;

        private static int _BadFilesReattemptCount = 0;

        private static Logger _Log;

        private static List<FileEntry> _NewFiles;

        private static WebClient client;
        private static bool _ShouldAbort = false;

        public static void Reset()
        {
            _NewFiles = new List<FileEntry>();

            if (_TempFolder != null)
            {
                Directory.Delete(_TempFolder, true);
            }

            _ServerFolder = "";
            _UpdateFolder = "";
            _TempFolder = "";
            _BadFilesReattemptCount = 0;
            _ShouldAbort = false;
        }

        public static async Task<bool> CheckForUpdates(string appName, string serverFolder)
        {
            _Application = appName;
            _ServerFolder = serverFolder;
            _UpdateFolder = Environment.GetEnvironmentVariable("LocalAppData") + "//" + appName + "//";
            _TempFolder = _UpdateFolder + "TemporaryDownloads//";   // Set private variables
            _Log = new Logger(_UpdateFolder + "\\Update.log");
            _NewFiles = new List<FileEntry>();

            Directory.CreateDirectory(_TempFolder);
            Directory.CreateDirectory(_TempFolder + "\\" + _Application + "\\");   //Create directories in preparation for download

            bool installerVerified = false;
            bool shouldRetry = true;
            int retryCount = 0;

            if (!Directory.Exists(_UpdateFolder))
            {
                Directory.CreateDirectory(_UpdateFolder);
                _Log.WriteEntry("Update folder \"" + _UpdateFolder + "\" didn't exist, creating it and returning true (an update is required)");
                return true;
            }

            else if (!Directory.Exists(_UpdateFolder + "Installer"))
            {
                Directory.CreateDirectory(_UpdateFolder + "Installer");
                _Log.WriteEntry("Installer folder didn't exist, creating directory " + _UpdateFolder + "Installer, and forcing an update.");
                return true;
            }

            _Log.WriteEntry("Downloading Version.md5 from server, saving in \"" + _TempFolder + "\" as " + SwiftUpdateConstants.newVersionMD5Name);
            await DownloadFile(_ServerFolder + SwiftUpdateConstants.versionMD5Name, _TempFolder + SwiftUpdateConstants.newVersionMD5Name);  //Download the Version.md5 from the server

            while (shouldRetry && !_ShouldAbort) //While we should reattempt
            {
                if (retryCount < SwiftUpdateConstants.RetryCount)  //If we've not exceeded the max reattempts
                {
                    if (!installerVerified) //If we haven't verified the installer's version, download it
                    {
                        _Log.WriteEntry("Downloading installer.md5.");
                        await DownloadFile(_ServerFolder + "Installer/" + _Application + " Installer.md5", _UpdateFolder + "Installer//Installer.new.md5"); //Downloading installer checksum

                        if (SwiftUpdateUtilities.ReadMD5FromFile(_UpdateFolder + "Installer//Installer.new.md5") != SwiftUpdateUtilities.CalculateMD5(_UpdateFolder + "Installer//Installer.exe")) //Comparing md5 of the downloaded checksum to the local installer
                        {
                            _Log.WriteEntry("Local md5 did not match download, downloading new installer.");
                            await DownloadFile(_ServerFolder + "Installer/" + _Application + " Installer.exe", _UpdateFolder + "Installer//Installer.new.exe"); //Downloading installer exe

                            if (SwiftUpdateUtilities.ReadMD5FromFile(_UpdateFolder + "Installer//Installer.new.md5") == SwiftUpdateUtilities.CalculateMD5(_UpdateFolder + "Installer//Installer.new.exe"))  //If the checksum of the new installer matches the md5 we downloaded earlier
                            {
                                _Log.WriteEntry("Downloaded checksums matched, new installer verified.");
                                File.Delete(_UpdateFolder + "Installer//Installer.md5");
                                File.Delete(_UpdateFolder + "Installer//Installer.exe");    //Delete the old installer files
                                File.Move(_UpdateFolder + "Installer//Installer.new.md5", _UpdateFolder + "Installer//Installer.md5");
                                File.Move(_UpdateFolder + "Installer//Installer.new.exe", _UpdateFolder + "Installer//Installer.exe");  //Copy over the new installer files
                                installerVerified = true;
                                retryCount = 0; //We've verified the new installer, so we reset our reattempts for the update files to 0
                            }

                            else   //The new installer's checksum does NOT match the one from earlier, we begin the process again
                            {
                                _Log.WriteEntry("Downloaded installer checksums did not match, retrying...");
                            }
                        }

                        else   //If the checksum we downloaded DOES match our local copy, no new installer required
                        {
                            _Log.WriteEntry("No new installer available.");
                            File.Delete(_UpdateFolder + "Installer//Installer.new.md5");
                            installerVerified = true;
                            retryCount = 0; //We've verified the new installer, so we delete the md5 we downloaded and reset our reattempts for the update files to 0
                        }
                    }

                    else    //The installer is verified, so we move on to the update files
                    {
                        if (!File.Exists(_UpdateFolder + SwiftUpdateConstants.versionMD5Name))   //If our local copy doesn't exist
                        {
                            _Log.WriteEntry("Our local Version.md5 at directory \"" + _UpdateFolder + SwiftUpdateConstants.versionMD5Name + "\" didn't exist, update required.");
                            return true;    //We redownload the update
                        }

                        else
                        {
                            if (SwiftUpdateUtilities.ReadMD5FromFile(_UpdateFolder + SwiftUpdateConstants.versionMD5Name) == SwiftUpdateUtilities.ReadMD5FromFile(_TempFolder + SwiftUpdateConstants.newVersionMD5Name))    //If the Version.md5 we downloaded matches our local copy
                            {
                                _Log.WriteEntry("Compared md5s, no update is required, deleting temp.");
                                Directory.Delete(_TempFolder, true);
                                return false;   //No update is available
                            }

                            else    //Our downloaded Version.md5 does NOT match our local copy, possibly an update available
                            {
                                _Log.WriteEntry("Our downloaded Version.md5 did not match our local copy. Downloading Version.xml from server, saving in \"" + _TempFolder + "\" as " + SwiftUpdateConstants.newVersionXMLName);
                                await DownloadFile(_ServerFolder + SwiftUpdateConstants.versionXMLName, _TempFolder + SwiftUpdateConstants.newVersionXMLName);  //We download the new Version.xml file from the server and store in in a temp folder
                                _Log.WriteEntry("Checksum comparison. " + SwiftUpdateUtilities.ReadMD5FromFile(_TempFolder + SwiftUpdateConstants.newVersionMD5Name) + " | " + SwiftUpdateUtilities.CalculateMD5(_TempFolder + SwiftUpdateConstants.newVersionXMLName));
                                if (SwiftUpdateUtilities.ReadMD5FromFile(_TempFolder + SwiftUpdateConstants.newVersionMD5Name) == SwiftUpdateUtilities.CalculateMD5(_TempFolder + SwiftUpdateConstants.newVersionXMLName))  //If the checksum we downloaded earlier matches the calculated checksum of the xml file (integrity guarenteed)
                                {
                                    _Log.WriteEntry("Checksum of Version.xml matches our previously downloaded Version.md5, checking versions now.");
                                    if (SwiftUpdateXML.GetVersionNumber(_TempFolder + SwiftUpdateConstants.newVersionXMLName).CompareTo(SwiftUpdateXML.GetVersionNumber(_UpdateFolder + SwiftUpdateConstants.versionXMLName)) > 0) //If the xml file we downloaded has a greater version number than our local copy
                                    {
                                        _Log.WriteEntry("Our new file has a greater version than our local, an update is available, returning true.");
                                        return true;    //We definitely have an update
                                    }

                                    else    //The version number on the server is the same or earlier than us
                                    {
                                        _Log.WriteEntry("We have the same or a later version than the server, returning false, deleting temp.");
                                        Directory.Delete(_TempFolder, true);
                                        return false;
                                    }
                                }

                                _Log.WriteEntry("The downloaded Version.xml didn't match the downloaded checksum, retrying...");
                            }
                        }
                    }

                    retryCount++;
                }

                else
                {
                    _Log.WriteEntry("Retried max amount of times.");
                    MessageBox.Show("Failed to verify if an update was available. Please ensure that you have a stable internet connection. Otherwise, try again later. If the issue persists, contact support and provide the Update.log.", "Update Check Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                    shouldRetry = false;
                }
            }

            return false;
        }

        public static void AbortDownload()
        {
            if (client != null)    //If we have an active webclient, cancel the asynchronous download
            {
                client.CancelAsync();
            }
        }

        public static async Task DownloadUpdate(ProgressBar downloadProgBar, TextBlock downloadPercBlock, TextBlock downloadProgDescBlock)
        {
            if (await Task.Run(() => StartUpdate()))    //If we await StartUpdate and it returns true
            {
                ProgressManager.progressDescription = "Launching installer for update";

                Process instUpd = new Process();
                instUpd.StartInfo.UseShellExecute = true;
                instUpd.StartInfo.CreateNoWindow = false;
                instUpd.StartInfo.Verb = "runas";   //Run as administrator

                instUpd.StartInfo.FileName = _UpdateFolder + @"Installer\Installer.exe";

                string arguments = "Update|" + _TempFolder + "|" + _UpdateFolder + "|" + Environment.CurrentDirectory;  //Arguments: Update Flag | Temporary Download Folder | Folder containing Update | Application Install Folder

                instUpd.StartInfo.Arguments = arguments;

                _Log.WriteEntry("About to start installer for update. Filename: " + instUpd.StartInfo.FileName + "\n Args: " + arguments);

                instUpd.Start();
                Environment.Exit(0);
            }
        }

        private static async Task<bool> StartUpdate()
        {
            bool shouldRetry = true;
            int retryCount = 0;

            while (shouldRetry && !_ShouldAbort) //While we should retry and not abort
            {
                if (retryCount < SwiftUpdateConstants.RetryCount)   //If we've not exceeded the retry count
                {
                    _Log.WriteEntry("Downloading the Version xml and md5 files.");
                    await DownloadFile(_ServerFolder + SwiftUpdateConstants.versionMD5Name, _TempFolder + SwiftUpdateConstants.newFileListMD5Name); //Downloading new file list checksum file
                    await DownloadFile(_ServerFolder + SwiftUpdateConstants.versionXMLName, _TempFolder + SwiftUpdateConstants.newVersionXMLName); //Download new file list xml file

                    _Log.WriteEntry("Downloading the file list xml and md5 files.");
                    await DownloadFile(_ServerFolder + SwiftUpdateConstants.fileListMD5Name, _TempFolder + SwiftUpdateConstants.newFileListMD5Name); //Downloading new file list checksum file
                    await DownloadFile(_ServerFolder + SwiftUpdateConstants.fileListXMLName, _TempFolder + SwiftUpdateConstants.newFileListXMLName); //Download new file list xml file

                    if (SwiftUpdateUtilities.ReadMD5FromFile(_TempFolder + SwiftUpdateConstants.newFileListMD5Name) == SwiftUpdateUtilities.CalculateMD5(_TempFolder + SwiftUpdateConstants.newFileListXMLName)) //Comparing md5 of new file list to local checksum of new file list
                    {
                        _Log.WriteEntry("The checksums of the file list match, downloading new update files.");
                        if (await Task.Run(() => GetNewFiles()))    //If we've got the list of new files
                        {
                            _Log.WriteEntry("Got a list of new files.");
                            ProgressManager.SetTotalBytes(GetTotalSizeOfFiles(_NewFiles));
                            if (await Task.Run(() => DownloadNewFiles()))   //If we've downloaded the new files
                            {
                                if (await Task.Run(() => DownloadInstaller()))  //If we've downloaded the installer
                                {
                                    _Log.WriteEntry("Returning true from StartUpdate");
                                    ProgressManager.progressDescription = "Download completed.";
                                    return true;
                                }
                            }
                        }

                        //Download abort/exception if we reach here, so abort the update process
                        _ShouldAbort = true;
                        shouldRetry = false;
                        Reset();
                        _Log.WriteEntry("Experienced a download error, aborting...");
                        return false;
                    }

                    else    //The checksum we downloaded does not match the checksum of the new file list
                    {
                        _Log.WriteEntry("The checksums of the file list didn't match, retrying...");
                    }

                    retryCount++;
                }

                else    //We've exceeded the count
                {
                    Reset();
                    shouldRetry = false;
                    _Log.WriteEntry("Retried max amount of times to get file list and Version.");
                    MessageBox.Show("Failed to download initial update files. Please ensure that you have a stable internet connection. Otherwise, try again later. If the issue persists, contact support and provide your Update.log.", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return false;
        }

        private static bool GetNewFiles()
        {
            try
            {
                if (File.Exists(_UpdateFolder + SwiftUpdateConstants.fileListXMLName))
                {
                    _Log.WriteEntry("We have both lists.");
                    XDocument oldList = XDocument.Load(_UpdateFolder + SwiftUpdateConstants.fileListXMLName); //Get the old local file list
                    XDocument newList = XDocument.Load(_TempFolder + SwiftUpdateConstants.newFileListXMLName);    //Get the new downloaded file list

                    //Get file entries from both files
                    List<FileEntry> oldFiles = (from file in oldList.Root.Descendants("File")
                                                select new FileEntry
                                                {
                                                    Path = file.Attribute("Filename").Value,
                                                    Checksum = file.Attribute("Checksum").Value,
                                                    FileSize = Convert.ToInt64(file.Attribute("Filesize").Value)
                                                }).ToList();

                    List<FileEntry> newFiles = (from file in newList.Root.Descendants("File")
                                                select new FileEntry
                                                {
                                                    Path = file.Attribute("Filename").Value,
                                                    Checksum = file.Attribute("Checksum").Value,
                                                    FileSize = Convert.ToInt64(file.Attribute("Filesize").Value)
                                                }).ToList();

                    bool doesFileExist = false;

                    foreach (var newFile in newFiles)   //For all new files
                    {
                        foreach (var oldFile in oldFiles)   //For all old files
                        {
                            if (oldFile.Path == newFile.Path)   //If we have a match with a path of a new and old file
                            {
                                if (oldFile.Checksum == newFile.Checksum)   //If the checksums match
                                {
                                    _Log.WriteEntry("The file " + oldFile.Path + " has not changed this update.");
                                    doesFileExist = true;   //This file hasn't changed this update
                                }
                            }
                        }

                        if (!doesFileExist) //If we have a new file not present in the old version
                        {
                            _Log.WriteEntry("The file " + newFile.Path + " is new, adding to list.");
                            _NewFiles.Add(newFile);    //Add this new file to the list
                        }

                        doesFileExist = false;
                    }

                    _Log.WriteEntry("Got new files. Count: " + _NewFiles.Count);
                }

                else    //We only have the new file list, so we assume every file is new (as we can't compare against the old one)
                {
                    _Log.WriteEntry("We only have new file list, assuming all are new. We checked the following folder: " + _UpdateFolder + SwiftUpdateConstants.fileListXMLName);
                    XDocument newList = XDocument.Load(_TempFolder + SwiftUpdateConstants.newFileListXMLName);

                    _NewFiles.AddRange((from file in newList.Root.Descendants("File")
                                        select new FileEntry
                                        {
                                            Path = file.Attribute("Filename").Value,
                                            Checksum = file.Attribute("Checksum").Value,
                                            FileSize = Convert.ToInt64(file.Attribute("Filesize").Value)
                                        }).ToList());
                }
            }

            catch (Exception ex)
            {
                _Log.WriteEntry("Exception when finding new files: " + ex);
                MessageBox.Show("Could not find new files. The update has been aborted. Please ensure you have a stable internet connection. Otherwise, try again later. If the issue persists, please contact support and provide your Update.log.", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private static long GetTotalSizeOfFiles(List<FileEntry> files)
        {
            long totalSize = 0;

            foreach (FileEntry file in files)
            {
                totalSize += file.FileSize; //Increment the size of file to totalSize
            }

            _Log.WriteEntry("Got a total size of " + totalSize + " bytes.");
            return totalSize;
        }

        private static async Task<bool> DownloadNewFiles()
        {
            try
            {
                int currentFile = 1;
                ProgressManager.SetTotalBytes(GetTotalSizeOfFiles(_NewFiles));  //Set the total size we have to download to the total size of the new files
                foreach (var newFile in _NewFiles)
                {
                    Directory.CreateDirectory(_TempFolder + _Application + "//" + Path.GetDirectoryName(newFile.Path));   //Create a directory for the new file
                    ProgressManager.progressDescription = "Downloading " + newFile.Path + ", file " + currentFile + " of " + _NewFiles.Count;   //Set the progress description
                    _Log.WriteEntry("Downloading the new file " + newFile.Path);
                    await DownloadFile(_ServerFolder + _Application + "/" + newFile.Path, _TempFolder + _Application + "//" + newFile.Path);  //Download the new file to the temp directory
                    currentFile++;  //File downloaded, increment

                    if (_ShouldAbort)   //If we aborted the download, return false
                    {
                        return false;
                    }
                }

                ProgressManager.progressDescription = "Verifying integrity of update, please wait...";
                return await Task.Run(() => VerifyUpdateIntegrity(_NewFiles));  //Return the value from verify update integrity
            }

            catch (Exception ex)
            {
                _Log.WriteEntry("Exception when downloading new files: " + ex);
                Directory.Delete(_TempFolder, true);
                MessageBox.Show("Failed to download update. Please ensure you have a stable internet connection. Otherwise, try again later. If the issue persists, please contact support and provide your Update.log.", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static async Task<bool> VerifyUpdateIntegrity(List<FileEntry> filesToVerify)
        {
            _Log.WriteEntry("Verifying integrity of files.");

            try
            {
                List<FileEntry> badFiles = new List<FileEntry>();
                bool isFileBad = true;

                foreach (var newFile in filesToVerify)  //For all of our new files
                {
                    foreach (var downloadedFile in Directory.GetFiles(_TempFolder, "*.*", SearchOption.AllDirectories))  //For all of the downloaded files in the temp folder
                    {
                        if (Path.GetFileName(newFile.Path) == Path.GetFileName(downloadedFile))   //If the two filenames match
                        {
                            if (newFile.Checksum == SwiftUpdateUtilities.CalculateMD5(downloadedFile))   //If the checksums match
                            {
                                isFileBad = false;  //The file integrity is guarenteed
                            }
                        }
                    }

                    if (isFileBad)  //If the file is bad, add it to the list of bad files to redownload
                    {
                        _Log.WriteEntry("The file " + newFile.Path + " is bad.");
                        badFiles.Add(newFile);
                    }

                    isFileBad = true;
                }

                if (badFiles.Count > 0) //If we have bad files, redownload them
                {
                    _Log.WriteEntry("We have bad files.");
                    bool shouldRetry = true;

                    while (shouldRetry && !_ShouldAbort)    //While we should retry and not abort
                    {
                        if (_BadFilesReattemptCount < SwiftUpdateConstants.RetryCount)
                        {
                            if (!await Task.Run(() => RedownloadBadFiles(badFiles)))    //If we did not successfully redownloaded the bad files/experienced an error, return false
                            {
                                return false;
                            }

                            else    //Otherwise the redownload was successful, so now we validate them
                            {
                                _BadFilesReattemptCount++;

                                if (!await Task.Run(() => VerifyUpdateIntegrity(badFiles)))    //If the integrity was bad again, we retry again
                                {
                                    _Log.WriteEntry("Failed to redownload bad files. Attempt " + (_BadFilesReattemptCount + 1) + " of " + SwiftUpdateConstants.RetryCount);
                                }

                                else    //Integrity was good this time
                                {
                                    _Log.WriteEntry("Successfully redownloaded bad files, returning true.");
                                    return true;
                                }
                            }
                        }

                        else    //We've exceeded the attempt count
                        {
                            shouldRetry = false;
                            Directory.Delete(_TempFolder, true);
                            MessageBox.Show("Failed to verify the integrity of the update. Please ensure you have a stable internet connection. Otherwise, try again later. If the issue persists, please contact support and provide your Update.log.", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    return false;
                }

                else    //No bad files, great, return true
                {
                    _Log.WriteEntry("No bad files, returning true.");
                    return true;
                }
            }

            catch (Exception ex)
            {
                _Log.WriteEntry("Exception when verifying update integrity: " + ex);
                Directory.Delete(_TempFolder);
                MessageBox.Show("Failed to verify the integrity of the update. Please ensure you have a stable internet connection. Otherwise, try again later. If the issue persists, please contact support and provide your Update.log.", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static async Task<bool> RedownloadBadFiles(List<FileEntry> badFiles)
        {
            try
            {
                int currentFile = 1;
                ProgressManager.SetTotalBytes(GetTotalSizeOfFiles(badFiles));   //Set the total bytes to download to the total size of the bad files
                foreach (var badFile in badFiles)
                {
                    Directory.CreateDirectory(_TempFolder + _Application + "//" + Path.GetDirectoryName(badFile.Path));   //Create the directory for the bad file
                    ProgressManager.progressDescription = "Re-downloading " + badFile.Path + ", file " + currentFile + " of " + badFiles.Count;
                    _Log.WriteEntry("Redownloading bad file " + badFile.Path);
                    await DownloadFile(_ServerFolder + _Application + "/" + badFile.Path, _TempFolder + _Application + "//" + badFile.Path);  //Download the new file to the temp directory
                    currentFile++;  //Increment the current file

                    if (_ShouldAbort)   //If we should abort, return false
                    {
                        return false;
                    }
                }

                ProgressManager.progressDescription = "Verifying integrity of update, please wait...";
                return true;
            }

            catch (Exception ex)
            {
                _Log.WriteEntry("Exception when redownloading bad files: " + ex);
                Directory.Delete(_TempFolder, true);
                MessageBox.Show("Failed to redowload bad files.Please ensure you have a stable internet connection. Otherwise, try again later. If the issue persists, please contact support and provide your Update.log.", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static async Task<bool> DownloadInstaller()
        {
            try
            {
                ProgressManager.SetTotalBytes(0);
                bool shouldRetry = true;
                int retryCount = 0;

                while (shouldRetry && !_ShouldAbort)    //While we should retry and not abort
                {
                    if (retryCount < SwiftUpdateConstants.RetryCount) //If we've not exceeded the retry count
                    {
                        ProgressManager.progressDescription = "Downloading latest version of installer for updating.";
                        _Log.WriteEntry("Downloading installer from server directory " + _ServerFolder + "Installer/" + _Application + " Installer.md5");

                        await DownloadFile(_ServerFolder + "Installer/" + _Application + " Installer.md5", _UpdateFolder + "Installer//Installer.md5"); //Downloading installer checksum
                        await DownloadFile(_ServerFolder + "Installer/" + _Application + " Installer.exe", _UpdateFolder + "Installer//Installer.exe"); //Downloading installer

                        if (SwiftUpdateUtilities.ReadMD5FromFile(_UpdateFolder + "Installer//Installer.md5") == SwiftUpdateUtilities.CalculateMD5(_UpdateFolder + "Installer//Installer.exe")) //Compare the downloaded md5 to the md5 of the downloaded installer
                        {
                            _Log.WriteEntry("Installer checksums matched.");
                            return true;
                        }

                        _Log.WriteEntry("Installer checksums did not match, retrying...");

                        retryCount++;
                    }

                    else    //We've exceeded the retry count, return false
                    {
                        Directory.Delete(_TempFolder, true);
                        shouldRetry = false;
                        _Log.WriteEntry("Retried max amount of times to download installer.");
                        MessageBox.Show("Failed to download installer. Please ensure you have a stable internet connection. Otherwise, try again later. If the issue persists, please contact support and provide your Update.log.", "Download Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                return false;
            }

            catch (Exception ex)
            {
                Directory.Delete(_TempFolder, true);
                _Log.WriteEntry("Exception when downloading the installer: " + ex);
                MessageBox.Show("Failed to download the installer. Please ensure you have a stable internet connection. Otherwise, try again later. If the issue persists, please contact support and provide your Update.log.", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static async Task DownloadFile(string file, string dest)
        {
            try
            {
                client = new WebClient();

                client.DownloadProgressChanged += (s, e) =>
                {
                    ProgressManager.UpdateTotalDownloadProgress(e.BytesReceived);
                };

                client.DownloadFileCompleted += (s, e) =>
                {
                    ProgressManager.NotifyNewFile();
                    ProgressManager.WriteDebugValues(_Log);
                    client.Dispose();
                    if (e.Cancelled)
                    {
                        ProgressManager.Reset();
                        ProgressManager.progressDescription = "Download aborted!";

                        _ShouldAbort = true;
                        _Log.WriteEntry("Aborted download.");
                        return;
                    }
                };

                await client.DownloadFileTaskAsync(new Uri(file), dest);
            }

            catch (Exception ex)
            {
                if (ex is System.Net.WebException)
                {
                    if (!_ShouldAbort)   //If this wasn't caused by aborting the download
                    {
                        MessageBox.Show("Failed to download file. Please ensure the server is online and that it is not being blocked by your antivirus/firewall. Otherwise, try again later. If the issue persists, please contact support and provide your Update.log.", "Update Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                        _ShouldAbort = true;
                    }

                    else
                    {
                        _Log.WriteEntry("Possibly aborted, exception is " + ex);
                    }
                }

                else
                {
                    MessageBox.Show("An exception occured when downloading a file from the server! The exact exception is: " + ex, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    _ShouldAbort = true;
                }
            }

        }

        private static class SwiftUpdateUtilities
        {
            public static string CalculateMD5(string filename)
            {
                try
                {
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(filename))
                        {
                            var hash = md5.ComputeHash(stream);
                            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        }
                    }
                }

                catch (Exception ex)
                {
                    _Log.WriteEntry("Caught an exception when calculating an MD5 checksum for " + filename + ". The exception is: " + ex);
                    return "";
                }
            }

            public static string ReadMD5FromFile(string file)
            {
                try
                {
                    return string.Join("", File.ReadAllLines(file));
                }

                catch (Exception ex)
                {
                    _Log.WriteEntry("Caught an exception when reading MD5 from " + file + ". The exception is: " + ex);
                    return "";
                }
            }
        }

        private static class SwiftUpdateXML
        {
            public static Version GetVersionNumber(string versionFile)
            {
                XDocument updFile = XDocument.Load(versionFile);

                return new Version((from ver in updFile.Descendants("Root")
                                    select ver.Element("Version").Value).First().ToString());
            }
        }

        private static class SwiftUpdateConstants
        {
            public static int RetryCount = 3;
            public static string versionMD5Name = "Version.md5";
            public static string newVersionMD5Name = "Version.new.md5";
            public static string versionXMLName = "Version.xml";
            public static string newVersionXMLName = "Version.new.xml";
            public static string fileListMD5Name = "FileList.md5";
            public static string newFileListMD5Name = "FileList.new.md5";
            public static string fileListXMLName = "FileList.xml";
            public static string newFileListXMLName = "FileList.new.xml";
        }

        private class FileEntry
        {
            public string Path { get; set; }
            public string Checksum { get; set; }
            public long FileSize { get; set; }
        }

    }
}

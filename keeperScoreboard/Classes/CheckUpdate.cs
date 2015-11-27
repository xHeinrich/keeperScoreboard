using System;
using System.Net;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Windows.Threading;
using UpdateClient;

namespace keeperScoreboard.Classes
{
    public class CheckUpdate
    {
        public void openUpdate(string newVer, string oldVer)
        {
            var dialog = new XAML.UpdateWindow(newVer, oldVer);
            if (dialog.ShowDialog() == true)
            {
                //start the update program
                //Process.Start(Directory.GetCurrentDirectory() + "/update.exe");
                //Exit the current program so the update program can overwrite the current files
                //Environment.Exit(0);
                //MainWindow.Close();
                UpdateClient.UpdateClient update = new UpdateClient.UpdateClient("scoreboard_update.zip", "http://nathan-dev.com/projects/update/", "keeperScoreboard.exe");
            }
        }
        public CheckUpdate()
        {
            //checkUpdaterVersion();
            using (WebClient wc = new WebClient())
            {
                try
                {
                    string update = wc.DownloadString("http://nathan-dev.com/projects/update/scoreboard_update.php?type=version");
                    if (Version.Parse(update) > Version.Parse(Classes.UsefulFunctions.checkVersion()))
                    {
                        Logger.addLog("Update Available -  Version:" + update + " Client Version:" + Classes.UsefulFunctions.checkVersion());
                        Application.Current.Dispatcher.BeginInvoke(
                             new Action(() =>
                             openUpdate(update, Classes.UsefulFunctions.checkVersion())
                             ),
                             DispatcherPriority.ApplicationIdle);

                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "The system cannot find the file specified")
                    {
                        MessageBox.Show("Could not find update.exe, make sure you are running this program as Administrator");
                        //Environment.Exit(0);
                    }
                    else
                    {
                        MessageBox.Show("Unable to connect to update server.");
                        //Environment.Exit(0);
                    }
                }

            }
            cleanUpdate();
        }
        public void cleanUpdate()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
            File.Delete(Directory.GetCurrentDirectory() + "/scoreboard_update.zip");
            foreach (string file in files)
            {
                if (file.Contains(".old"))
                {
                    File.Delete(file);
                }
            }
            using (WebClient wc = new WebClient())
            {
                wc.DownloadString("http://nathan-dev.com/projects/update/scoreboard_update.php?type=update");
            }
        }
        public void checkUpdaterVersion()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string currentDir = Directory.GetCurrentDirectory();
                    string version = wc.DownloadString("http://nathan-dev.com/projects/update/scoreboard_update.php?type=updateversion");
                    if(!File.Exists(currentDir + "\\update.exe"))
                    {
                        try
                        {
                            wc.DownloadFile("http://nathan-dev.com/projects/update/update.exe", currentDir + "\\update1.exe");
                            File.Delete(currentDir + "\\update.exe");
                            File.Move(currentDir + "\\update1.exe", currentDir + "\\update.exe");
                            File.Delete(currentDir + "\\Update1.exe");
                            //Update code
                        }
                        catch (Exception ex)
                        {
                            File.Delete(currentDir + "\\Update1.exe");
                            MessageBox.Show("An error occured whilst updating:" + ex.ToString());
                        }
                    }
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(currentDir + "\\update.exe");
                    string fileVersion = myFileVersionInfo.FileVersion;
                    //check if current version is lower than website version
                    if (Version.Parse(version) > Version.Parse(fileVersion))
                    {
                        try
                        {
                            wc.DownloadFile("http://nathan-dev.com/projects/update/update.exe", currentDir + "\\update1.exe");
                            File.Delete(currentDir + "\\update.exe");
                            File.Move(currentDir + "\\update1.exe", currentDir + "\\update.exe");
                            File.Delete(currentDir + "\\Update1.exe");
                            //Update code
                        }
                        catch (Exception ex)
                        {
                            File.Delete(currentDir + "\\Update1.exe");

                             Application.Current.Dispatcher.BeginInvoke(
                             new Action(() => MessageBox.Show("An error occured whilst updating:" + ex.ToString())),
                             DispatcherPriority.ApplicationIdle
                         ); 

                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }

    }
}

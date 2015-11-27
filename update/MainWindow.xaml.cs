using System;
using System.Windows;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel;
namespace update
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            download();
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
        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            pbDownload.Value = e.ProgressPercentage;
            lblDownload.Content = String.Format("{0} of {1} Mb. {2} %",

                ConvertBytesToMegabytes(e.BytesReceived),
                ConvertBytesToMegabytes(e.TotalBytesToReceive),
            e.ProgressPercentage
               );
        }
        static double ConvertBytesToMegabytes(long bytes)
        {
            return Math.Round((bytes / 1024f) / 1024f, 2);
        }
        public void download()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Uri uri = new Uri("http://nathan-dev.com/projects/update/scoreboard_update.zip");
                    //Run when the file is downloaded
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback2);
                    // Specify a progress notification handler.
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                    wc.DownloadFileAsync(uri, Directory.GetCurrentDirectory() + "/scoreboard_update.zip");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Code 2: " + ex.ToString());
                }
            }
        }
        private void DownloadFileCallback2(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/update/");
                try
                {
                    ZipFile.ExtractToDirectory(Directory.GetCurrentDirectory() + "/scoreboard_update.zip", Directory.GetCurrentDirectory() + "/update/");
                    string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/update/");
                    string filename1 = "";
                    try
                    {
                        string currentDir = Directory.GetCurrentDirectory();
                        foreach (string file in files)
                        {
                            filename1 = file;
                            if (file == "update.exe")
                            {
                                return;
                            }
                            string[] filename = file.Split('/');
                            File.Delete(Directory.GetCurrentDirectory() + "/" + filename[filename.Length - 1]);
                            File.Move(file, currentDir + "/" + filename[filename.Length - 1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(filename1 + "Code 1: " + ex.ToString());
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    cleanUpdate();
                    ZipFile.ExtractToDirectory(Directory.GetCurrentDirectory() + "/scoreboard_update.zip", Directory.GetCurrentDirectory() + "/update/");
                }
                cleanUpdate();
                MessageBox.Show("Successfully updated to latest version of BF4Scoreboard");
                Process.Start(Directory.GetCurrentDirectory() + "/keeperScoreboard.exe");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                cleanUpdate();
            }

        }
    }
}

using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel;

namespace UpdateClient
{
    public class UpdateClient
    {
        public String ClientVersion = "";
        public String ServerVersion = "";
        /// <summary>
        /// The Zip file you wish to download and save
        /// </summary>
        public String ServerZipFile = "";
        /// <summary>
        /// The patch server url you wish to use
        /// </summary>
        public String ServerUrl = "";
        /// <summary>
        /// The name of the executable file you wish to update
        /// </summary>
        public String ExeName = "";
        /// <summary>
        /// Gets the current working directory of the Application
        /// </summary>
        public String CurrentDir;
        /// <summary>
        /// File extension of the old version of the .exe
        /// </summary>
        public String ExtensionName = ".old";
        /// <summary>
        /// How you want to update e.g 1 = rename the current execuatble to .old and delete on startup
        /// </summary>
        public Byte PatchConfiguration = 1;
        /// <summary>
        /// Status window
        /// </summary>
        public UpdateStatus StatusWindow = new UpdateStatus();
        public UpdateClient(string serverZipFile, string serverUrl, string exeName)
        {
            ServerZipFile = serverZipFile;
            ServerUrl = serverUrl;
            ExeName = exeName;
            CurrentDir = Directory.GetCurrentDirectory();
            DownloadUpdate();
        }
        public void DownloadUpdate()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Uri uri = new Uri(ServerUrl + "/" + ServerZipFile);
                    StatusWindow.Show();
                    //Run when the file is downloaded
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                    // Specify a progress notification handler.
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                    wc.DownloadFileAsync(uri, Directory.GetCurrentDirectory() + "\\" + ServerZipFile);
                }
                catch (Exception)
                {
                }
            }
        }
        public void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try {
                StatusWindow.DownloadProgressText.Text = "Moving old exe";
                File.Move(CurrentDir + "\\" + ExeName, CurrentDir + "\\" + ExeName + ExtensionName);
                StatusWindow.DownloadProgressText.Text = "Extracting new exe to directory";
                ZipFile.ExtractToDirectory(CurrentDir + "\\" + ServerZipFile, CurrentDir);
                StatusWindow.DownloadProgressText.Text = "Deleting zip file";
                File.Delete(CurrentDir + "\\" + ServerZipFile);
                StatusWindow.DownloadProgressText.Text = "Starting new exe";
                Process.Start(Directory.GetCurrentDirectory() + "\\" + ExeName);
                Environment.Exit(1);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
                }
        }
        public void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            StatusWindow.DownloadProgress.Value = e.ProgressPercentage;
            StatusWindow.DownloadProgressText.Text = String.Format("{0} of {1} Mb. {2} %",
                ConvertBytesToMegabytes(e.BytesReceived),
                ConvertBytesToMegabytes(e.TotalBytesToReceive),
            e.ProgressPercentage
               );
        }
        static double ConvertBytesToMegabytes(long bytes)
        {
            return Math.Round((bytes / 1024f) / 1024f, 2);
        }
    }
}

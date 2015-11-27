using System;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using System.Timers;

namespace keeperScoreboard.XAML
{
    /// <summary>
    /// Interaction logic for Scoreboard.xaml
    /// </summary>
    public partial class Scoreboard : Window
    {
        string guid = "";
        public System.Timers.Timer _timer;
        public Scoreboard(string guidFromMain = "", Classes.CustomSnapshotRoot root = null)
        {
            guid = guidFromMain;
            Classes.Logger.addLog("Opened scoreboard on server -> " + guid);
            InitializeComponent();
            if(root != null)
            {
                Classes.ScoreboardRenderer render = new Classes.ScoreboardRenderer(this, root.snapshot.mapId, root);
                Classes.Logger.addLog("Enabled initial render: ");
            }
            else
            {
                _timer = new System.Timers.Timer(3000);
                _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                _timer.Enabled = true; // Enable it
                _timer_Elapsed(this, null);
            }
            if(MainWindow.keeperSettings.ScoreboardBackgrounds == false)
            {
                BackgroundAnimation.Stop();
                BackgroundAnimation.Visibility = Visibility.Hidden;
            }
            else
            {
                BackgroundAnimation.Play();
            }
        }

        public void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Classes.CustomSnapshotRoot root = null;
            Classes.KeeperAPI keeper = new Classes.KeeperAPI();
            Action workAction = delegate
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += delegate
                {
                    root = keeper.getKeeperInfo(guid);
                };
                worker.RunWorkerCompleted += delegate
                {
                    try {
                        //image.Source = null;
                        Classes.ScoreboardRenderer render = new Classes.ScoreboardRenderer(this, root.snapshot.mapId, root);
                    }catch(Exception ex)
                    {
                        Classes.Logger.addLog(ex.ToString(), 1);
                    }
                    //Update code in here
                };
                worker.RunWorkerAsync();
            };
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, workAction);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if(_timer != null && _timer.Enabled == true)
            { 
                _timer.Enabled = false;
            }
        }
    }
}

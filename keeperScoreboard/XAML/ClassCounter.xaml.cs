using System;
using System.Windows;
using System.Timers;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media;
namespace keeperScoreboard.XAML
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public class classCounters
    {
        public int t1Assault { get; set; }
        public int t1Engineer { get; set; }
        public int t1Support { get; set; }
        public int t1Recon { get; set; }
        public int t2Assault { get; set; }
        public int t2Engineer { get; set; }
        public int t2Support { get; set; }
        public int t2Recon { get; set; }
    }
    public partial class ClassCounter : Window
    {
        string guid = "";
        public System.Timers.Timer _timer;
        public classCounters classCounts = new classCounters();
        public ClassCounter(string guidFromMain = "", Classes.CustomSnapshotRoot root = null)
        {
            guid = guidFromMain;
            Classes.Logger.addLog("Opened scoreboard on server -> " + guid);
            InitializeComponent();
            if (root != null)
            {
                Classes.Logger.addLog("Started Class Counter: " + guid);
            }
            else
            {
                _timer = new System.Timers.Timer(60000);
                _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                _timer.Enabled = true; // Enable it
                _timer_Elapsed(this, null);
            }
        }
        public async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Classes.CustomSnapshotRoot root = null;
            Classes.KeeperAPI keeper = new Classes.KeeperAPI();
            //classCounts = null;
            Action workAction = delegate
            {
                BackgroundWorker worker = new  BackgroundWorker();
                worker.DoWork += delegate
                {
                    root = keeper.getKeeperInfo(guid);
                };
                worker.RunWorkerCompleted += async delegate
                {
                    try
                    {
                        foreach (var player in root.snapshot.teamInfo.team1.player)
                        {
                            
                            Classes.Structs.PlayerLoadout playerInfo = await Classes.GetPlayersKit.GetWeaponInfo(player.playerId, player.name);
                            switch (playerInfo.selectedKit.ToString())
                            {
                                case "0":
                                    classCounts.t1Assault += 1;
                                    break;
                                case "1":
                                    classCounts.t1Engineer += 1;
                                    break;
                                case "2":
                                    classCounts.t1Support += 1;
                                    break;
                                case "3":
                                    classCounts.t1Recon += 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        //Assault
                        pbAssault1.Value = classCounts.t1Assault;
                        lblt1Assault.Content = classCounts.t1Assault.ToString();
                        //Engineer
                        pbEngineer1.Value = classCounts.t1Engineer;
                        lblt1Engineer.Content = classCounts.t1Engineer.ToString();
                        //Support
                        pbSupport1.Value = classCounts.t1Support;
                        lblt1Support.Content = classCounts.t1Support.ToString();
                        //Recon
                        pbRecon1.Value = classCounts.t1Recon;
                        lblt1Recon.Content = classCounts.t1Recon.ToString();

                        foreach (var player in root.snapshot.teamInfo.team2.player)
                        {
                            Classes.Structs.PlayerLoadout playerInfo = await Classes.GetPlayersKit.GetWeaponInfo(player.playerId, player.name);
                            switch (playerInfo.selectedKit.ToString())
                            {
                                case "0":
                                    classCounts.t2Assault += 1;
                                    break;
                                case "1":
                                    classCounts.t2Engineer += 1;
                                    break;
                                case "2":
                                    classCounts.t2Support += 1;
                                    break;
                                case "3":
                                    classCounts.t2Recon += 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        //Assault
                        pbAssault2.Value = classCounts.t2Assault;
                        lblt2Assault.Content = classCounts.t2Assault.ToString();
                        //Support
                        pbEngineer2.Value = classCounts.t2Engineer;
                        lblt2Engineer.Content = classCounts.t2Engineer.ToString();
                        //Engineer
                        pbSupport2.Value = classCounts.t2Support;
                        lblt2Support.Content = classCounts.t2Support.ToString();
                        //Recon
                        pbRecon2.Value = classCounts.t2Recon;
                        lblt2Recon.Content = classCounts.t2Recon.ToString();
                        classCounts = new classCounters();
                    }
                    catch (Exception ex)
                    {
                        Classes.Logger.addLog(ex.ToString(), 1);
                    }
                    //Update code in here
                };
                worker.RunWorkerAsync();
            };
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, workAction);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_timer != null && _timer.Enabled == true)
            {
                _timer.Enabled = false;
            }
        }
    }
}

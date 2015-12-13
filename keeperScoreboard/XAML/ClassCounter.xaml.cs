using System;
using System.Windows;
using System.Timers;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

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
        public int defib { get; set; }
    }
    public partial class ClassCounter : Window
    {
        public Image MyIcon
        {
            get
            {
                return new Bitmap(@"Resources/usa.png");
            }
        }
        string guid = "";
        public System.Timers.Timer _timer;
        classCounters classCounts = new classCounters();
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

            classCounts = null;
            classCounts = new classCounters();
            Classes.CustomSnapshotRoot root = null;
            Classes.KeeperAPI keeper = new Classes.KeeperAPI();
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
                        if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team1.faction) == "US")
                            imgFaction1.Source = loadBitmap(Properties.Resources.usa);
                        else if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team1.faction) == "CN")
                            imgFaction1.Source = loadBitmap(Properties.Resources.china);
                        else if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team1.faction) == "RU")
                            imgFaction1.Source = loadBitmap(Properties.Resources.ru);
                        if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team2.faction) == "US")
                            imgFaction2.Source = loadBitmap(Properties.Resources.usa);
                        else if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team2.faction) == "CN")
                            imgFaction2.Source = loadBitmap(Properties.Resources.china);
                        else if (Classes.JSONHelper.whatFaction(root.snapshot.teamInfo.team2.faction) == "RU")
                            imgFaction2.Source = loadBitmap(Properties.Resources.ru);
                            
                        foreach (var player in root.snapshot.teamInfo.team1.player)
                        {
                            
                            Classes.Structs.PlayerLoadout playerInfo = await Classes.GetPlayersKit.GetWeaponInfo(player.playerId, player.name);
                            switch (playerInfo.selectedKit.ToString())
                            {
                                case "0":
                                    classCounts.t1Assault += 1;

                                    if (playerInfo.kitList[0].kitIdInformation[3] != 2887915611 && playerInfo.kitList[0].kitIdInformation[4] != 2887915611)
                                        classCounts.defib += 1;
                                    break; //2887915611
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
                                    if (playerInfo.kitList[0].kitIdInformation[3] != 2887915611 && playerInfo.kitList[0].kitIdInformation[4] != 2887915611)
                                        classCounts.defib += 1;
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
                        lblAssaultPlayerWithDefibs.Content = classCounts.defib.ToString();
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
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
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

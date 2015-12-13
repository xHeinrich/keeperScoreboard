using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using System.Timers;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Controls.Primitives;

namespace keeperScoreboard.XAML
{
    /// <summary>
    /// Interaction logic for ScoreboardBasic.xaml
    /// </summary>
    public partial class ScoreboardBasic : Window
    {
        List<Classes.CustomSnapshotRoot> extendedStats = new List<Classes.CustomSnapshotRoot>();
        string guid = "";
        public System.Timers.Timer _timer;


        private void ShowCustomBalloon(string header, string content, XAML.Controls.ToasterNotification.ErrorType type = 0)
        {
            XAML.Controls.ToasterNotification balloon = new XAML.Controls.ToasterNotification(header, content, type);

            //show balloon and close it after 4 seconds
            MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Fade, 6000);
        }
        public ScoreboardBasic(string guidFromUpper)
        {
            guid = guidFromUpper;
            InitializeComponent();
            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true; // Enable it
            _timer_Elapsed(this, null);
            txtDescription.IsEnabled = false;
            Classes.Logger.addLog("Started logging server -> " + guid);
            ShowCustomBalloon("Stats Logging", "Started logging on server: \n " + guid);

        }

        public async void getData()
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
                    if (root == null)
                    {
                        if (extendedStats.Count == 0)
                        {
                            lblSaved.Content = "Something went wrong with the server";
                            //Potentially exit the form here!
                            _timer.Enabled = false;
                            MessageBox.Show("The server crashed or was never available. \n Please close this window.");
                            this.Close();
                            return;
                        }
                        else
                        {
                            forceSave();
                            extendedStats.Clear();
                            return;
                        }

                    }
                    if (!root.snapshot.status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                        return;
                    try
                    {
                        int t1count = root.snapshot.teamInfo.team1.player.Count;
                        int t2count = root.snapshot.teamInfo.team2.player.Count;
                        int totalPlayers = t1count + t2count;
                        lblTime.Content = "Round Time: " + Classes.UsefulFunctions.getTime(root.snapshot.roundTime, 1);
                        lblCountPlayers.Content = "Getting Statistics for " + totalPlayers.ToString() + " players";
                        lblMap.Content = "Map: " + Classes.JSONHelper.whatMap(root.snapshot.mapId);
                        lblMode.Content = "Mode: " + Classes.JSONHelper.whatMode(root.snapshot.modeId);
                        foreach( var player in root.snapshot.teamInfo.team1.player )
                        {
                            Classes.Structs.PlayerLoadout playerInfo = await Classes.GetPlayersKit.GetWeaponInfo(player.playerId, player.name);
                        
                            player.kit = playerInfo.selectedKit;
                            player.primaryWeapon = playerInfo.kitList[Convert.ToInt32(playerInfo.selectedKit)].kitIdInformation[0];
                            player.secondaryWeapon = playerInfo.kitList[Convert.ToInt32(playerInfo.selectedKit)].kitIdInformation[1];
                        }
                        this.Title = "Logging: " + totalPlayers.ToString() + "P | " + Classes.UsefulFunctions.getTime(root.snapshot.roundTime, 1) + " | " + Classes.JSONHelper.whatMap(root.snapshot.mapId);
                        if (root.snapshot.roundTime != 0)
                        {
                            extendedStats.Add(root);
                        }
                        bool descrEnabled = false;
                        if ((bool)cbAddDescrAuto.IsChecked)
                        {
                            descrEnabled = true;
                        }
                        Classes.DataExport export = new Classes.DataExport(extendedStats, root);
                        string fileName;
                        if (descrEnabled == true)
                        {
                            fileName = export.saveOrNot(txtDescription.Text);
                            if (fileName != "")
                            {
                                lblSaved.Content = "Last Saved at: " + DateTime.Now.ToString();
                                if (cbOpenOnFinish.IsChecked == true)
                                {
                                    XAML.AddBombs addBombs = new XAML.AddBombs(Classes.UsefulFunctions.getFullReport(fileName), fileName);
                                    addBombs.Show();
                                }
                                fileName = null;
                                extendedStats.Clear();
                            }
                        }
                        else
                        {
                            fileName = export.saveOrNot(txtDescription.Text);
                            if (fileName != "")
                            {
                                lblSaved.Content = "Last Saved at: " + DateTime.Now.ToString();
                                if (cbOpenOnFinish.IsChecked == true)
                                {
                                    XAML.AddBombs addBombs = new XAML.AddBombs(Classes.UsefulFunctions.getFullReport(fileName), fileName);
                                    addBombs.Show();

                                }
                                fileName = null;

                                extendedStats.Clear();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Classes.Logger.addLog("Guid: " + guid + "|" + root.ToString() + ":" + ex.ToString(), 1);
                    }

                };
                worker.RunWorkerAsync();
            };
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, workAction);
        }
        public void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                getData();
            }
            catch (Exception ex)
            {
                Classes.Logger.addLog("Guid: " + guid + "|" + ex.ToString());
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _timer.Enabled = false;
            if(MainWindow.keeperSettings.SaveOnClose == true)
            {
                forceSave();
            }
            MyNotifyIcon.Dispose();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            txtDescription.IsEnabled = true;
        }

        private void cbAddDescrAuto_Unchecked(object sender, RoutedEventArgs e)
        {
            txtDescription.IsEnabled = false;
        }

        private void btnOpenScoreboard_Click(object sender, RoutedEventArgs e)
        {
            XAML.Scoreboard scoreboard = new XAML.Scoreboard(guid);
            scoreboard.Show();
        }

        private void cbOpenOnFinish_Checked(object sender, RoutedEventArgs e)
        {

        }
        public bool forceSave()
        {
            try
            {
                long saveTicks = DateTime.Now.Ticks;
                string fileName = Directory.GetCurrentDirectory() + "\\reports\\scoreboard_" + saveTicks.ToString() + ".scbrd";
                string json = JsonConvert.SerializeObject(extendedStats.ToArray());
                System.IO.File.WriteAllText(fileName, json);
                lblSaved.Content = "Last Saved at: " + DateTime.Now.ToString();
                return true;
            }
            catch (Exception ex)
            {
                Classes.Logger.addLog("Guid: " + guid + "|" + ex.ToString(), 1);
                return false;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using CrashReporterDotNET;
using System.ComponentModel;
using Hardcodet.Wpf.TaskbarNotification;
using System.Diagnostics;
using System.Windows.Navigation;

namespace keeperScoreboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public System.Timers.Timer _timer;
        //server guid
        public string guid;
        //List of the saved reports
        string[] reports;
        //List of descriptions for each report
        public static List<Classes.Description> reportDescriptions = new List<Classes.Description>();
        //Window title name
        public string buildString { get; set; }
        public static List<Classes.Structs.TeamData> CompetitiveTeamData = new List<Classes.Structs.TeamData>();
        public static Classes.Structs.SettingsStruct keeperSettings = Classes.Settings.Load();

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (keeperSettings.MinimizeToTray == true)
            {
                e.Cancel = true;

                //show balloon with custom icon
                ShowCustomBalloon("Attention", "Minimised to system tray", XAML.Controls.ToasterNotification.ErrorType.Normal);
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                TaskbarIcon.Dispose();
                e.Cancel = false;
            }
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        private void ShowCustomBalloon(string header, string content, XAML.Controls.ToasterNotification.ErrorType type = 0)
        {
            XAML.Controls.ToasterNotification balloon = new XAML.Controls.ToasterNotification(header, content, type);
            //show balloon and close it after 6 seconds
            TaskbarIcon.ShowCustomBalloon(balloon, PopupAnimation.Fade, 6000);
        }
        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += ApplicationThreadException;

            Classes.ReportCompatabilityCleanup clean = new Classes.ReportCompatabilityCleanup();

            InitializeComponent();
            buildString = String.Format("Scoreboard : Build v" + Classes.UsefulFunctions.checkVersion().ToString() + " " + Classes.UsefulFunctions.RetrieveLinkerTimestamp().ToString());
            //Check if the program is already open, if so exit imediately 
            if (checkIfAlreadyExists() == true)
            {
                TaskbarIcon.Dispose();
                MessageBox.Show("KeeperScoreboard Already Running");
                MessageBox.Show("KeeperScoreboard Already Running");
                Environment.Exit(0);
            }
            reloadReports();
            reloadServers();
            loadSettings();
            this.Title = buildString;
            Classes.CheckUpdate update = new Classes.CheckUpdate();
            // int i = 0 / crash();
            TaskbarIcon.ToolTip = buildString;
        }
        public bool checkIfAlreadyExists()
        {
            Process[] pname = Process.GetProcessesByName("keeperScoreboard");
            if (pname.Length <= 1 )
                return false;
            else
                return true;
        }
        public void saveSettings()
        {
            //Scoreboard Background Animation
            if (cbScoreboardBackgrounds.IsChecked.HasValue && cbScoreboardBackgrounds.IsChecked.Value == true)
                keeperSettings.ScoreboardBackgrounds = true;
            else
                keeperSettings.ScoreboardBackgrounds = false;
            //Save report on logger close
            if (cbSaveOnClose.IsChecked.HasValue && cbSaveOnClose.IsChecked.Value == true)
                keeperSettings.SaveOnClose = true;
            else
                keeperSettings.SaveOnClose = false;
            //Minimize to system tray on close
            if (cbMinimizeToTray.IsChecked.HasValue && cbMinimizeToTray.IsChecked.Value == true)
                keeperSettings.MinimizeToTray = true;
            else
                keeperSettings.MinimizeToTray = false;
            Classes.Settings.Save();
        }
        public void loadSettings()
        {
            if (keeperSettings == null)
            {
                Classes.Settings.Save();
                Classes.Settings.Load();
                loadSettings();
            }
            else
            {
                cbMinimizeToTray.IsChecked = keeperSettings.MinimizeToTray;
                cbSaveOnClose.IsChecked = keeperSettings.SaveOnClose;
                cbScoreboardBackgrounds.IsChecked = keeperSettings.ScoreboardBackgrounds;
            }
        }
        public int crash()
        {
            return 0;
        }
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            ReportCrash((Exception)unhandledExceptionEventArgs.ExceptionObject);
            Environment.Exit(0);
        }

        private static void ApplicationThreadException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ReportCrash(e.Exception);
            Environment.Exit(0);
        }

        private static void ReportCrash(Exception exception)
        {
            var reportCrash = new ReportCrash
            {
                ToEmail = "nathan_heinrich@outlook.com"
            };

            reportCrash.Send(exception);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            guid = "";
            try
            {

                string[] tokenString = txtBattlelogURL.Text.Split('/');

                foreach (string token in tokenString)
                {
                    Guid guidNull;
                    if (Guid.TryParse(token, out guidNull))
                    {
                        guid = guidNull.ToString();
                        break;
                    }
                }
                XAML.Scoreboard scoreboard = new XAML.Scoreboard(guid);
                scoreboard.Show();
                Classes.Logger.addLog("btnSubmit_Click() - Opened scoreboard");
            }
            catch (Exception ex)
            {
                MessageBox.Show("please enter a valid battlelog url: " + ex.ToString());
            }

        }

        private void btnLogStats_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = txtBattlelogURL.Text.ToLower();
                string splitGUID = Regex.Split(s, "pc")[1];
                guid = Regex.Split(splitGUID, "/")[1];
                XAML.ScoreboardBasic scoreboard = new XAML.ScoreboardBasic(guid);
                scoreboard.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("please enter a valid battlelog url: " + ex.ToString());
            }
        }
        public Classes.CustomSnapshotRoot getSnapshot(string filename)
        {
            Classes.CustomSnapshotRoot returnRoot = null;
            try
            {
                string s = File.ReadAllText(filename);
                List<Classes.CustomSnapshotRoot> rootList = JsonConvert.DeserializeObject<List<Classes.CustomSnapshotRoot>>(s);

                returnRoot = rootList[rootList.Count - 1];
                int i = 0;
                foreach (var roots in rootList)
                {

                    if (roots.snapshot.roundTime == returnRoot.snapshot.roundTime)
                    {
                        return rootList[i];
                    }
                    i++;

                }
            }

            catch (Exception ex)
            {
                Classes.Logger.addLog(ex.ToString(), 1);
            }

            return returnRoot;
        }
        public void reloadReports()
        {
            Classes.DescriptionLoader.readDescription();
            Classes.Logger.addLog("reportLoader() - Opened report loader");
            try
            {
                reports = Directory.GetFiles(Directory.GetCurrentDirectory() + "//reports");
                lbReports.Items.Clear();
                loadGridView();
                LoadReports();
            }
            catch (Exception)
            {
                if(!Directory.Exists(Directory.GetCurrentDirectory() + "//reports"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "//reports");
                }
                //MessageBox.Show("You must have saved a report before viewing one");
            }
        }
        public void loadGridView()
        {
            Classes.Logger.addLog("loadGridView() - gridview loaded");
            var gridView = new GridView();
            this.lbReports.View = gridView;
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Date",
                DisplayMemberBinding = new Binding("date")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Map",
                DisplayMemberBinding = new Binding("map")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Round Time",
                DisplayMemberBinding = new Binding("roundTime")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Description",
                DisplayMemberBinding = new Binding("description")
            });
        }
  
        public string[] LoadReportMap(string filename)
        {
            try
            {
                string s = File.ReadAllText(filename);
                List<Classes.CustomSnapshotRoot> rootList = JsonConvert.DeserializeObject<List<Classes.CustomSnapshotRoot>>(s);
                string[] arrayReturn = { Classes.JSONHelper.whatMap(rootList[0].snapshot.mapId), Classes.UsefulFunctions.getTime(rootList[rootList.Count - 1].snapshot.roundTime, 1) };
                return arrayReturn;
            }
            catch (Exception)
            {
                string[] arrayReturn = { "Delete Me", "Delete Me" };
                return arrayReturn;
            }
        }
        public void LoadReports()
        {
            Classes.Logger.addLog("LoadReports() - reports loaded");
            foreach (var report in reports)
            {
                string s = report;
                string[] reportName = s.Split('/');
                string[] reportMap = reportName[reportName.Length - 1].Split('_');
                string[] reportTime = reportMap[1].Split('.');
                //string map = Classes.JSONHelper.whatMap(reportMap[1] + "_" + reportMap[2]);
                string description = "";
                string[] reportItems = LoadReportMap(report);
                foreach (var item in MainWindow.reportDescriptions)
                {
                    if (item.time == long.Parse(reportTime[0]))
                    {
                        description = item.descr;
                    }
                }
                lbReports.Items.Add(new MyItem { map = reportItems[0], date = ConvertTicksToDateTime(long.Parse(reportTime[0])), roundTime = reportItems[1], description = description });
            }
            //lbReports.Items.Add("Test");
        }
        public class MyItem
        {
            public string map { get; set; }
            public DateTime date { get; set; }
            public string roundTime { get; set; }
            public string description { get; set; }
        }
        //
        //Load server gridview
        //
        public void reloadServers()
        {
            loadGridViewServers();
            LoadServers();
        }
        public void LoadServers()
        {
            List<Classes.Structs.ServerInfo> serverList = Classes.BattlelogCrawler.BattlelogCrawlerBegin();
            if(serverList.Count == 0)
            {
                lbCurrentServers.Items.Add(new MyItemServers {  svname = "No squad obliteration servers active" });
            }
            foreach (var server in serverList)
            {
                lbCurrentServers.Items.Add(new MyItemServers { svcountry = server.location, svname = server.serverName, svplayers = server.playerSlots.ToString(), svguid = server.serverGuid.ToString() });
            }
        }
        public class MyItemServers
        {
            public string svname { get; set; }
            public string svplayers { get; set; }
            public string svcountry { get; set; }
            public string svguid { get; set; }

        }
        public void loadGridViewServers()
        {
            Classes.Logger.addLog("loadGridView() - gridview loaded");
            var gridView = new GridView();
            this.lbCurrentServers.View = gridView;
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Server Name",
                DisplayMemberBinding = new Binding("svname")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Players",
                DisplayMemberBinding = new Binding("svplayers")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Country",
                DisplayMemberBinding = new Binding("svcountry")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Server Guid",
                DisplayMemberBinding = new Binding("svguid")
            });
        }


        public static DateTime ConvertTicksToDateTime(long lticks)
        {
            DateTime dtresult = new DateTime(lticks);
            return dtresult;
        }

        private void lbReports_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnLoadReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XAML.Scoreboard scoreboard = new XAML.Scoreboard("", getSnapshot(reports[lbReports.SelectedIndex]));
                scoreboard.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must first select a report: " + ex.ToString());
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => reloadReports()), System.Windows.Threading.DispatcherPriority.DataBind);
        }

        private void EditDecsription_OnClick(object sender, RoutedEventArgs e)
        {

            Classes.Logger.addLog("EditDecsription_OnClick() - description edit window opened");
            bool editOrAdd = false;//add = 0, edit = 1
            string s = reports[lbReports.SelectedIndex];
            string[] reportName = s.Split('/');
            string[] reportMap = reportName[reportName.Length - 1].Split('_');
            string[] reportTime = reportMap[1].Split('.');
            string description = "";
            foreach (var text in MainWindow.reportDescriptions)
            {
                if (text.time == long.Parse(reportTime[0]))
                {
                    description = text.descr;
                    editOrAdd = true;
                }
            }
            var dialog = new XAML.DiagBox(description);
            string descriptionText = "";
            if (dialog.ShowDialog() == true)
            {
                descriptionText = dialog.ResponseText;
            }
            if (editOrAdd)
            {
                Classes.DescriptionLoader.editDescription(long.Parse(reportTime[0]), descriptionText);
            }
            else
            {
                Classes.DescriptionLoader.addDescription(long.Parse(reportTime[0]), descriptionText);
            }
            lbReports.Items.Clear();
            LoadReports();
        }

        private void DeleteReport_OnClick(object sender, RoutedEventArgs e)
        {
            if (lbReports.SelectedItems.Count > 1)
            {
                if (MessageBox.Show("Pressing YES will permanently delete MULTIPLE reports!", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Classes.Logger.addLog("DeleteReport_OnClick() - Multiple reports deleted");
                    foreach (MyItem item in lbReports.SelectedItems)
                    {
                        string s = reports[lbReports.Items.IndexOf(item)];
                        Classes.DescriptionLoader.deleteReport(s);

                    }
                    lbReports.Items.Clear();
                    reports = Directory.GetFiles(Directory.GetCurrentDirectory() + "//reports");
                    LoadReports();
                }
            }
            else
            {

                if (MessageBox.Show("Pressing YES will permanently delete the report!", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Classes.Logger.addLog("DeleteReport_OnClick() - Single report deleted");
                    string s = reports[lbReports.SelectedIndex];
                    Classes.DescriptionLoader.deleteReport(s);
                    lbReports.Items.Clear();
                    reports = Directory.GetFiles(Directory.GetCurrentDirectory() + "//reports");
                    LoadReports();
                }
            }
        }

        private void btnMassPlayerData_Click(object sender, RoutedEventArgs e)
        {
            try {
                XAML.MassPlayerData dataShow = new XAML.MassPlayerData(CreateMassData(), lbReports.SelectedItems.Count);
                dataShow.Show();
            }catch(Exception)
            {

            }
        }
        public List<Classes.playersData> CreateMassData()
        {
            List<Classes.playersData> massPlayerData = new List<Classes.playersData>();
            Classes.Logger.addLog("btnMassPlayerData_Click() - mass report created");
            massPlayerData.Clear();
            //if (lbReports.SelectedItems.Count < 2)
            if(2 == 1)
            {
               // MessageBox.Show("Select more than one Battlereport");
            }
            else
            {
                foreach (MyItem item in lbReports.SelectedItems)
                {

                    Classes.CustomSnapshotRoot m_report = getSnapshot(reports[lbReports.Items.IndexOf(item)]);

                    foreach (var player in m_report.snapshot.teamInfo.team1.player)
                    {
                        bool addOrNot = true;
                        foreach (var name in massPlayerData)
                        {
                            if (player.name == name.name)
                            {
                                addOrNot = false;
                            }
                        }
                        if (addOrNot == true)
                        {
                            massPlayerData.Add(new Classes.playersData { deaths = player.deaths, kills = player.kills, name = player.name, score = player.score, bombsDetonated = player.bombsDetonated });
                        }
                        else
                        {
                            int index = massPlayerData.FindLastIndex(c => c.name == player.name);
                            int addKills = massPlayerData[index].kills;
                            int addDeaths = massPlayerData[index].deaths;
                            int addScore = massPlayerData[index].score;
                            int addBombs = massPlayerData[index].bombsDetonated;
                            massPlayerData[index] = new Classes.playersData() { deaths = player.deaths + addDeaths, kills = player.kills + addKills, name = player.name, score = player.score + addScore, bombsDetonated = player.bombsDetonated + addBombs };

                        }

                    }
                    foreach (var player in m_report.snapshot.teamInfo.team2.player)
                    {
                        bool addOrNot = true;
                        foreach (var name in massPlayerData)
                        {
                            if (player.name == name.name)
                            {
                                addOrNot = false;
                            }
                        }
                        if (addOrNot == true)
                        {
                            massPlayerData.Add(new Classes.playersData { deaths = player.deaths, kills = player.kills, name = player.name, score = player.score, bombsDetonated = player.bombsDetonated });
                        }
                        else
                        {
                            int index = massPlayerData.FindLastIndex(c => c.name == player.name);
                            int addKills = massPlayerData[index].kills;
                            int addDeaths = massPlayerData[index].deaths;
                            int addScore = massPlayerData[index].score;
                            int addBombs = massPlayerData[index].bombsDetonated;

                            massPlayerData[index] = new Classes.playersData() { deaths = player.deaths + addDeaths, kills = player.kills + addKills, name = player.name, score = player.score + addScore, bombsDetonated = player.bombsDetonated + addBombs };
                        }
                    }
                }

            }
            return massPlayerData;
        }

        private void btnResizeReport_Click(object sender, RoutedEventArgs e)
        {
            try {
                XAML.ResizeReport reportResize = new XAML.ResizeReport(Classes.UsefulFunctions.getFullReport(reports[lbReports.SelectedIndex]), reports[lbReports.SelectedIndex]);
                reportResize.Show();
            }catch(Exception)
            {

            }
        }

        private void btnAddBombs_Click(object sender, RoutedEventArgs e)
        {
            try {
                XAML.AddBombs addBombs = new XAML.AddBombs(Classes.UsefulFunctions.getFullReport(reports[lbReports.SelectedIndex]), reports[lbReports.SelectedIndex]);
                addBombs.Show();
            }catch(Exception)
            {

            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string[] guidArray = { "6846208b-0054-4d61-946e-983c43b93378",
                                   "44511019-2c41-470a-a074-ba0a68bc6de6",
                                   "be9790a7-1b47-42fd-8bb0-77bf53738694",
                                   "cede121d-1840-4c26-84eb-677f8e35a5d3",
                                   "8efa81c4-bf42-4e94-b168-79f60c80ad0f",
                                   ""

            };
            foreach (var guida in guidArray)
            {
                XAML.ScoreboardBasic basic = new XAML.ScoreboardBasic(guida);
                basic.Show();
            }
        }

        private void taskbarOpen_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void taskbarExit_Click(object sender, RoutedEventArgs e)
        {
            TaskbarIcon.Dispose();
            Environment.Exit(0);
        }

        private void btnAddTeams_Click(object sender, RoutedEventArgs e)
        {
            XAML.CreateTeam CreateTeamWindow = new XAML.CreateTeam();
            CreateTeamWindow.Show();
        }

        private void btnTeamData_Click(object sender, RoutedEventArgs e)
        {
            try {
                XAML.CreateTeamData TeamData = new XAML.CreateTeamData(CreateMassData());
                TeamData.Show();
            }catch(Exception)
            {

            }
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
        }

        private void txtBattlelogURL_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBattlelogURL.Text == "Enter a Battlelog server url.........")
            {
                txtBattlelogURL.Text = "";
            }
        }

        private async void button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Classes.Structs.PlayerLoadout playerInfo = await Classes.GetPlayersKit.GetWeaponInfo(350737813, "Dipsy-Ruddm9");
            string primaryWeaponString = "";
            string secondaryWeaponString = "";

            foreach (var weapon in Classes.JSONHelper.JsonHelperWeaponData)
                if(weapon.key == playerInfo.kitList[Convert.ToInt32(playerInfo.selectedKit)].kitIdInformation[0])
                {
                    Classes.JSONHelper.JsonHelperWeaponDataStrings.TryGetValue(weapon.weaponData.name, out primaryWeaponString);
                    //myValue is the name of the current weapon
                }
            foreach (var weapon in Classes.JSONHelper.JsonHelperWeaponData)
                if (weapon.key == playerInfo.kitList[Convert.ToInt32(playerInfo.selectedKit)].kitIdInformation[1])
                {
                    Classes.JSONHelper.JsonHelperWeaponDataStrings.TryGetValue(weapon.weaponData.name, out secondaryWeaponString);
                    //myValue is the name of the current weapon
                }
            MessageBox.Show("Primary Weapon: " + primaryWeaponString + " | Secondary Weapon: " + secondaryWeaponString);
        }

        private void btnServerRefresh_Click(object sender, RoutedEventArgs e)
        {
            lbCurrentServers.Items.Clear();
            Application.Current.Dispatcher.BeginInvoke(new Action(() => LoadServers()),System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        private void LogStats_Click(object sender, RoutedEventArgs e)
        {
            foreach (MyItemServers item in lbCurrentServers.SelectedItems)
            {
                XAML.ScoreboardBasic scoreboard = new XAML.ScoreboardBasic(item.svguid);
                scoreboard.Show();
            }
        }

        private void ViewServers_Click(object sender, RoutedEventArgs e)
        {
            foreach (MyItemServers item in lbCurrentServers.SelectedItems)
            {
                XAML.Scoreboard scoreboard = new XAML.Scoreboard(item.svguid);
                scoreboard.Show();
            }
        }
    }
}

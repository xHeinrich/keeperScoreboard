using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace keeperScoreboard.XAML
{
    /// <summary>
    /// Interaction logic for AddBombs.xaml
    /// </summary>
    public partial class AddBombs : Window
    {
        public long[] times;
        public string[] names;
        public string s_filename;
        List<Classes.CustomSnapshotRoot> rootInfo;
        public AddBombs(List<Classes.CustomSnapshotRoot> root, string filename)
        {

            rootInfo = Classes.UsefulFunctions.getFullReport(filename);
            s_filename = filename;
            times = Classes.UsefulFunctions.getTimes(rootInfo);
            names = Classes.UsefulFunctions.getPlayerNames(rootInfo);
            InitializeComponent();
            populatePlayerBox();
        }
        public void populatePlayerBox()
        {
            cbPlayerNames.Items.Clear();
            foreach (string name in names)
            {
                cbPlayerNames.Items.Add(name);
            }
            foreach (long time in times)
            {
                cbTimers.Items.Add(getTime(time));
            }
        }
        public string getTime(long time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);
            string str = t.ToString(@"hh\:mm\:ss");
            return str;
        }
        public void resetAll()
        {
            foreach (var row in rootInfo)
            {
                row.snapshot.team1Tickets.tickets = 0;
                row.snapshot.team2Tickets.tickets = 0;
                foreach (var player in row.snapshot.teamInfo.team1.player)
                {
                    if (player.bombsDetonated > 0)
                    {
                        player.bombsDetonated = 0;
                        player.bombDetonationTime.Clear();
                    }
                }
                foreach (var player in row.snapshot.teamInfo.team2.player)
                {
                    if (player.bombsDetonated > 0)
                    {
                        player.bombsDetonated = 0;
                        player.bombDetonationTime.Clear();
                    }
                }
            }
        }
        public void submit(long time, string name)
        {
            //string name = cbPlayerNames.SelectedValue.ToString();
            //string time = cbTimers.SelectedValue.ToString();
            foreach (var row in rootInfo)
            {
                if (row.snapshot.roundTime >= time)
                {
                    bool team1plant = false;
                    bool team2plant = false;
                    foreach (var player in row.snapshot.teamInfo.team1.player)
                    {
                        if (player.name == name)
                        {
                            player.bombsDetonated = player.bombsDetonated + 1;
                            List<long> longList = new List<long>();
                            longList.Add(time);
                            try
                            {
                                player.bombDetonationTime.AddRange(longList);
                            }
                            catch (Exception)
                            {
                                player.bombDetonationTime = longList;
                            }
                            team1plant = true;
                            break;
                        }
                    }
                    foreach (var player in row.snapshot.teamInfo.team2.player)
                    {
                        if (player.name == name)
                        {
                            player.bombsDetonated = player.bombsDetonated + 1;
                            List<long> longList = new List<long>();
                            longList.Add(time);
                            try
                            {
                                player.bombDetonationTime.AddRange(longList);
                            }
                            catch (Exception)
                            {
                                player.bombDetonationTime = longList;
                            }
                            team2plant = true;
                            break;
                        }
                    }
                    if (team1plant == true)
                    {
                        row.snapshot.team1Tickets.tickets += 1;
                    }
                    if (team2plant == true)
                    {
                        row.snapshot.team2Tickets.tickets += 1;
                    }

                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string name = cbPlayerNames.SelectedValue.ToString();
            long time = times[cbTimers.SelectedIndex];
            submit(time, name);
            string json = JsonConvert.SerializeObject(rootInfo.ToArray());
            System.IO.File.WriteAllText(s_filename, json);
            Classes.Logger.addLog("button_Click() - Wrote report: " + s_filename);
            // cbTimers.SelectedIndex
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
            string json = JsonConvert.SerializeObject(rootInfo.ToArray());
            System.IO.File.WriteAllText(s_filename, json);
            Classes.Logger.addLog("button1_Click() - Wrote report: " + s_filename);
        }
    }
}

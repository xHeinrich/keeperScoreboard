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
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
namespace keeperScoreboard.XAML
{
    /// <summary>
    /// Interaction logic for ResizeReport.xaml
    /// </summary>
    public partial class ResizeReport : Window
    {
        List<Classes.CustomSnapshotRoot> rewt;
        string filenameSave;
        public ResizeReport(List<Classes.CustomSnapshotRoot> root, string filename)
        {
            rewt = root;
            filenameSave = filename;
            InitializeComponent();
            pop();
            cbTimes.SelectedIndex = 0;
        }
        public void pop()
        {
            foreach(var rewt1 in rewt)
            {
                cbTimes.Items.Add(Classes.UsefulFunctions.getTime(rewt1.snapshot.roundTime, 1));
            }
        }
        public int getPlayerCount(Classes.CustomSnapshotRoot root)
        {
            int t1 = root.snapshot.teamInfo.team1.player.Count;
            int t2 = root.snapshot.teamInfo.team2.player.Count;

            return t1 + t2;
        }
        private void cbTimes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblPlayerCount.Content = "Player Count: " + getPlayerCount(rewt[cbTimes.SelectedIndex]).ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            XAML.Scoreboard scoreboard = new XAML.Scoreboard("", rewt[cbTimes.SelectedIndex]);
            scoreboard.Show();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Save 
            List<Classes.CustomSnapshotRoot> rewt2 = rewt;
            rewt2.RemoveRange(cbTimes.SelectedIndex+1, (rewt2.Count-cbTimes.SelectedIndex)-1);
            //MessageBox.Show("ff");
            Classes.Logger.addLog("btnSave_Click() - Reached log save: " + filenameSave);
            try {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\reports"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\reports");
                }
                string json = JsonConvert.SerializeObject(rewt2.ToArray());
                System.IO.File.WriteAllText(filenameSave, json);
                MessageBox.Show("Cut and saved file to: " + filenameSave);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }


}

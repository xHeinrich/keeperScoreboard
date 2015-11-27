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

namespace keeperScoreboard.XAML
{
    /// <summary>
    /// Interaction logic for MassPlayerData.xaml
    /// </summary>
    public partial class MassPlayerData : Window
    {
        List<Classes.playersData> massPlayerData = new List<Classes.playersData>();
        public MassPlayerData(List<Classes.playersData> massData, int reportCount)
        {
            massPlayerData = massData;
            InitializeComponent();
            PopulateData();
            this.Title = "Loaded from " + reportCount.ToString() + " reports";
            Classes.Logger.addLog("Opened mass report count of -> " + reportCount.ToString());
        }

        public void PopulateData()
        {
            massPlayerData = massPlayerData.OrderByDescending(x => x.score).ToList();
            lvPlayerData.Items.Clear();
            loadGridView();
            loadListViewItems();
        }
        public void loadListViewItems()
        {
            foreach(var player in massPlayerData)
            {
                lvPlayerData.Items.Add(new MyItemMassData { name = player.name, score = player.score, kills = player.kills, deaths = player.deaths });
            }
        }
        public void loadGridView()
        {
            Classes.Logger.addLog("loadGridView() - gridview loaded");
            var gridView = new GridView();
            this.lvPlayerData.View = gridView;
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Name",
                DisplayMemberBinding = new Binding("name")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Score",
                DisplayMemberBinding = new Binding("score")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Kills",
                DisplayMemberBinding = new Binding("kills")
            });
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Deaths",
                DisplayMemberBinding = new Binding("deaths")
            });
            //this.lbReports.Items.Add(new MyItem { Id = 1, Name = "Davidwroxy" });
        }
        public class MyItemMassData
        {
            public string name { get; set; }
            public int kills { get; set; }
            public int deaths { get; set; }
            public int score { get; set; }
        }

    }
}

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
    /// Interaction logic for CreateTeamData.xaml
    /// </summary>
    public partial class CreateTeamData : Window
    {
        private List<Classes.playersData> MassDataToSort = new List<Classes.playersData>();
        private List<Classes.playersData> TeamData = new List<Classes.playersData>();

        public CreateTeamData(List<Classes.playersData> Data)
        {
            MassDataToSort = Data;
            InitializeComponent();
            Classes.TeamData.ReadTeamData();
            ReloadTeams();
        }
        public void ReloadTeams()
        {
            TeamNames.Items.Clear();
            foreach (var TeamName in Classes.TeamData.ListOfTeam())
                TeamNames.Items.Add(TeamName);
            if (TeamNames.HasItems == true)
                TeamNames.SelectedIndex = 0;
        }
        public void SortTeamDataFromMassData()
        {
            if(TeamData != null)
                TeamData.Clear();
            foreach(var Team in MainWindow.CompetitiveTeamData)
            {
                if(Team.TeamName == (string)TeamNames.SelectedValue && Team.TeamMembers != null)
                {
                    foreach( var member in Team.TeamMembers)
                    {
                        foreach(var Player in MassDataToSort)
                        {
                            if(Player.name == member.MemberName)
                            {
                                TeamData.Add(Player);
                            }
                        }
                    }
                    return;
                }
            }
        }

        private void ViewReport_Click(object sender, RoutedEventArgs e)
        {
            SortTeamDataFromMassData();
            if(TeamData.Count <1)
            {
                MessageBox.Show("No data could found for this team");
                return;
            }
            XAML.TeamScoreboard scoreboard = new XAML.TeamScoreboard(TeamData, (string)TeamNames.SelectedValue);
            scoreboard.Show();
        }
    }
}

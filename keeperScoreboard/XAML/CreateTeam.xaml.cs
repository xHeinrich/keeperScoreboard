using System.Windows;

namespace keeperScoreboard.XAML
{
    /// <summary>
    /// Interaction logic for CreateTeam.xaml
    /// </summary>
    public partial class CreateTeam : Window
    {
        public CreateTeam()
        {
            
            InitializeComponent();
            Classes.TeamData.ReadTeamData();
            ReloadTeams();
        }
        public void ReloadTeams()
        {
            CurrentTeams.Items.Clear();
            string[] Teams = Classes.TeamData.ListOfTeam();
            if (Teams != null)
            {
                foreach (var TeamName in Teams)
                    CurrentTeams.Items.Add(TeamName);
                if (CurrentTeams.HasItems == true)
                    CurrentTeams.SelectedIndex = 0;
            }
        }

        private void SubmitTeam_Click(object sender, RoutedEventArgs e)
        {
            Classes.TeamData.AddTeam(TeamName.Text, "", "");
            ReloadTeams();
        }

        private void AddTeamMembers_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTeams.SelectedValue == null)
            {
                MessageBox.Show("Please create a team first");
            }
            else
            {
                XAML.AddTeamMembers AddMembers = new XAML.AddTeamMembers(CurrentTeams.SelectedValue.ToString());
                AddMembers.Show();
            }
        }
    }
}

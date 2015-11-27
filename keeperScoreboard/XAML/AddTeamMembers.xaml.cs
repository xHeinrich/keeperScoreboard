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
    /// Interaction logic for AddTeamMembers.xaml
    /// </summary>
    public partial class AddTeamMembers : Window
    {
        public string TeamNameCompare;
        public List<Classes.Structs.TeamMember> TeamMembers = new List<Classes.Structs.TeamMember>();
        public AddTeamMembers(string TeamName)
        {
            InitializeComponent();
            TeamNameCompare = TeamName;
            RefreshPlayerNames();
            Title = String.Format("Add Members To {0}", TeamNameCompare);
        }

        public void RefreshPlayerNames()
        {
            PlayerName.Items.Clear();
            foreach(var Teams in MainWindow.CompetitiveTeamData)
                if(Teams.TeamName == TeamNameCompare)
                    if(Teams.TeamMembers != null)
                    {
                        foreach (var Name in Teams.TeamMembers)
                        {
                            PlayerName.Items.Add(Name.MemberName);
                            TeamMembers = Teams.TeamMembers;
                        }
                        PlayerName.SelectedIndex = 0;
                    }

        }
        public bool DoesPlayerAlreadyExist(string PlayerName)
        {
            foreach(var Player in TeamMembers)
            {
                if (Player.MemberName == PlayerName)
                    return true;
            }
            return false;
        }
        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            Classes.Structs.TeamMember.CoreOrSub IsMemberCoreOrSub;
            if (CoreOrSubChoice.SelectedIndex == 0)
                IsMemberCoreOrSub = Classes.Structs.TeamMember.CoreOrSub.core;
            else
                IsMemberCoreOrSub = Classes.Structs.TeamMember.CoreOrSub.substitute;

            if(!DoesPlayerAlreadyExist(PlayerNameText.Text))
            {
                TeamMembers.Add(new Classes.Structs.TeamMember
                {
                    MemberName = PlayerNameText.Text,
                    MemberIsCoreOrSub = IsMemberCoreOrSub
                });
                Classes.TeamData.AddMembers(TeamNameCompare, TeamMembers);
                RefreshPlayerNames();
                MessageBox.Show(String.Format("Added {0} to {1}", PlayerNameText.Text, TeamNameCompare));
            }
            else
            {
                foreach(var player in TeamMembers)
                {
                    if(player.MemberName == PlayerName.SelectedValue.ToString())
                    {
                        player.MemberIsCoreOrSub = IsMemberCoreOrSub;
                        Classes.TeamData.SaveTeamData();
                    }
                }
                MessageBox.Show(String.Format("Edited {0} member {1}",TeamNameCompare, PlayerName.SelectedValue.ToString()));
            }
        }

        private void SaveTeamMembers_Click(object sender, RoutedEventArgs e)
        {
            Classes.TeamData.AddMembers(TeamNameCompare, TeamMembers);
        }

        private void PlayerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(var player in TeamMembers)
            {
                if(PlayerName.SelectedValue != null && player.MemberName == PlayerName.SelectedValue.ToString())
                {
                    CoreOrSubChoice.SelectedIndex = (int)player.MemberIsCoreOrSub;
                }
            }
        }
    }
}

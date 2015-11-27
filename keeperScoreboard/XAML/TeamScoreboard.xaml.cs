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
    /// Interaction logic for TeamScoreboard.xaml
    /// </summary>
    public partial class TeamScoreboard : Window
    {
        public TeamScoreboard(List<Classes.playersData> TeamData, string TeamName)
        {
            InitializeComponent();
            Title = "Team Stats - " + TeamName;
            Classes.ScoreboardCompetitiveTeamRenderer render = new Classes.ScoreboardCompetitiveTeamRenderer(this, TeamData, TeamName);

        }
    }
}

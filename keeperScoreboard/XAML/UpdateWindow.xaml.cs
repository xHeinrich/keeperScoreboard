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
using System.Diagnostics;
using System.Windows.Navigation;

namespace keeperScoreboard.XAML
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow(string newVer, string oldVer)
        {
            InitializeComponent();
            lblCurrent.Content += oldVer;
            lblNew.Content += newVer;
            Title += newVer;
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void bntYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void bntNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

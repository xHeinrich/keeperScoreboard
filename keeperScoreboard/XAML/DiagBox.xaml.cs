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
    /// Interaction logic for DiagBox.xaml
    /// </summary>
    public partial class DiagBox : Window
    {
        string m_description;
        public DiagBox(string description)
        {
            InitializeComponent();
            m_description = description;
            ResponseTextBox.Text = m_description;
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

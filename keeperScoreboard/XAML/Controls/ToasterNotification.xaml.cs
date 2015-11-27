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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace keeperScoreboard.XAML.Controls
{
    /// <summary>
    /// Interaction logic for ToasterNotification.xaml
    /// </summary>
    public partial class ToasterNotification : UserControl
    {
        public enum ErrorType { Error = 1, Log, Exception, Normal };
        public string MyText
        {
            get;
            set;
        }
        public ToasterNotification(string header, string message, ErrorType type = 0)
        {
            InitializeComponent();
            MyText = String.Format( message); 
            lblText.Content = header;
            lblSubMessage.Text = message;
            if(type == ErrorType.Error)
            {
                bgColor.Background = Brushes.Red;
            }
            if(type == ErrorType.Log)
            {
                bgColor.Background = Brushes.Yellow;
            }
            if(type == ErrorType.Exception)
            {
                bgColor.Background = Brushes.OrangeRed;
            }
            if(type == ErrorType.Normal)
            {
                bgColor.Background = Brushes.Green;
            }
        }
    }
}

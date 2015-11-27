using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Timers;
using Newtonsoft.Json;

namespace KeeperScoreboardServerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Classes.Structs.ServerInfoByTime> activePrivateServersTimeline = new List<Classes.Structs.ServerInfoByTime>();
        static Timer _timer;
        public static int activeServers = 0;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new Timer(30000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true;
        }
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Classes.BattlelogCrawler crawler = new Classes.BattlelogCrawler();
            Application.Current.Dispatcher.BeginInvoke(
                                         new Action(() => this.Title = "Active Sq Oblit Servers: " + activeServers.ToString()),
                                         DispatcherPriority.ApplicationIdle
                                     ); 
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _timer.Enabled = false;
            string output = JsonConvert.SerializeObject(activePrivateServersTimeline);
            File.WriteAllText(Directory.GetCurrentDirectory() + "/out.json", output);
            _timer.Enabled = true;
        }
    }
}

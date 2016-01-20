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
using System.Windows.Threading;
using System.Timers;
using System.Threading;
using CrashReporterDotNET;
using Newtonsoft.Json;
namespace StormBackend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static System.Timers.Timer ProgramLoop;

        public MainWindow()
        {
            Application.Current.DispatcherUnhandledException += ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            InitializeComponent();
            DataContext = new MainWindowViewModel();
            Program.IsRunning = true;
            ProgramLoop = new System.Timers.Timer(10000);
            ProgramLoop.Enabled = true;
            ProgramLoop.Elapsed += new ElapsedEventHandler(LogicLoop);
            Program.UpdateServers();            
        }
        static void LogicLoop(object sender, ElapsedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj => Program.UpdateServers()));


        }
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            ReportCrash((Exception)unhandledExceptionEventArgs.ExceptionObject);
            Environment.Exit(0);
        }

        private static void ApplicationThreadException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ReportCrash(e.Exception);
        }

        private static void ReportCrash(Exception exception)
        {
            var reportCrash = new ReportCrash
            {
                ToEmail = "nathan_heinrich@outlook.com"
            };

            reportCrash.Send(exception);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string export = JsonConvert.SerializeObject(Program.Servers);
            FileHandler.Save(export);
        }
    }
}

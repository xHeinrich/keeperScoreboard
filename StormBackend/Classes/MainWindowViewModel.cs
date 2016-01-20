using System.ComponentModel;
using System.Windows.Data;

namespace StormBackend
{
    public class MainWindowViewModel
    {
        public ICollectionView LoggingView { get; private set; }
        public ICollectionView Servers { get; private set; }

        public MainWindowViewModel()
        {
            LoggingView = CollectionViewSource.GetDefaultView(Program.Logs);
            Servers = CollectionViewSource.GetDefaultView(Program.Servers);
        }
    }
}

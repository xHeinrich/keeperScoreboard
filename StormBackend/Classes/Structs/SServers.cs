using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;
namespace StormBackend
{
    public class SServers : ObservableObject
    {
       
        public Guid _ServerGuid { get; set; }
        public string _ServerName { get; set; }
        public int _TickRate { get; set; }
        public int _PlayerSlots { get; set; }
        public string _MapID { get; set; }
        public string _Location { get; set; }
        public int _PlayersPlaying { get; set; }
        public DateTime _LastUpdate { get; set; }
        public List<CustomSnapshotRoot> _Snapshots { get ; set; }
        
        public List<CustomSnapshotRoot> Snapshots
        {
            get { return _Snapshots; }
            set
            {
                _Snapshots = value;
                NotifyPropertyChanged("Snapshots");
            }
        }
        
        public Guid ServerGuid
        {
            get { return _ServerGuid; }
            set
            {
                _ServerGuid = value;
                NotifyPropertyChanged("ServerGuid");
            }
        }

        public string ServerName
        {
            get { return _ServerName; }
            set
            {
                _ServerName = value;
                NotifyPropertyChanged("ServerName");
            }
        }

        public int TickRate
        {
            get { return _TickRate; }
            set
            {
                _TickRate = value;
                NotifyPropertyChanged("TickRate");
            }
        }

        public int PlayerSlots
        {
            get { return _PlayerSlots; }
            set
            {
                _PlayerSlots = value;
                NotifyPropertyChanged("PlayerSlots");
            }
        }
        public string MapID
        {
            get { return _MapID; }
            set
            {
                _MapID = value;
                NotifyPropertyChanged("MapID");
            }
        }

        public string Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
                NotifyPropertyChanged("Location");
            }
        }

        public int PlayersPlaying
        {
            get { return _PlayersPlaying; }
            set
            {
                _PlayersPlaying = value;
                NotifyPropertyChanged("PlayersPlaying");
            }
        }

        public DateTime LastUpdate
        {
            get { return _LastUpdate; }
            set
            {
                _LastUpdate = value;
                NotifyPropertyChanged("LastUpdate");
            }
        }
    }
}

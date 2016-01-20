using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace StormBackend
{
    public enum LogType
    {
        Error,
        Server,
        Update
    }
    public class SLogging : ObservableObject
    {
        public int _LogID { get; set; }
        public DateTime _LogTime { get; set; }
        public LogType _LogType  { get; set; }
        public string _LogString { get; set; }

        public int LogID
        {
            get { return _LogID; }
            set
            {
                _LogID = value;
                NotifyPropertyChanged("LogID");
            }
        }
        public DateTime LogTime
        {
            get { return _LogTime; }
            set
            {
                _LogTime = value;
                NotifyPropertyChanged("LogTime");
            }
        }
        public LogType LogType
        {
            get { return _LogType; }
            set
            {
                _LogType = value;
                NotifyPropertyChanged("LogType");
            }
        }
        public string LogString
        {
            get { return _LogString; }
            set
            {
                _LogString = value;
                NotifyPropertyChanged("LogString");
            }
        }
        
    }
}

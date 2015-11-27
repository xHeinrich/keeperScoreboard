using System.Collections.Generic;
using System.Windows.Media;

namespace keeperScoreboard.Classes
{
    public static class ReportFunctions
    {
        static public DoubleCollection viewTimes(List<Classes.CustomSnapshotRoot> m_report)
        {
            DoubleCollection doubleList = new DoubleCollection();
            foreach (var timers in m_report)
            {
                // times.Add(timers.time);
                doubleList.Add(double.Parse(timers.snapshot.roundTime.ToString()));
            }
            return doubleList;
        }
    }
}

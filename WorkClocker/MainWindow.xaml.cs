using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

namespace WorkClocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer DispatcherTimer;
        public Dictionary<string, int> AppTimes;
        public List<string> AppExcludeList;

        private int _totaltime;
        public int TotalTime
        {
            get { return _totaltime; }
            set
            {
                _totaltime = value;
                lElapsedTime.Content = TimeSpan.FromSeconds(_totaltime).ToString("g");
            }
        }

        private int _excludedtime;
        public int ExcludedTime
        {
            get { return _excludedtime; }
            set
            {
                _excludedtime = value;
                lExcludedTime.Content = TimeSpan.FromSeconds(_excludedtime).ToString("g");
            }
        }

        private int _workedtime;
        public int WorkedTime
        {
            get { return _workedtime; }
            set
            {
                _workedtime = value;
                lWorkedTime.Content = TimeSpan.FromSeconds(_workedtime).ToString("g");
            }
        }

        private void SetOrAddAppTime(string app)
        {
            if (AppTimes.ContainsKey(app))
            {
                AppTimes[app] = AppTimes[app] + 1;
            }
            else
            {
                AppTimes.Add(app, 1);
            }
        }

        public void CalculateTimes()
        {
            var worked = 0;
            var excluded = 0;

            foreach (var kv in AppTimes)
            {
                var appname = kv.Key;
                var apptime = kv.Value;

                if (AppExcludeList.Contains(appname))
                    excluded += apptime;
                else
                    worked += apptime;
            }

            ExcludedTime = excluded;
            WorkedTime = worked;
        }

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer = new DispatcherTimer();
            DispatcherTimer.Tick += DispatcherTimer_Tick;
            DispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            TotalTime = 0;
            AppTimes = new Dictionary<string, int>();
            AppExcludeList = new List<string>();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TotalTime++;

            var curApp = ActiveWindow.GetFocusWindow();
            if (curApp != null)
            {
                SetOrAddAppTime(curApp);
            }
        }

        private void GenerateList()
        {
            AppItems.Items.Clear();

            foreach (var kv in AppTimes)
            {
                var txt = String.Format("{0} - {1}", TimeSpan.FromSeconds(kv.Value).ToString("g"), kv.Key);
                var item = new ListBoxItem {Content = txt};
                AppItems.Items.Add(item);
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer.Start();
            BtnStart.IsEnabled = false;
            BtnStop.IsEnabled = true;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer.Stop();
            BtnStart.IsEnabled = true;
            BtnStop.IsEnabled = false;

            GenerateList();
            CalculateTimes();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            BtnStop_Click(sender, e);
            TotalTime = 0;
            AppTimes.Clear();
        }

        private void BtnExclude_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < AppItems.Items.Count; i++)
            {
                var item = (ListBoxItem)AppItems.Items[i];
                if (!item.IsSelected) continue;

                var s = (string) item.Content;
                var appName = s.Substring(s.IndexOf('-')+2);

                if (AppExcludeList.Contains(appName)) continue;
                AppExcludeList.Add(appName);
                item.FontStyle = FontStyles.Italic;
            }

            CalculateTimes();
        }

        private void BtnInclude_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < AppItems.Items.Count; i++)
            {
                var item = (ListBoxItem)AppItems.Items[i];
                if (!item.IsSelected) continue;

                var s = (string)item.Content;
                var appName = s.Substring(s.IndexOf('-') + 2);

                if (!AppExcludeList.Contains(appName)) continue;
                AppExcludeList.Remove(appName);
                item.FontStyle = FontStyles.Normal;
            }

            CalculateTimes();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WorkClocker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer {Interval = new TimeSpan(0,0,1)};
        private readonly Dictionary<string, int> _appTimes = new Dictionary<string, int>();
        private readonly List<string> _appExcludeList = new List<string>();

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
            if (_appTimes.ContainsKey(app))
            {
                _appTimes[app]++;
            }
            else
            {
                _appTimes.Add(app, 1);
            }
        }

        public void CalculateTimes()
        {
            WorkedTime = _appTimes.Where(o => !_appExcludeList.Contains(o.Key)).Sum(o => o.Value);
	        ExcludedTime = _appTimes.Where(o => _appExcludeList.Contains(o.Key)).Sum(o => o.Value);
        }

        public MainWindow()
        {
            InitializeComponent();
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
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
			
	        foreach (var item in _appTimes.Select(kv => string.Format("{0} - {1}", TimeSpan.FromSeconds(kv.Value).ToString("g"), kv.Key)).Select(txt => new ListBoxItem {Content = txt}))
	        {
		        AppItems.Items.Add(item);
	        }
        }

	    private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Start();
            BtnStart.IsEnabled = false;
            BtnStop.IsEnabled = true;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();
            BtnStart.IsEnabled = true;
            BtnStop.IsEnabled = false;

            GenerateList();
            CalculateTimes();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            BtnStop_Click(sender, e);
            TotalTime = 0;
            _appTimes.Clear();
        }

        private void BtnExclude_Click(object sender, RoutedEventArgs e)
        {
	        foreach (var t in AppItems.SelectedItems)
	        {
		        var item = (ListBoxItem)t;

		        var s = (string) item.Content;
		        var appName = s.Substring(s.IndexOf('-')+2);

		        if (_appExcludeList.Contains(appName)) continue;
		        _appExcludeList.Add(appName);
		        item.FontStyle = FontStyles.Italic;
	        }

	        CalculateTimes();
        }

	    private void BtnInclude_Click(object sender, RoutedEventArgs e)
        {
	        foreach (var t in AppItems.SelectedItems)
	        {
		        var item = (ListBoxItem)t;

		        var s = (string)item.Content;
		        var appName = s.Substring(s.IndexOf('-') + 2);

		        if (!_appExcludeList.Contains(appName)) continue;
		        _appExcludeList.Remove(appName);
		        item.FontStyle = FontStyles.Normal;
	        }

	        CalculateTimes();
        }
    }
}

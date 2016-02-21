using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using WorkClocker.Helpers;

namespace WorkClocker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly ViewModel.ViewModel _viewModel;
	    private readonly Stopwatch _stopwatch;

// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly MouseHookListener _mouseListener;
// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly KeyboardHookListener _keyboardListener;
	    private readonly WindowExe _afkExe = new WindowExe {Title = "Away From Keyboard", Exe = "Away From Keyboard", IsAfkExe = true};

        private System.Windows.Forms.NotifyIcon _notifyIcon;

        public int LastAction => _stopwatch.Elapsed.Seconds;

	    public MainWindow()
		{
		    InitializeComponent();

	        WindowExe.SetDefaultIcon(Properties.Resources.application);

			_viewModel = new ViewModel.ViewModel();
			_viewModel.Timer.Tick += DispatcherTimer_Tick;
			DataContext = _viewModel;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            _mouseListener = new MouseHookListener(new GlobalHooker()) { Enabled = true };
            _mouseListener.MouseDownExt += ActionListener;
            _mouseListener.MouseMove += ActionListener;
            _mouseListener.MouseWheel += ActionListener;

            _keyboardListener = new KeyboardHookListener(new GlobalHooker()) { Enabled = true };
            _keyboardListener.KeyDown += ActionListener;

            _viewModel.LoadFromDisk();

	        Stop();
		}

        private void ActionListener(object sender, object e)
        {
            _stopwatch.Restart();
	    }

	    private bool _hasResetPotentials;
	    private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (LastAction > Properties.Settings.Default.AFKDelay)
            {
                if (!_hasResetPotentials)
                {
                    _hasResetPotentials = true;
                    foreach (var window in _viewModel.AppTimes.SelectMany(appGroup => appGroup.Windows))
                    {
                        window.PotentialSeconds = 0;
                    }

                    _viewModel.SetOrAddAppTime(_afkExe, 0, Properties.Settings.Default.AFKDelay + 1);
                }
                else
                {
                    _viewModel.SetOrAddAppTime(_afkExe, 0);
                }

                _viewModel.UpdateProps();
                return;
            }
	        _hasResetPotentials = false;

			var curApp = Natives.GetFocusWindow();

            if (curApp?.Title != null)
            {
                TitleManipulator.CleanTitle(ref curApp);
                _viewModel.SetOrAddAppTime(curApp, LastAction);
			}
		}

	    public void Start()
        {
            _viewModel.Start();
            StartButton.IsEnabled = false;
            PauseButton.IsEnabled = true;

            _notifyIcon.Icon = Properties.Resources.clock_on;
        }

	    public void Stop()
        {
            _viewModel.Stop();
            StartButton.IsEnabled = true;
            PauseButton.IsEnabled = false;

	        _notifyIcon.Icon = Properties.Resources.clock_off;
        }

	    public void ToggleOnOff()
	    {
	        if (_viewModel.IsRunning())
	            Stop();
	        else
	            Start();
	    }

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
		    Start();
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
		    Stop();
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			PauseButton_Click(sender, e);
			_viewModel.Reset();
		}

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();
            window.Show();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveToDisk();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.SaveToDisk();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Reset();
            _viewModel.LoadFromDisk();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Click += notifyIcon_Click;
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
        }

	    private void notifyIcon_DoubleClick(object sender, EventArgs e)
	    {
	        ToggleOnOff();
	    }

	    private void notifyIcon_Click(object sender, EventArgs e)
	    {
	        Activate();
	    }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _notifyIcon.Visible = true;
        }
    }
}

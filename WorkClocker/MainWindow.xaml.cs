using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using WorkClocker.ViewModel;

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

	    public const int AFK_TIME = 30;

	    public int LastAction
	    {
	        get { return _stopwatch.Elapsed.Seconds; }
	    }

		public MainWindow()
		{
		    InitializeComponent();
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

			if (curApp != null && curApp.Title != null)
			{
				_viewModel.SetOrAddAppTime(curApp, LastAction);
			}
		}

		private void BtnStart_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.Start();
			BtnStart.IsEnabled = false;
			BtnStop.IsEnabled = true;
		}

		private void BtnStop_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.Stop();
			BtnStart.IsEnabled = true;
			BtnStop.IsEnabled = false;
		}

		private void BtnReset_Click(object sender, RoutedEventArgs e)
		{
			BtnStop_Click(sender, e);
			_viewModel.Reset();
		}

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow();
            window.Show();
        }
	}
}

using System;
using System.Linq;
using System.Windows;
using WorkClocker.ViewModel;

namespace WorkClocker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly ViewModel.ViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();
			_viewModel = new ViewModel.ViewModel();
			_viewModel.Timer.Tick += DispatcherTimer_Tick;
			DataContext = _viewModel;
		}

		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			var curApp = Natives.GetFocusWindow();

			if (curApp?.Title != null)
			{
				_viewModel.SetOrAddAppTime(curApp);
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
	}
}

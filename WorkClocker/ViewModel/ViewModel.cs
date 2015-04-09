using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using WorkClocker.Annotations;

namespace WorkClocker.ViewModel
{
	internal class ViewModel : INotifyPropertyChanged
	{
		public DispatcherTimer Timer { get; } = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
		public ObservableCollection<AppGroup> AppTimes { get; } = new ObservableCollection<AppGroup>();

		public TimeSpan IncludedTime => new TimeSpan(0, 0, AppTimes.Sum(o => o.IncludedTime.Seconds));

		public TimeSpan ExcludedTime => new TimeSpan(0, 0, AppTimes.Sum(o => o.ExcludedTime.Seconds));
		public TimeSpan TotalTime => new TimeSpan(0, 0, AppTimes.Sum(o => o.TotalTime.Seconds));

		public void SetOrAddAppTime(WindowExe app)
		{
			if (AppTimes.Any(o => o.Exe == app.Exe))
			{
				AppTimes.First(o => o.Exe == app.Exe).IncrementWindow(app.Title);
			}
			else
			{
				var nts = new AppGroup(app.Exe);
				nts.PropertyChanged += Nts_PropertyChanged;
				nts.IncrementWindow(app.Title);
				AppTimes.Add(nts);
			}

			PropChanged(nameof(IncludedTime));
			PropChanged(nameof(ExcludedTime));
			PropChanged(nameof(TotalTime));
		}

		private void Nts_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != @"IncludedTime" && e.PropertyName != @"ExcludedTime") return;

			PropChanged(nameof(IncludedTime));
			PropChanged(nameof(ExcludedTime));
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void PropChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		public void Reset()
		{
			AppTimes.Clear();
			
			PropChanged(nameof(IncludedTime));
			PropChanged(nameof(ExcludedTime));
			PropChanged(nameof(TotalTime));
		}

		public void Stop()
		{
			Timer.Stop();
		}

		public void Start()
		{
			Timer.Start();
		}
	}
}
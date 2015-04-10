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
		public ViewModel()
		{
			Timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
			AppTimes = new ObservableCollection<AppGroup>();
		}

		public DispatcherTimer Timer { get; private set; }
		public ObservableCollection<AppGroup> AppTimes { get; private set; }

		public TimeSpan IncludedTime
		{
			get { return new TimeSpan(0, 0, (int) AppTimes.Sum(o => o.IncludedTime.TotalSeconds)); }
		}

		public TimeSpan ExcludedTime
		{
			get { return new TimeSpan(0, 0, (int) AppTimes.Sum(o => o.ExcludedTime.TotalSeconds)); }
		}

		public TimeSpan TotalTime
		{
			get { return new TimeSpan(0, 0, (int) AppTimes.Sum(o => o.TotalTime.TotalSeconds)); }
		}

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

			PropChanged("IncludedTime");
			PropChanged("ExcludedTime");
			PropChanged("TotalTime");
		}

		private void Nts_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != @"IncludedTime" && e.PropertyName != @"ExcludedTime") return;

			PropChanged("IncludedTime");
			PropChanged("ExcludedTime");
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void PropChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
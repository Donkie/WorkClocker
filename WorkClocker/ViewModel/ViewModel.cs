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
			get { return new TimeSpan(0, 0, (int) AppTimes.Where(o => !o.App.IsAfkExe).Sum(o => o.IncludedTime.TotalSeconds)); }
		}

		public TimeSpan ExcludedTime
		{
			get { return new TimeSpan(0, 0, (int) AppTimes.Where(o => !o.App.IsAfkExe).Sum(o => o.ExcludedTime.TotalSeconds)); }
		}

		public TimeSpan TotalTime
		{
			get { return new TimeSpan(0, 0, (int) AppTimes.Where(o => !o.App.IsAfkExe).Sum(o => o.TotalTime.TotalSeconds)); }
		}

		public void SetOrAddAppTime(WindowExe app, int lastAction, int timeInc = 1)
		{
			if (AppTimes.Any(o => o.App.Exe == app.Exe))
			{
                AppTimes.First(o => o.App.Exe == app.Exe).IncrementWindow(app.Title, lastAction, timeInc);
			}
			else
			{
				var nts = new AppGroup(app);
				nts.PropertyChanged += Nts_PropertyChanged;
                nts.IncrementWindow(app.Title, lastAction, timeInc);
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

	    public void UpdateProps()
	    {
            PropChanged("IncludedTime");
            PropChanged("ExcludedTime");
            PropChanged("TotalTime");
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
			
			PropChanged("IncludedTime");
			PropChanged("ExcludedTime");
			PropChanged("TotalTime");
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
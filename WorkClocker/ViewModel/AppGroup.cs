using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WorkClocker.Annotations;

namespace WorkClocker.ViewModel
{
	internal class AppGroup : INotifyPropertyChanged
	{
		private bool _included = true;
		public string Exe { get; private set; }
		public ObservableCollection<TimeSlot> Windows { get; private set; }
		
		public TimeSpan TotalTime
		{
			get { return new TimeSpan(0, 0, Windows.Sum(o => o.Seconds)); }
		}

		public TimeSpan IncludedTime
		{
			get { return _included ? new TimeSpan(0, 0, Windows.Where(o => o.Included).Sum(o => o.Seconds)) : new TimeSpan(0); }
		}

		public TimeSpan ExcludedTime
		{
			get
			{
				return _included
					? new TimeSpan(0, 0, Windows.Where(o => !o.Included).Sum(o => o.Seconds))
					: new TimeSpan(0, 0, Windows.Sum(o => o.Seconds));
			}
		}

		public bool Included
		{
			get { return _included; }
			set
			{
				if (value == _included) return;
				_included = value;
				PropChanged();

				PropChanged("IncludedTime");
				PropChanged("ExcludedTime");
			}
		}

		public AppGroup(string exe)
		{
			Exe = exe;
			Windows = new ObservableCollection<TimeSlot>();
		}

		public void IncrementWindow(string title)
		{
			if (Windows.All(o => o.Title != title))
			{
				var timeSlot = new TimeSlot(title);
				timeSlot.PropertyChanged += TimeSlot_PropertyChanged;
				Windows.Add(timeSlot);
				return;
			}

			foreach (var timeSlot in Windows.Where(timeSlot => timeSlot.Title == title))
			{
				timeSlot.Seconds++;
			    break;
			}

			PropChanged("IncludedTime");
			PropChanged("ExcludedTime");
			PropChanged("TotalTime");
		}

		private void TimeSlot_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != @"Included") return;
			PropChanged("IncludedTime");
			PropChanged("ExcludedTime");
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void PropChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

	}
}

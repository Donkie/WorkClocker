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
		public string Exe { get; }
		public ObservableCollection<TimeSlot> Windows { get; } = new ObservableCollection<TimeSlot>();
		public TimeSpan TotalTime => new TimeSpan(0, 0, Windows.Sum(o => o.Seconds));
		public TimeSpan IncludedTime=> _included ? new TimeSpan(0,0, Windows.Where(o => o.Included).Sum(o => o.Seconds)) : new TimeSpan(0);
		public TimeSpan ExcludedTime => _included ? new TimeSpan(0, 0, Windows.Where(o=>!o.Included).Sum(o => o.Seconds)) : new TimeSpan(0,0, Windows.Sum(o => o.Seconds));
		public bool Included
		{
			get { return _included; }
			set
			{
				if (value == _included) return;
				_included = value;
				PropChanged();

				PropChanged(nameof(IncludedTime));
				PropChanged(nameof(ExcludedTime));
			}
		}

		public AppGroup(string exe)
		{
			Exe = exe;
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
			}

			PropChanged(nameof(IncludedTime));
			PropChanged(nameof(ExcludedTime));
			PropChanged(nameof(TotalTime));
		}

		private void TimeSlot_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != @"Included") return;
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

	}
}

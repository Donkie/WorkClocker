using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml.Serialization;
using WorkClocker.Annotations;
using WorkClocker.Helpers;

namespace WorkClocker.ViewModel
{
    [Serializable]
    public class AppGroup : INotifyPropertyChanged, IComparable
	{
		private bool _included = true;
		public WindowExe App { get; set; }
		public ObservableCollection<TimeSlot> Windows { get; set; }
        [XmlIgnore]
        public CollectionViewSource CvsWindows { get; set; }
        [XmlIgnore]
        public ICollectionView AllWindows => CvsWindows.View;

        private string _filter;
        [XmlIgnore]
        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                OnFilterChanged();
                PropChanged("FilterIncluded");
            }
        }

        private void OnFilterChanged()
        {
            CvsWindows.View.Refresh();
        }

        public TimeSpan TotalTime
		{
			get { return new TimeSpan(0, 0, Windows.Sum(o => o.TotalSeconds)); }
		}

		public TimeSpan IncludedTime
		{
            get { return _included ? new TimeSpan(0, 0, Windows.Where(o => o.Included).Sum(o => o.TotalSeconds)) : new TimeSpan(0); }
		}

		public TimeSpan ExcludedTime
		{
			get
			{
				return _included
                    ? new TimeSpan(0, 0, Windows.Where(o => !o.Included).Sum(o => o.TotalSeconds))
                    : new TimeSpan(0, 0, Windows.Sum(o => o.TotalSeconds));
			}
		}

        [XmlIgnore]
        public bool? FilterIncluded
        {
            get
            {
                var i = AllWindows.Cast<TimeSlot>().Count(v => v.Included);
                if (i == 0)
                    return false;
                if (i == AllWindows.Cast<TimeSlot>().Count())
                    return true;

                return null;
            }
            set
            {
                var boolin = false;
                if (value != null)
                    boolin = (bool) value;

                foreach (TimeSlot v in AllWindows)
                    v.Included = boolin;
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

        public AppGroup()
        {
            Windows = new ObservableCollection<TimeSlot>();

            CvsWindows = new CollectionViewSource {Source = Windows};
            CvsWindows.Filter += ApplyFilter;
        }

        private void ApplyFilter(object sender, FilterEventArgs e)
        {
            var tsvm = (TimeSlot) e.Item;

            if (string.IsNullOrWhiteSpace(Filter) || Filter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = tsvm.Title.ToLower().Contains(Filter.ToLower());
            }
        }

        public AppGroup(WindowExe exe)
		{
			App = exe;
			Windows = new ObservableCollection<TimeSlot>();
		}
        
		public void IncrementWindow(string title, int lastAction, int timeInc)
		{
            //Couldn't find any existing title
			if (Windows.All(o => o.Title != title))
			{
				var timeSlot = new TimeSlot(title);
				timeSlot.PropertyChanged += TimeSlot_PropertyChanged;
			    timeSlot.Seconds = timeInc;
				Windows.Add(timeSlot);

                if (Properties.Settings.Default.Autosort)
                    Windows.BubbleSort();
                return;
			}

			foreach (var timeSlot in Windows.Where(timeSlot => timeSlot.Title == title))
			{
			    if (lastAction < 1)
			    {
                    timeSlot.Seconds += timeInc;
			        timeSlot.TransferPotentialTime();
			    }
			    else
                    timeSlot.PotentialSeconds += timeInc;

			    break;
			}

            if(Properties.Settings.Default.Autosort)
                Windows.BubbleSort();

            PropChanged("IncludedTime");
			PropChanged("ExcludedTime");
			PropChanged("TotalTime");
            PropChanged("FilterIncluded");
        }

		public void TimeSlot_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != @"Included") return;
			PropChanged("IncludedTime");
            PropChanged("ExcludedTime");
            PropChanged("TotalTime");
            PropChanged("FilterIncluded");
		}

        #region INotifyPropertyChanged
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void PropChanged([CallerMemberName] string propertyName = null)
		{
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	    #endregion

        public int CompareTo(object obj)
        {
            var t = obj as AppGroup;
            if (t == null)
                throw new ArgumentException("Object is not a AppGroup");

            //We're basically using TotalTime here but skipping having to create a TimeSpan for each compare
            return t.Windows.Sum(o => o.TotalSeconds).CompareTo(Windows.Sum(o => o.TotalSeconds));
        }
	}
}

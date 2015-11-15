using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WorkClocker.Annotations;

namespace WorkClocker.ViewModel
{
	internal class TimeSlot : INotifyPropertyChanged
	{
		private int _seconds = 1;
	    private int _potentialseconds;
		private bool _included=true;

		public string Title { get; private set; }
		
		public bool Included
		{
			get { return _included; }
			set
			{
				if (value == _included) return;
				_included = value;
				PropChanged();
			}
		}

		public int Seconds
		{
			get { return _seconds; }
			set
			{
				if (value == _seconds) return;
				_seconds = value;
				PropChanged();
				PropChanged("Time");
			}
		}

	    public int TotalSeconds => _potentialseconds + _seconds;

	    public int PotentialSeconds
        {
            get { return _potentialseconds; }
            set
            {
                if (value == _potentialseconds) return;
                _potentialseconds = value;
                PropChanged();
                PropChanged("Time");
                PropChanged("PotentialTime");
                PropChanged("Included");
            }
        }

	    public void TransferPotentialTime()
	    {
            _seconds += PotentialSeconds;
            PotentialSeconds = 0;
	    }

		public TimeSpan Time => new TimeSpan(0, 0, TotalSeconds);

	    public TimeSpan PotentialTime => new TimeSpan(0, 0, PotentialSeconds);

	    public event PropertyChangedEventHandler PropertyChanged;

		public TimeSlot(string title)
		{
			Title = title;
		}
		[NotifyPropertyChangedInvocator]
		protected virtual void PropChanged([CallerMemberName] string propertyName = null)
		{
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
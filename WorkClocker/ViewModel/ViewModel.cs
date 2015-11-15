using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Xml.Serialization;
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

		public DispatcherTimer Timer { get; }
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
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WorkClocker.xml");
        public void LoadFromDisk()
        {
            var serializer = new XmlSerializer(typeof(ObservableCollection<AppGroup>));
            using (var reader = new StreamReader(_filePath))
            {
                AppTimes = (ObservableCollection<AppGroup>) serializer.Deserialize(reader);
                reader.Close();
            }
        }

        public void SaveToDisk()
        {
            var serializer = new XmlSerializer(typeof(ObservableCollection<AppGroup>));
            using (var writer = new StreamWriter(_filePath))
                serializer.Serialize(writer, AppTimes);
        }
    }
}
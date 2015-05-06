using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;
using System.Data.Linq;
using System.Threading;
using System.Globalization;
using System.Linq;
using VeniceTideCommon.Code.Utils;

namespace VeniceTideCommon.Code.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
			this.TideDataPoints = new ObservableCollection<TideDataPointViewModel>();
		}

		public static MainViewModel CurrentMainViewModel;

		public ObservableCollection<TideDataPointViewModel> TideDataPoints { get; private set; }
		public ObservableCollection<LocationHeightViewModel> LocationHeights { get; private set; }
		public ObservableCollection<LocationHeightViewModel> FavoriteLocations { get; private set; }

		private string newRawData;
		public bool DownloadOk { get { return newRawData != null; } }
		public bool DownloadCancelled { get; private set; }
		private bool DownloadRunning { get; set; }

		private DateTime _lastUpdated = DateTime.Now;
		public DateTime LastUpdated
		{
			get
			{
				return _lastUpdated;
			}
			set
			{
				if (value != _lastUpdated)
				{
					_lastUpdated = value;
					NotifyPropertyChanged("LastUpdated");
					NotifyPropertyChanged("CurrentTideLevel");

					foreach (LocationHeightViewModel lhvm in LocationHeights)
						lhvm.CurrentHeightChanged();

					foreach (LocationHeightViewModel lhvm in FavoriteLocations)
						lhvm.CurrentHeightChanged();
				}
			}
		}

		public int CurrentTideLevel
		{
			get
			{
				//return 95;
				int val = 0;
				if (TideDataPoints.Count > 2)
				{
					TideDataPointViewModel first = TideDataPoints[0];
					TideDataPointViewModel second = TideDataPoints[1];

					if (first.Time > DateTime.Now)
					{
						second = TideDataPoints[0];
						first = new TideDataPointViewModel() { Time = second.Time.Subtract(TimeSpan.FromHours(6)), IsMaximum = !second.IsMaximum, Height = TideDataPoints[1].Height };
					}

					float perc = (float)(DateTime.Now.Ticks - first.Time.Ticks) / (float)(second.Time.Ticks - first.Time.Ticks);

					if (perc < 0)
						perc = Math.Abs(perc);

					return (int)Math.Round((float)first.Height + Math.Sin(perc * Math.PI / 2.0) * (float)(second.Height - first.Height));
				}
				return val;
			}
		}

		public bool IsDataLoaded
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates and adds a few ItemViewModel objects into the Items collection.
		/// </summary>
		public void LoadLocalData(bool loadCachedForecast = true)
		{
			if (loadCachedForecast)
			{
				TideDataPoints = (Settings.Get(Settings.SettingKey.Data) as ObservableCollection<TideDataPointViewModel>) ?? new ObservableCollection<TideDataPointViewModel>();
				NotifyPropertyChanged("TideDataPoints");
				NotifyPropertyChanged("CurrentTideLevel");
			}

			LoadLocationHeights();
			NotifyPropertyChanged("LocationHeights");

			LoadFavoriteLocations();
			NotifyPropertyChanged("FavoriteLocations");

			this.IsDataLoaded = true;
		}

		private void LoadFavoriteLocations()
		{
			if (!Settings.Exists(Settings.SettingKey.FavoriteLocations))
			{
				if (FavoriteLocations == null)
				{
					FavoriteLocations = new ObservableCollection<LocationHeightViewModel>();
				}
				else
				{
					FavoriteLocations.Clear();
				}

				FavoriteLocations.Add(LocationHeights.First(x => x.Name == "Accademia"));
				FavoriteLocations.Add(LocationHeights.First(x => x.Name == "P.zza S. Marco"));
				FavoriteLocations.Add(LocationHeights.First(x => x.Name == "Strada Nuova"));
				FavoriteLocations.Add(LocationHeights.First(x => x.Name == "F.ta di Cannaregio"));
				FavoriteLocations.Add(LocationHeights.First(x => x.Name == "Ruga Rialto"));
			}
			else
			{
				FavoriteLocations = Settings.Get(Settings.SettingKey.FavoriteLocations) as ObservableCollection<LocationHeightViewModel>;
			}
		}

		private void LoadLocationHeights()
		{
			if (LocationHeights == null)
			{
				LocationHeights = new ObservableCollection<LocationHeightViewModel>();
			}
			else
			{
				LocationHeights.Clear();
			}

			LocationHeights.Add(new LocationHeightViewModel { Name = "Accademia", Height = 115 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Biennale Giardini", Height = 175 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Calle dei Fabbri", Height = 100 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Calle della Bissa", Height = 122 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Calle di Canonica", Height = 95 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Calle Larga Foscari", Height = 125 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Calle Lunga S. Barnaba", Height = 90 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Calle Vallaresso", Height = 115 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo dei Frari", Height = 120 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo del Ghetto", Height = 120 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo dell'Abbazia", Height = 100 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo della Guerra", Height = 105 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo della Salute", Height = 150 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Alvise", Height = 130 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Barnaba", Height = 105 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Bartolomeo", Height = 125 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Lio", Height = 135 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Luca", Height = 124 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Margherita", Height = 120 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Martino", Height = 120 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Moisè", Height = 95 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Pietro", Height = 155 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Salvador", Height = 115 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Stefano", Height = 120 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Trovaso", Height = 110 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Campo S. Vio", Height = 130 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta dei Cereri", Height = 140 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta della Misericordia", Height = 110 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta di Cannaregio", Height = 95 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta Ormesini", Height = 110 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta Rialto", Height = 105 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta S. Giobbe", Height = 95 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta S. Sebastiano", Height = 95 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta Tolentini", Height = 115 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "F.ta Zattere S. Basilio", Height = 104 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Fondazione Guggenheim", Height = 120 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Frezzaria", Height = 100 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Giudecca Palanca", Height = 110 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Giudecca Redentore", Height = 115 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Lista di Spagna", Height = 105 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Merceria dell'Orologio", Height = 90 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Merceria S. Salvador", Height = 115 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Merceria S. Zulian", Height = 105 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "P.zza S. Marco", Height = 80 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Piscina S. Alvise", Height = 120 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Piscina Sacca Fisola", Height = 165 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Rio Terrà Foscarini", Height = 115 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Riva degli Schiavoni", Height = 110 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Riva del Vin", Height = 90 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Ruga Rialto", Height = 115 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "S.Elena", Height = 175 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "S. Giorgio Maggiore", Height = 105 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Salizzada S. Canciano", Height = 110 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Strada Nuova", Height = 120 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Via Garibaldi", Height = 125 });
			LocationHeights.Add(new LocationHeightViewModel { Name = "Via XXII Marzo", Height = 105 });
		}

		public void DownloadData(BackgroundWorker loadingWorker = null)
		{
			this.DownloadCancelled = false;

			var signaler = new ManualResetEvent(false); try
			{
				DownloadRunning = true;
				WebClient client = new WebClient();
				client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
				client.AllowReadStreamBuffering = true;
				client.DownloadStringAsync(new Uri("http://lionfisk.com/apps/tide/tidedata.php"), signaler);

				while (DownloadRunning && (loadingWorker == null || !loadingWorker.CancellationPending))
				{
					signaler.WaitOne(50);
				}

				if (loadingWorker != null && loadingWorker.CancellationPending)
				{
					client.CancelAsync();
					this.DownloadCancelled = true;
				}
			}
			finally
			{
				signaler.Dispose();
			}
		}

		public class IncomingTideData
		{
			public string t;
			public string x;
			public int v;
		}

		void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs evt)
		{
			if (evt.Error == null)
			{
				this.newRawData = evt.Result;
			}
			else
			{
				this.newRawData = null;
			}

			this.DownloadCancelled = evt.Cancelled;

			DownloadRunning = false;
			try
			{
				((ManualResetEvent)evt.UserState).Set();
			}
			catch (ObjectDisposedException ex)
			{
				// Ignore 
			}
		}

		public void UpdateFromRawData(bool saveToLS = true)
		{
			List<IncomingTideData> data = JsonConvert.DeserializeObject<List<IncomingTideData>>(this.newRawData);

            // Sample data:
            //data[0].v = -10;
            //data[1].v = 75;
            //data[2].v = 10;
            //data[3].v = 100;
            //data[4].v = 35;
            //data[5].v = 135;

			TideDataPoints.Clear();
			foreach (IncomingTideData d in data)
			{
				TideDataPoints.Add(new TideDataPointViewModel()
				{
					Time = DateTime.Parse(d.t, new CultureInfo("it-IT")),
					Height = d.v,
					IsMaximum = d.x == "+"
				});
			}

			if (saveToLS)
			{
				Settings.Set(Settings.SettingKey.Data, TideDataPoints);
			}

			NotifyPropertyChanged("Items");
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
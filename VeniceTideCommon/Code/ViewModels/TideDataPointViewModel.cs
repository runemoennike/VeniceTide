using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using VeniceTideCommon.Resources.Localization;

namespace VeniceTideCommon.Code.ViewModels
{
	public class TideDataPointViewModel : INotifyPropertyChanged
	{
		private DateTime _time;
		public DateTime Time
		{
			get
			{
				return _time;
			}
			set
			{
				if (value != _time)
				{
					_time = value;
					NotifyPropertyChanged("Time");
					NotifyPropertyChanged("FormattedDay");
					NotifyPropertyChanged("FormattedTime");
				}
			}
		}

		public string FormattedDay
		{
			get
			{
				string day = Time.ToString("dddd", CultureInfo.CurrentUICulture);
				return char.ToUpper(day[0]) + day.Substring(1);
			}
		}

		public string FormattedTime
		{
			get
			{
				return Time.ToString("t", CultureInfo.CurrentUICulture);
			}
		}

		private bool _isMaximum;
		public bool IsMaximum
		{
			get
			{
				return _isMaximum;
			}
			set
			{
				if (value != _isMaximum)
				{
					_isMaximum = value;
					NotifyPropertyChanged("IsMaximum");
					NotifyPropertyChanged("IsMinimum");
				}
			}
		}

		public bool IsMinimum
		{
			get { return !IsMaximum; }
		}

		private int _height;
		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				if (value != _height)
				{
					_height = value;
					NotifyPropertyChanged("Height");
					NotifyPropertyChanged("HeightBrush");
					NotifyPropertyChanged("FloodedLocationsText");
				}
			}
		}

		public Brush HeightBrush
		{
			get
			{
				if (Height < -90)
					return new SolidColorBrush(Color.FromArgb(255, 27, 161, 226));
				else if (Height < -51)
					return (Brush)Application.Current.Resources["PhoneForegroundBrush"];
				else if (Height < 79)
					return new SolidColorBrush(Color.FromArgb(255, 51, 153, 51));
				else if (Height < 109)
					return new SolidColorBrush(Color.FromArgb(255, 240, 150, 9));
				else if (Height < 139)
					return new SolidColorBrush(Color.FromArgb(255, 255, 102, 0));
				else
					return new SolidColorBrush(Color.FromArgb(255, 229, 20, 0));

			}
		}

		public string FloodedLocationsText
		{
			get
			{
				var floodedLocations = MainViewModel.CurrentMainViewModel.FavoriteLocations.Where(x => x.Height - 2 < Height);
				return floodedLocations.Count() > 0 ? string.Format(AppResources.TideDPFloodedLocs, string.Join(AppResources.ListSeparatorWithSpace, floodedLocations.Select(x => x.Name)))
					: AppResources.TideDPNoFloodedLocs;
			}
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
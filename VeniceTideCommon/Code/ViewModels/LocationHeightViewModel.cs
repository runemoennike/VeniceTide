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
	public class LocationHeightViewModel : INotifyPropertyChanged
	{
		private string _name;
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (value != _name)
				{
					_name = value;
					NotifyPropertyChanged("Name");
				}
			}
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
					CurrentHeightChanged();				
				}
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public string HeightText
		{
			get
			{
				return string.Format(AppResources.LocHeightText, Height);
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public bool IsFlooded
		{
			get { return MainViewModel.CurrentMainViewModel.CurrentTideLevel > Height + 3; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public bool IsNotFlooded
		{
			get { return MainViewModel.CurrentMainViewModel.CurrentTideLevel < Height - 8; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public bool IsMaybeFlooded
		{
			get { return !IsFlooded && !IsNotFlooded; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public int FloodAmount
		{
			get { return MainViewModel.CurrentMainViewModel.CurrentTideLevel - Height; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public string HeightDescription
		{
			get
			{
				string r = string.Format(AppResources.LocHeightDescLead, Height) + " ";
				if (IsFlooded)
				{
					r += string.Format(AppResources.LocHeightDescFlooded, MainViewModel.CurrentMainViewModel.CurrentTideLevel - Height);
				}
				else if (IsNotFlooded)
				{
					r += string.Format(AppResources.LocHeightDescAbove, Height - MainViewModel.CurrentMainViewModel.CurrentTideLevel);
				}
				else if (Height > MainViewModel.CurrentMainViewModel.CurrentTideLevel)
				{
					r += string.Format(AppResources.LocHeightDescSlightlyAbove, Height - MainViewModel.CurrentMainViewModel.CurrentTideLevel);
				}
				else
				{
					r += string.Format(AppResources.LocHeightDescSlightlyUnder, MainViewModel.CurrentMainViewModel.CurrentTideLevel - Height);
				}
				return r;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public bool IsFavorite
		{
			get
			{
				if (MainViewModel.CurrentMainViewModel.FavoriteLocations == null)
					return false;

				return MainViewModel.CurrentMainViewModel.FavoriteLocations.Any(x => x.Name == this.Name);
			}
			set
			{
				if (MainViewModel.CurrentMainViewModel.FavoriteLocations == null)
					return;

				if (value)
				{
					if (!IsFavorite)
					{
						MainViewModel.CurrentMainViewModel.FavoriteLocations.Add(this);
					}
				}
				else
				{
					if (IsFavorite)
					{
						MainViewModel.CurrentMainViewModel.FavoriteLocations.Remove(MainViewModel.CurrentMainViewModel.FavoriteLocations.First(x => x.Name == this.Name));
					}
				}

				NotifyPropertyChanged("IsFavorite");
			}
		}

		public void CurrentHeightChanged() {
			NotifyPropertyChanged("IsFlooded");
			NotifyPropertyChanged("IsNotFlooded");
			NotifyPropertyChanged("IsMaybeFlooded");
			NotifyPropertyChanged("HeightDescription");
			NotifyPropertyChanged("FloodAmount");
			NotifyPropertyChanged("HeightText");
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
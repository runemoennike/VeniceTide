using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VeniceTideCommon.Code.Utils;
using VeniceTideCommon.Resources.Localization;

namespace VeniceTide
{
	public partial class MainPage : PhoneApplicationPage
	{
		private BackgroundWorker loadingWorker;

		// Constructor
		public MainPage()
		{
			InitializeComponent();
			BuildLocalizedApplicationBar();

			// Set the data context of the listbox control to the sample data
			DataContext = App.ViewModel;
			this.Loaded += new RoutedEventHandler(MainPage_Loaded);

			DownloadFreshData();
		}

		private void BuildLocalizedApplicationBar()
		{
			// Set the page's ApplicationBar to a new instance of ApplicationBar.
			ApplicationBar = new ApplicationBar();

			// Create a new button and set the text value to the localized string from AppResources.
			ApplicationBarIconButton locationsBtn = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.Relative));
			locationsBtn.Text = AppResources.AppBarLocations;
			locationsBtn.Click += AppBar_SelectFavs_Click;
			ApplicationBar.Buttons.Add(locationsBtn);

			// Create a new menu item with the localized string from AppResources.
			ApplicationBarMenuItem aboutMntm = new ApplicationBarMenuItem(AppResources.AppBarAbout);
			aboutMntm.Click += AppBar_About_Click;
			ApplicationBar.MenuItems.Add(aboutMntm);

			ApplicationBarMenuItem livetileMntm = new ApplicationBarMenuItem(AppResources.AppBarLiveTile);
			livetileMntm.Click += AppBar_LiveTile_Click;
			ApplicationBar.MenuItems.Add(livetileMntm);
		}

		public void DownloadFreshData()
		{
			if (loadingWorker != null && loadingWorker.IsBusy)
			{
				//while (loadingWorker.IsBusy)
				//	Thread.Sleep(200);
				return;
			}

			loadingWorker = new BackgroundWorker();
			loadingWorker.WorkerSupportsCancellation = true;
			Progress.IsVisible = true;

			loadingWorker.DoWork += ((s, args) =>
			{
				App.ViewModel.DownloadData(loadingWorker);
			});

			loadingWorker.RunWorkerCompleted += ((s, args) =>
			{
				this.Dispatcher.BeginInvoke(() =>
				{
					Progress.IsVisible = false;
					if (App.ViewModel.DownloadOk)
					{
						App.ViewModel.UpdateFromRawData();
						App.ViewModel.LastUpdated = DateTime.Now;
					}
					else if(!App.ViewModel.DownloadCancelled)
					{
						MessageBox.Show("An error occured while updating tidal data from the internet. Please make sure your device is connected to the internet, and try again later.");
					}
				}
				);
			});

			loadingWorker.RunWorkerAsync();
		}

		// Handle selection changed on ListBox
		private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//// If selected index is -1 (no selection) do nothing
			//if (MainListBox.SelectedIndex == -1)
			//	return;

			//// Navigate to the new page
			//NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + MainListBox.SelectedIndex, UriKind.Relative));

			//// Reset selected index to -1 (no selection)
			//MainListBox.SelectedIndex = -1;
		}

		// Load data for the ViewModel Items
		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!App.ViewModel.IsDataLoaded)
			{
				App.ViewModel.LoadLocalData();
			}
		}

		private void AppBar_SelectFavs_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Pages/SelectLocations.xaml", UriKind.Relative));
		}

		private void AppBar_About_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative));
		}

		private void AppBar_LiveTile_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Pages/LiveTilePage.xaml", UriKind.Relative));
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.IsNavigationInitiator)
			{
				Settings.Set(Settings.SettingKey.FavoriteLocations, App.ViewModel.FavoriteLocations);
			}
			else
			{
				DownloadFreshData();
			}
		}

		protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);

			if (loadingWorker.IsBusy)
			{
				loadingWorker.CancelAsync();
			}
		}

		private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
		}
	}
}
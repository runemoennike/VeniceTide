using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VeniceTide.Code;
using VeniceTideAgent.Code;
using VeniceTideCommon.Code;
using VeniceTideCommon.Code.ViewModels;
using VeniceTideCommon.Resources.Localization;

namespace VeniceTide.Pages
{
	public partial class LiveTilePage : PhoneApplicationPage
	{
		public LiveTilePage()
		{
			InitializeComponent();
		}

		private void LiveTileBtn_Click(object sender, RoutedEventArgs e)
		{
			LiveTileBtn.IsEnabled = false;

			var dtNow = DateTime.Now;
			if (!MainViewModel.CurrentMainViewModel.DownloadOk || MainViewModel.CurrentMainViewModel.TideDataPoints.Count(x => x.Time > dtNow && x.IsMaximum) < 2)
			{
				MessageBox.Show(AppResources.LiveTileDataNotFresh);
			}

			try
			{
				ScheduledAgentTools.Install();
				LiveTileBuilderFull.CreateNew();
			}
			catch(InvalidOperationException ex)
			{
				if (ex.Message.Contains("UserDisabled"))
				{
					MessageBox.Show(AppResources.BackgroundAgentDisabledErrorMsg);
					LiveTileBtn.IsEnabled = true;
				}
				else
				{
					throw ex;
				}
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			LiveTileBtn.IsEnabled = true;
		}
	}
}
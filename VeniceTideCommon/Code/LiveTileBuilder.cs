using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using VeniceTideCommon.Code.Utils;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using VeniceTideCommon.Resources.Localization;
using VeniceTideCommon.Code.ViewModels;
using System.Reflection;

namespace VeniceTideCommon.Code
{
	public class LiveTileBuilderLimited
	{

		public const string LOCATIONS_FONT = "SegoeWP";
		public const int LOCATIONS_FONTSIZE = 36;
		public const int LOCATIONS_SPACING = -1;
		public const string FORECAST_FONT = "SegoeWP";
		public const int FORECAST_FONTSIZE = 36;
		public const int FORECAST_SPACING = 2;
		public const string TITLES_FONT = "SegoeWP SemiBold";
		public const int TITLES_FONTSIZE = 30;

		public const int TILE_SIZE = 336;
		public const int DIVISION_POS = 217;
		public const int INFO_LEFT_MARGIN = 74;

		public const string LIVETILE_FILENAME = "/Shared/ShellContent/tile.png";

		public static bool Update()
		{
			BuildBitmap();

			ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("islivetile"));

			if (tile != null)
			{
				var data = new StandardTileData
				{
					BackgroundImage = new Uri("isostore:" + LIVETILE_FILENAME, UriKind.Absolute)
				};
				tile.Update(data);

				return true;
			}
			else
			{
				return false;
			}
		}

		protected static void BuildBitmap()
		{
			BitmapFont.RegisterFont("SegoeWP", new int[] { 18, 36 });
			BitmapFont.RegisterFont("SegoeWP SemiBold", new int[] { 15, 30 });
			//BitmapFont.RegisterFont("SegoeWP Bold", new int[] { 15, 18, 20 });

			WriteableBitmap iconLocationOk = LoadWritableBitmap("Resources/Images/icon-location-ok.png");
			WriteableBitmap iconLocationMaybeFlooded = LoadWritableBitmap("Resources/Images/icon-location-maybe-flooded.png");
			WriteableBitmap iconLocationFlooded = LoadWritableBitmap("Resources/Images/icon-location-flooded.png");

			//WriteableBitmap bmp = new WriteableBitmap(173, 173);
			WriteableBitmap bmp = new WriteableBitmap(TILE_SIZE, TILE_SIZE);

			// Background
			bmp.Clear(Colors.Black);
			bmp.FillRectangle(0, DIVISION_POS, TILE_SIZE, TILE_SIZE, (Color)Application.Current.Resources["PhoneAccentColor"]);

			// Locations
			int i = 0;
			foreach (LocationHeightViewModel lhvm in MainViewModel.CurrentMainViewModel.FavoriteLocations.OrderByDescending(x => x.IsFlooded).ThenByDescending(x => x.IsMaybeFlooded).ThenBy(x => x.Name))
			{
				bmp.DrawString(lhvm.Name, INFO_LEFT_MARGIN, i * (LOCATIONS_FONTSIZE + LOCATIONS_SPACING), LOCATIONS_FONT, LOCATIONS_FONTSIZE, Colors.White);

				WriteableBitmap icon;
				if (lhvm.IsFlooded)
				{
					icon = iconLocationFlooded;
				}
				else if (lhvm.IsMaybeFlooded)
				{
					icon = iconLocationMaybeFlooded;
				}
				else
				{
					icon = iconLocationOk;
				}

				Rect iconPos = new Rect(INFO_LEFT_MARGIN - 15, i * (LOCATIONS_FONTSIZE + LOCATIONS_SPACING) + LOCATIONS_FONTSIZE / 2.0 - 2, 10, 10);
				bmp.Blit(iconPos, icon, new Rect(0, 0, 10, 10), WriteableBitmapExtensions.BlendMode.Alpha);


				i++;
				if (i >= 6)
					break;
			}

			// Forecast
			i = 0;
			DateTime dtNow = DateTime.Now;
			foreach (TideDataPointViewModel tdpvm in MainViewModel.CurrentMainViewModel.TideDataPoints.Where(x => x.Time >= dtNow && x.IsMaximum).OrderBy(x => x.Time))
			{
				string levelStr = String.Format("{0}cm", tdpvm.Height);
				int levelStrWidth = (int)BitmapFont.MeasureString(levelStr, FORECAST_FONT, FORECAST_FONTSIZE).Width;

				string timeStr = String.Format("{1}", tdpvm.Time.ToString("ddd", CultureInfo.CurrentUICulture), tdpvm.Time.ToShortTimeString());
				int timeStrWidth = (int)BitmapFont.MeasureString(timeStr, FORECAST_FONT, FORECAST_FONTSIZE).Width;

				bmp.DrawString(timeStr, INFO_LEFT_MARGIN, DIVISION_POS + i * (FORECAST_FONTSIZE + FORECAST_SPACING), FORECAST_FONT, FORECAST_FONTSIZE, Colors.White);
				bmp.DrawString(levelStr, TILE_SIZE - levelStrWidth - 10, DIVISION_POS + i * (FORECAST_FONTSIZE + FORECAST_SPACING), FORECAST_FONT, FORECAST_FONTSIZE, Colors.White);

				i++;
				if (i >= 3)
					break;
			}

			// Titles
			bmp = bmp.Rotate(90);

			int titleForecastWidth = (int)BitmapFont.MeasureString(AppResources.LiveTileForecastTitle, TITLES_FONT, TITLES_FONTSIZE).Width;
			bmp.DrawString(AppResources.LiveTileForecastTitle, TILE_SIZE - DIVISION_POS - titleForecastWidth - 6, 6, TITLES_FONT, TITLES_FONTSIZE, Colors.Black);

			int titleCurrentlyWidth = (int)BitmapFont.MeasureString(AppResources.LiveTileCurrentlyTitle, TITLES_FONT, TITLES_FONTSIZE).Width;
			bmp.DrawString(AppResources.LiveTileCurrentlyTitle, TILE_SIZE - DIVISION_POS + 6, 6, TITLES_FONT, TITLES_FONTSIZE, (Color)Application.Current.Resources["PhoneAccentColor"]);

			//bmp.DrawString(DateTime.Now.ToString("T"), 250, 6, "SegoeWP", 18, Colors.White);


			bmp = bmp.Rotate(270);

			// Debug
#if DEBUG
			bmp.DrawString(new Random().Next(1, 500).ToString(), 0, 0, TITLES_FONT, TITLES_FONTSIZE, Colors.White);
#endif

			// Finalize
			bmp.Invalidate();

			var isf = IsolatedStorageFile.GetUserStoreForApplication();

			if (!isf.DirectoryExists("/Shared/ShellContent"))
			{
				isf.CreateDirectory("/Shared/ShellContent");
			}

			using (var stream = isf.OpenFile(LIVETILE_FILENAME, System.IO.FileMode.OpenOrCreate))
			{
				stream.Seek(0, System.IO.SeekOrigin.Begin);
				//bmp.SaveJpeg(stream, 173, 173, 0, 100);
				bmp.WritePNG(stream);

				stream.Flush();
				stream.Close();
			}
		}

		protected static WriteableBitmap LoadWritableBitmap(string path)
		{
			BitmapImage bi = new BitmapImage();
			
			bi.SetSource(Application.GetResourceStream(new Uri(path, UriKind.Relative)).Stream);
			return new WriteableBitmap(bi);
		}
	}
}

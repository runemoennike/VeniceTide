using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Shell;

namespace VeniceTide.Code
{
	class LiveTileBuilderFull : VeniceTideCommon.Code.LiveTileBuilderLimited
	{
		public static void CreateNew()
		{
			BuildBitmap();

			var data = new StandardTileData
			{
				BackgroundImage = new Uri("isostore:" + LIVETILE_FILENAME, UriKind.Absolute),
			};

			ShellTile.ActiveTiles.Where(x => x.NavigationUri.ToString().Contains("islivetile")).ToList().ForEach(x => x.Delete());

			try
			{
				ShellTile.Create(new Uri("/Pages/MainPage.xaml?islivetile", UriKind.Relative), data);
			}
			catch (InvalidOperationException ex)
			{
				// Occurs if the user spam-clicks the create button. Ignore.
			}
		}
	}
}

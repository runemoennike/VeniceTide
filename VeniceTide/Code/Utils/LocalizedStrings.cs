using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VeniceTideCommon.Resources.Localization;
using VeniceTideCommon.Code;

namespace VeniceTide.Code.Utils
{
	public class LocalizedStrings
	{
		public LocalizedStrings()
		{
		}

		private static AppResources localizedResources = AppResourcesFactory.Construct();

		public AppResources AppResources
		{
			get { return localizedResources; }
		}
	}
}

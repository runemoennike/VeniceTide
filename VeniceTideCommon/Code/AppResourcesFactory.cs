using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VeniceTideCommon.Resources.Localization;

namespace VeniceTideCommon.Code
{
	public class AppResourcesFactory
	{
		public static AppResources Construct()
		{
			return new AppResources();
		}
	}
}

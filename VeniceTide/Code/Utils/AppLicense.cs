using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Marketplace;

namespace VeniceTide.Code.Utils
{
	public class AppLicense
	{
		public bool IsInTrialMode
		{
			get
			{
#if TRIAL
                return true;
#else
				LicenseInformation license = new LicenseInformation();
				return license.IsTrial();
#endif
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace VeniceTide
{
	public partial class SelectLocations : PhoneApplicationPage
	{
		public SelectLocations()
		{
			InitializeComponent();
			DataContext = App.ViewModel;
		}
	}
}
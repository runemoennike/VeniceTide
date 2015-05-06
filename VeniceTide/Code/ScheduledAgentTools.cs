using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Phone.Scheduler;

namespace VeniceTideAgent.Code
{
	public class ScheduledAgentTools
	{
		private const string AGENT_NAME = "VeniceTideAgent";
		private const string AGENT_DESCRIPTION = "Updates the Venice Tide live tile with the latest tide level predictions.";


		public static void Install()
		{			
			Remove();

			PeriodicTask periodicTask = new PeriodicTask(AGENT_NAME)
			{
				Description = AGENT_DESCRIPTION,
				//ExpirationTime = DateTime.Now.AddDays(14),
			};

			try
			{
				ScheduledActionService.Add(periodicTask);

#if(DEBUG)
				ScheduledActionService.LaunchForTest(AGENT_NAME, TimeSpan.FromSeconds(10));
#endif
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.Contains("BNS Error: The action is disabled"))
				{
					throw new InvalidOperationException("UserDisabled", ex);
				}
			}
			catch (SchedulerServiceException)
			{
				// No user action required.  
			}
		}

		public static void Remove()
		{
			try
			{
				PeriodicTask periodicTask = ScheduledActionService.Find(AGENT_NAME) as PeriodicTask;

				if (periodicTask != null)
				{
					ScheduledActionService.Remove(AGENT_NAME);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}

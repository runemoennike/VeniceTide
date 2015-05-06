using System;
using System.Threading;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using VeniceTideCommon.Code;
using VeniceTideCommon.Code.Utils;
using VeniceTideCommon.Code.ViewModels;

namespace VeniceTideAgent.Code
{
	public class ScheduledAgent : ScheduledTaskAgent
	{
		private static volatile bool _classInitialized;

		/// <remarks>
		/// ScheduledAgent constructor, initializes the UnhandledException handler
		/// </remarks>
		public ScheduledAgent()
		{
			if (!_classInitialized)
			{
				_classInitialized = true;
				// Subscribe to the managed exception handler
				Deployment.Current.Dispatcher.BeginInvoke(delegate
				{
					Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
				});
			}
		}

		/// Code to execute on Unhandled Exceptions
		private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		/// <summary>
		/// Agent that runs a scheduled task
		/// </summary>
		/// <param name="task">
		/// The invoked task
		/// </param>
		/// <remarks>
		/// This method is called when a periodic or resource intensive task is invoked
		/// </remarks>
		protected override void OnInvoke(ScheduledTask task)
		{

#if DEBUG
			ShellToast toast = new ShellToast();
			toast.Title = "VeniceTide";
			toast.Content = "Live tile update agent running.";
			toast.Show();

			ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
#endif

			MainViewModel viewModel = new MainViewModel();
			MainViewModel.CurrentMainViewModel = viewModel;

			viewModel.LoadLocalData(false);
			viewModel.DownloadData();

			if (viewModel.DownloadOk)
			{
				viewModel.UpdateFromRawData(false);
				viewModel.LastUpdated = DateTime.Now;

				Thread.MemoryBarrier();

				EventWaitHandle opWaitHandle = new AutoResetEvent(false);
				bool opSuccess = true;

				var op = Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					opSuccess = LiveTileBuilderLimited.Update();

					opWaitHandle.Set();
				});

				opWaitHandle.WaitOne();

				if (!opSuccess)
				{
					Abort();
				}
			}

			NotifyComplete();
		}
	}
}
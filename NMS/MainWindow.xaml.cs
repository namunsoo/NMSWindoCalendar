using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using WinRT.Interop;
using WinUIEx;

using NMS.Helpers;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NMS
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
	{
		private AppWindow m_AppWindow;

		public MainWindow()
		{
			this.InitializeComponent();
			//var manager = WinUIEx.WindowManager.Get(this);
			//manager.MinWidth = 400;
			//manager.MinHeight = 400;

			m_AppWindow = GetAppWindowForCurrentWindow();
			//var titleBar = m_AppWindow.TitleBar;
			//// Hide system title bar.
			//// titleBar.ExtendsContentIntoTitleBar = true;

			var presenter = m_AppWindow.Presenter as OverlappedPresenter;
			presenter.IsMaximizable = false;
			presenter.IsMinimizable = false;
			presenter.IsAlwaysOnTop = false;
			presenter.IsResizable = false;
			presenter.SetBorderAndTitleBar(false, false);

			Helpers.Window.hWnd = Helpers.Window.GetHWnd(this);
			Helpers.Window.MakeTransparent();

			Helpers.Window.App.TitleBar.ExtendsContentIntoTitleBar = true;
			Helpers.Window.App.TitleBar.ButtonBackgroundColor = Colors.Transparent;
			Helpers.Window.App.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
			Helpers.Window.App.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(50, 255, 255, 255);
			Helpers.Window.App.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(90, 255, 255, 255);

			//this.InitializeComponent();
			Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(
				Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
				new Microsoft.UI.Dispatching.DispatcherQueueHandler(() =>
				{
					Helpers.Window.SetMica(true, false);
					Helpers.Window.SetAcrylic(false, false);
					Main.Background = new SolidColorBrush(Colors.Transparent);
					Helpers.Window.SetBlur(true, false);
					PopulateProjects();
				}));
		}

		private AppWindow GetAppWindowForCurrentWindow()
		{
			IntPtr hWnd = WindowNative.GetWindowHandle(this);
			WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
			return AppWindow.GetFromWindowId(wndId);
		}

		private void PopulateProjects()
		{
			DateTime startDate = DateTime.Now;
			List<Project> Projects = new List<Project>();

			Project newProject = new Project();
			newProject.Name = "Project 1";
			newProject.Activities.Add(new Activity()
			{ Name = "Activity 1", Complete = true, DueDate = startDate.AddDays(4) });
			newProject.Activities.Add(new Activity()
			{ Name = "Activity 2", Complete = true, DueDate = startDate.AddDays(5) });
			Projects.Add(newProject);

			newProject = new Project();
			newProject.Name = "Project 2";
			newProject.Activities.Add(new Activity()
			{ Name = "Activity A", Complete = true, DueDate = startDate.AddDays(2) });
			newProject.Activities.Add(new Activity()
			{ Name = "Activity B", Complete = false, DueDate = startDate.AddDays(3) });
			Projects.Add(newProject);

			newProject = new Project();
			newProject.Name = "Project 3";
			Projects.Add(newProject);

			StyledGrid.ItemsSource = Projects;
		}
	}

	public class Project
	{
		public Project()
		{
			Activities = new ObservableCollection<Activity>();
		}

		public string Name { get; set; }
		public ObservableCollection<Activity> Activities { get; private set; }
	}

	public class Activity
	{
		public string Name { get; set; }
		public DateTime DueDate { get; set; }
		public bool Complete { get; set; }
		public string Project { get; set; }
	}
}

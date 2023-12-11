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
using NMS.Page;

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
			var manager = WinUIEx.WindowManager.Get(this);
			manager.MinWidth = 800;
			manager.MinHeight = 750;
			manager.Width = 800;
			manager.Height = 750;
			////manager.IsResizable = false;
			//manager.IsTitleBarVisible = true;
			//manager.IsMaximizable = false;
			//manager.IsMinimizable = false;
			//manager.IsTitleBarVisible=false;

			IntPtr hWnd = WindowNative.GetWindowHandle(this);
			WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
			m_AppWindow = AppWindow.GetFromWindowId(wndId);
			//var titleBar = m_AppWindow.TitleBar;
			//// Hide system title bar.
			//titleBar.ExtendsContentIntoTitleBar = true;
			if (m_AppWindow is not null)
			{
				Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(wndId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
				if (displayArea is not null)
				{
					var CenteredPosition = m_AppWindow.Position;
					CenteredPosition.X = ((displayArea.WorkArea.Width - m_AppWindow.Size.Width) / 2);
					//CenteredPosition.Y = ((displayArea.WorkArea.Height - m_AppWindow.Size.Height) / 2);
					CenteredPosition.Y = 0;
					m_AppWindow.Move(CenteredPosition);
				}
			}
			//m_AppWindow.Title = "Title";
			//m_AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
			//m_AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

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
				}));
			PageFrame.Navigate(typeof(CalendarMonth));
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			App.m_window.Close();
		}
	}
}

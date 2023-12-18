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
using Microsoft.UI.Xaml.Shapes;

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

			IntPtr hWnd = WindowNative.GetWindowHandle(this);
			WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
			m_AppWindow = AppWindow.GetFromWindowId(wndId);
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

			Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(
				Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
				new Microsoft.UI.Dispatching.DispatcherQueueHandler(() =>
				{
					Helpers.Window.SetMica(true, false);
					Helpers.Window.SetAcrylic(false, false);
					Main.Background = new SolidColorBrush(Colors.Transparent);
					//Helpers.Window.SetBlur(true, false);
					GetMyOptions();
				}));
			PageFrame.Navigate(typeof(CalendarMonth));
		}

		private void GetMyOptions()
		{
			var manager = WinUIEx.WindowManager.Get(this);
			manager.MinWidth = 800;
			manager.MinHeight = 750;
			manager.Width = 800;
			manager.Height = 750;

			IntPtr hWnd = WindowNative.GetWindowHandle(this);
			WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
			m_AppWindow = AppWindow.GetFromWindowId(wndId);
			DisplayArea displayArea = null;
			if (m_AppWindow is not null)
			{
				displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Nearest);
			}

			FileInfo fi = new FileInfo("C:\\MyCalendarAssets\\myOptions.txt");
			if (fi.Exists)
			{
				using (StreamReader reader = new StreamReader("C:\\MyCalendarAssets\\myOptions.txt"))
				{
					string[] data = reader.ReadToEnd().Split(',');

					TogSw_Blur.IsOn = data[0].Equals("blur_true");
					Main.Background = new SolidColorBrush(Common.GetColor(data[1]));
					manager.MinWidth = Convert.ToInt32(data[2]);
					manager.MinHeight = Convert.ToInt32(data[3]);
					manager.Width = Convert.ToInt32(data[2]);
					manager.Height = Convert.ToInt32(data[3]);
					if (displayArea is not null)
					{
						var position = m_AppWindow.Position;
						position.X = Convert.ToInt32(data[4]);
						position.Y = Convert.ToInt32(data[5]);
						m_AppWindow.Move(position);
					}
				}
			}
			else
			{
				manager.MinWidth = 800;
				manager.MinHeight = 750;
				manager.Width = 800;
				manager.Height = 750;
				if (displayArea is not null)
				{
					var position = m_AppWindow.Position;
					position.X = ((displayArea.WorkArea.Width - m_AppWindow.Size.Width) / 2);
					//CenteredPosition.Y = ((displayArea.WorkArea.Height - m_AppWindow.Size.Height) / 2);
					position.Y = 20;
					m_AppWindow.Move(position);
				}
				TogSw_Blur.IsOn = true;
				using (StreamWriter writer = new StreamWriter("C:\\MyCalendarAssets\\myOptions.txt"))
				{
					writer.Write("blur_true,#00000000,800,750,");
					writer.Write(displayArea == null ? "0,0" : ((displayArea.WorkArea.Width - m_AppWindow.Size.Width) / 2) + ",20");
				}
			}
			NBox_OpenPointX.Maximum = (displayArea.WorkArea.Width - m_AppWindow.Size.Width);
			NBox_OpenPointY.Maximum = (displayArea.WorkArea.Height - m_AppWindow.Size.Height);
		}

		private void TogSw_Blur_Toggled(object sender, RoutedEventArgs e)
		{
			ToggleSwitch toggleSwitch = sender as ToggleSwitch;
			if (toggleSwitch != null)
			{
				Helpers.Window.hWnd = Helpers.Window.GetHWnd(this);
				if (toggleSwitch.IsOn == true)
				{
					Helpers.Window.SetBlur(true, false);
				}
				else
				{
					Helpers.Window.SetBlur(false, false);
				}
			}
		}

		private void CP_BackgroundColor_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
		{
			Main.Background = new SolidColorBrush(sender.Color);
		}

		private void Btn_OptionSave_Click(object sender, RoutedEventArgs e)
		{
			FileInfo fi = new FileInfo("C:\\MyCalendarAssets\\myOptions.txt");
			string options = string.Empty;
			options += TogSw_Blur.IsOn ? "blur_true," : "blur_false,";
			options += CP_BackgroundColor.Color.ToString() + ",";
			options += NBox_WindowWith.Value + ",";
			options += NBox_WindowHeight.Value + ",";
			options += NBox_OpenPointX.Value + ",";
			options += NBox_OpenPointY.Value;
			using (StreamWriter writer = new StreamWriter("C:\\MyCalendarAssets\\myOptions.txt"))
			{
				writer.Write(options);
			}
		}

		private void SizeAndPoint_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
		{
			if (NBox_WindowWith.IsLoaded && NBox_WindowHeight.IsLoaded && NBox_OpenPointX.IsLoaded && NBox_OpenPointY.IsLoaded)
			{
				var manager = WinUIEx.WindowManager.Get(this);
				manager.MinWidth = NBox_WindowWith.Value;
				manager.MinHeight = NBox_WindowHeight.Value;
				manager.Width = NBox_WindowWith.Value;
				manager.Height = NBox_WindowHeight.Value;

				var position = m_AppWindow.Position;
				position.X = Convert.ToInt32(NBox_OpenPointX.Value);
				position.Y = Convert.ToInt32(NBox_OpenPointY.Value);
				m_AppWindow.Move(position);
			}
		}

		private void Btn_Close_Click(object sender, RoutedEventArgs e)
		{
			App.m_window.Close();
		}
	}
}

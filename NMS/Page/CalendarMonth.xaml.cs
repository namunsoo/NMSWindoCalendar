using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Collections.ObjectModel;
using NMS.Helpers;
using static System.Net.Mime.MediaTypeNames;
using Windows.System;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NMS.Page
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CalendarMonth
	{
		private string headerBorderThickness { get; set; } = "0,0,0,2";
		private Brush headerBorderBrush { get; set; } = new SolidColorBrush(Common.GetColor("#1A000000"));
		private Grid _grid;
		private GridViewItem _clickedItem;
		private Button _buttonDetail;
		private List<CalendarDay> _dayList;
		public CalendarMonth()
		{
			this.InitializeComponent();

			// ������ �Է�
			DataBinding();

			// GridView �ʱⰪ ������
			CalendarGridView.SelectedIndex = -1;
		}

		#region [| ������ �Է� |]
		private void DataBinding()
		{
			_dayList = CalendarCore.GetMonth(DateTime.Today);
			List<Item> items = new List<Item>();
			Item item = null;
			CustomDay customDay = null;
			int startDay = CalendarCore.FristWeekStart(DateTime.Today);
			int thisMonthDays = CalendarCore.DaysByMonth(DateTime.Today.Year)[DateTime.Today.Month - 1];
			for (int i = 0; i < _dayList.Count; i++)
			{
				item = new Item();
				customDay = new CustomDay();
				customDay.Year = _dayList[i].Year;
				customDay.Month = _dayList[i].Month;
				customDay.Day = _dayList[i].Day;
				customDay.Memo = _dayList[i].Memo;
				customDay.LocalMemo = _dayList[i].LocalMemo;
				if (i < startDay)
				{
					customDay.NumberColor = new SolidColorBrush(Common.GetColor("#FF7a808b"));
					customDay.FontColor = new SolidColorBrush(Common.GetColor("#FF7a808b"));
				} else if (i < (startDay + thisMonthDays))
				{
					customDay.NumberColor = GetColorText(i);
					customDay.FontColor = new SolidColorBrush(Common.GetColor("#FF000f1a"));
				} else
				{
					customDay.NumberColor = new SolidColorBrush(Common.GetColor("#FF7a808b"));
					customDay.FontColor = new SolidColorBrush(Common.GetColor("#FF7a808b"));
				}
				item.Connection.Add(customDay);
				items.Add(item);
			}
			MonthData.Source = items;
		}
		#endregion

		#region [| Grid View ������ ���� �̺�Ʈ |]
		private void OnGridViewSizeChanged(object sender, SizeChangedEventArgs e)
		{
			((ItemsWrapGrid)CalendarGridView.ItemsPanelRoot).ItemWidth = e.NewSize.Width / 7;
			((ItemsWrapGrid)CalendarGridView.ItemsPanelRoot).ItemHeight = (e.NewSize.Height - 40) / 6;
		}
		#endregion

		#region [| Grid View ���� �̺�Ʈ |]
		private Grid beforeSelectedGrid = null;
		private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GridView gridView = sender as GridView;
			_clickedItem = (GridViewItem)CalendarGridView.ContainerFromItem(gridView.SelectedItem);
			if (beforeSelectedGrid != null)
			{
				TextBox beforeTextBox = beforeSelectedGrid.Children.OfType<TextBox>().LastOrDefault();
				beforeTextBox.Visibility = Visibility.Collapsed;
				Button buttonSave = beforeSelectedGrid.Children.OfType<Button>().LastOrDefault();
				buttonSave.Visibility = Visibility.Collapsed;
			}
			if (_clickedItem != null)
			{
				Grid grid = _clickedItem.ContentTemplateRoot as Grid;
				TextBox textBox = grid.Children.OfType<TextBox>().LastOrDefault();
				Button buttonSave = _grid.Children.OfType<Button>().LastOrDefault();
				textBox.Visibility = Visibility.Visible;
				buttonSave.Visibility = Visibility.Visible;
				textBox.Focus(FocusState.Programmatic);
				beforeSelectedGrid = grid;
			}
		}
		#endregion

		#region [| ��¥ mouse over ȿ�� |]
		private void GridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
		{
			_grid = sender as Grid;
			_buttonDetail = _grid.Children.OfType<Button>().FirstOrDefault();
			_buttonDetail.Visibility = Visibility.Visible;
			_grid.Background = new SolidColorBrush(Common.GetColor("#4DF3F3F3"));
		}

		private void GridViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
		{
			_grid = sender as Grid;
			_buttonDetail = _grid.Children.OfType<Button>().FirstOrDefault();
			_buttonDetail.Visibility = Visibility.Collapsed;
			_grid.Background = new SolidColorBrush(Common.GetColor("#00000000"));
		}
		#endregion

		#region [| �޸� ���� |]
		private async void BtnMemoSave_Click(object sender, RoutedEventArgs e)
		{
			Button buttonMemoSave = sender as Button;
			CustomDay customDay = buttonMemoSave.Tag as CustomDay;
			Grid grid = buttonMemoSave.Parent as Grid;
			TextBox localMemo = grid.Children.OfType<TextBox>().LastOrDefault();
			try
			{
				if (localMemo != null && customDay != null && !string.IsNullOrEmpty(localMemo.Text))
				{
					// ���� ����
					string path = "C:\\MyCalendarAssets\\localMemo\\" + customDay.Year + "\\" + customDay.Month;
					DirectoryInfo di = new DirectoryInfo(path);
					if (di.Exists == false)
					{
						di.Create();
					}
					using (StreamWriter writer = new StreamWriter(path + "\\" + customDay.Day + ".txt"))
					{
						writer.Write(localMemo.Text);
					}

					// ������ �޸�� ����
					TextBlock textBlock = grid.Children.OfType<TextBlock>().LastOrDefault();
					CalendarDay calendarDay = _dayList.Find(item => item.Month.Equals(customDay.Month) && item.Day.Equals(customDay.Day));
					if (calendarDay.Timetable != null && calendarDay.Timetable.Count != 0)
					{
						textBlock.Text = string.Empty;
						foreach (CalendarTimetableItem calendarTimetableItem in calendarDay.Timetable)
						{
							textBlock.Text += calendarTimetableItem.Summary + "\r";
						}
					}
					textBlock.Text += localMemo.Text;

					// ��Ŀ�� ����
					CalendarGridView.SelectedIndex = -1;
					localMemo.Visibility = Visibility.Collapsed;
					Button buttonSave = grid.Children.OfType<Button>().LastOrDefault();
					buttonSave.Visibility = Visibility.Collapsed;
				}
			}
			catch
			{
				ContentDialog errorDataDialog = new ContentDialog
				{
					Title = "����",
					Content = "�޸� �����ϴ� ���� �߻�",
					CloseButtonText = "Ȯ��"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				errorDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await errorDataDialog.ShowAsync();
			}
		}
		#endregion

		#region [| �޸� ����Ű ���� |]
		private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Left || e.Key == VirtualKey.Right || e.Key == VirtualKey.Up || e.Key == VirtualKey.Down)
			{
				e.Handled = true;
			}
		}
		#endregion

		#region [| �� ���� ���� |]
		private async void BtnOpenDetail_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog detailDataDialog = new ContentDialog
			{
				Title = "�󼼺���",
				Content = "�޸� �����ϴ� ���� �߻�",
				CloseButtonText = "Ȯ��"
			};

			// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
			detailDataDialog.XamlRoot = this.MyPanel.XamlRoot;

			ContentDialogResult result = await detailDataDialog.ShowAsync();
		}
		#endregion

		private SolidColorBrush GetColorText(int count)
		{
			int num = count % 7;
			switch (num)
			{
				case 0:
					return new SolidColorBrush(Common.GetColor("#FFDB4455"));
				case 6:
					return new SolidColorBrush(Common.GetColor("#FFA4C4EE"));
				default:
					return new SolidColorBrush(Common.GetColor("#FF000f1a"));
			}
		}
	}

	public class Item
	{
		public Item()
		{
			Connection = new ObservableCollection<CustomDay>();
		}
		public ObservableCollection<CustomDay> Connection { get; private set; }
	}

	public class CustomDay
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public string Memo { get; set; }
		public string LocalMemo { get; set; }
		public Brush NumberColor { get; set; }
		public Brush FontColor { get; set; }
	}
}

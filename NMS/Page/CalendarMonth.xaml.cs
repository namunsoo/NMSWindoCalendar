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
		public CalendarMonth()
        {
            this.InitializeComponent();

			// 데이터 입력
			DataBinding();

			// GridView 초기값 삭제용
			CalendarGridView.SelectedIndex = -1;
		}

		#region [| 데이터 입력 |]
		private void DataBinding()
		{
			List<CalendarDay> targetMonth = CalendarCore.GetMonth(DateTime.Today);
			List<CalendarDay> beforeMonth = CalendarCore.GetMonth(DateTime.Today.AddMonths(-1));
			List<CalendarDay> nextMonth = CalendarCore.GetMonth(DateTime.Today.AddMonths(1));
			List<Item> items = new List<Item>();
			Item item = null;
			CustomDay customDay = null;
			int i = 0;
			int count = 0;
			int startDay = CalendarCore.FristWeekStart(DateTime.Today);
			#region [| 이전달(일부) |]
			for (i = startDay; i > 0; i--)
			{
				item = new Item();
				customDay = new CustomDay();
				customDay.Year = beforeMonth[beforeMonth.Count - i].Year;
				customDay.Month = beforeMonth[beforeMonth.Count - i].Month;
				customDay.Day = beforeMonth[beforeMonth.Count - i].Day;
				customDay.Memo = beforeMonth[beforeMonth.Count - i].Memo;
				customDay.NumberColor = new SolidColorBrush(Common.GetColor("#FF7a808b"));
				customDay.FontColor = new SolidColorBrush(Common.GetColor("#FF7a808b"));
				item.Connection.Add(customDay);
				items.Add(item);
				count++;
			}
			#endregion
			#region [| 이번달 |]
			for (i = 0; i < targetMonth.Count; i++)
			{
				item = new Item();
				customDay = new CustomDay();
				customDay.Year = targetMonth[i].Year;
				customDay.Month = targetMonth[i].Month;
				customDay.Day = targetMonth[i].Day;
				customDay.Memo = targetMonth[i].Memo;
				customDay.NumberColor = GetColorText(count);
				customDay.FontColor = new SolidColorBrush(Common.GetColor("#FF000f1a"));
				item.Connection.Add(customDay);
				items.Add(item);
				count++;
			}
			#endregion
			#region [| 다음달(일부) |]
			int remainCount = 42 - (startDay + targetMonth.Count);
			for (i = 0; i < remainCount; i++)
			{
				item = new Item();
				customDay = new CustomDay();
				customDay.Year = nextMonth[i].Year;
				customDay.Month = nextMonth[i].Month;
				customDay.Day = nextMonth[i].Day;
				customDay.Memo = nextMonth[i].Memo;
				customDay.NumberColor = new SolidColorBrush(Common.GetColor("#FF7a808b"));
				customDay.FontColor = new SolidColorBrush(Common.GetColor("#FF7a808b"));
				item.Connection.Add(customDay);
				items.Add(item);
				count++;
			}
			#endregion
			MonthData.Source = items;
		}
		#endregion

		#region [| Grid View 사이즈 변경 이벤트 |]
		private void OnGridViewSizeChanged(object sender, SizeChangedEventArgs e)
		{
			((ItemsWrapGrid)CalendarGridView.ItemsPanelRoot).ItemWidth = e.NewSize.Width / 7;
			((ItemsWrapGrid)CalendarGridView.ItemsPanelRoot).ItemHeight = (e.NewSize.Height - 40) / 6;
		}
		#endregion

		#region [| Grid View 선택 이벤트 |]
		private Grid beforeSelectedGrid = null;
		private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GridView gridView = sender as GridView;
			_clickedItem = (GridViewItem)CalendarGridView.ContainerFromItem(gridView.SelectedItem);
            if (beforeSelectedGrid != null)
            {
				TextBox beforeTextBox = beforeSelectedGrid.Children.OfType<TextBox>().LastOrDefault();
				beforeTextBox.Visibility = Visibility.Collapsed;
			}
            if (_clickedItem != null)
			{
				Grid grid = _clickedItem.ContentTemplateRoot as Grid;
				TextBox textBox = grid.Children.OfType<TextBox>().LastOrDefault();
				textBox.Visibility = Visibility.Visible;
				textBox.Focus(FocusState.Programmatic);
				beforeSelectedGrid = grid;
			}
		}
		#endregion

		#region [| 날짜 mouse over 효과 |]
		private void GridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
		{
			_grid = sender as Grid;
			Button button = _grid.Children.OfType<Button>().FirstOrDefault();
			button.Visibility = Visibility.Visible;
			_grid.Background = new SolidColorBrush(Common.GetColor("#4DF3F3F3"));
		}

		private void GridViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
		{
			_grid = sender as Grid;
			Button button = _grid.Children.OfType<Button>().FirstOrDefault();
			button.Visibility = Visibility.Collapsed;
			_grid.Background = new SolidColorBrush(Common.GetColor("#00000000"));
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
		public Brush NumberColor { get; set; }
		public Brush FontColor { get; set; }
	}
}

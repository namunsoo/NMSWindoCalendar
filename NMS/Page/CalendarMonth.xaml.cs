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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NMS.Page
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarMonth
    {
		public string headerBorderThickness { get; set; } = "0,0,0,2";
		public Brush headerBorderBrush { get; set; } = new SolidColorBrush(Common.GetColor("#1A000000"));
		public CalendarMonth()
        {
            this.InitializeComponent();

			DataBinding();
		}

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

		private void onGridViewSizeChanged(object sender, SizeChangedEventArgs e)
		{
			((ItemsWrapGrid)gridView.ItemsPanelRoot).ItemWidth = e.NewSize.Width / 7;
			((ItemsWrapGrid)gridView.ItemsPanelRoot).ItemHeight = (e.NewSize.Height - 40) / 6;
		}

		private void GridViewItem_Click(object sender, ItemClickEventArgs e)
		{

		}

		private void GridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
		{
			Grid Item = sender as Grid;
			Item.Background = new SolidColorBrush(Common.GetColor("#4DF3F3F3"));
		}

		private void GridViewItem_PointerExited(object sender, PointerRoutedEventArgs e)
		{
			Grid Item = sender as Grid;
			Item.Background = new SolidColorBrush(Common.GetColor("#00000000"));
		}

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

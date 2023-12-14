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
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NMS.Page
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CalendarMonth
	{
		private string TodayString { get; set; } = DateTime.Today.ToString("yyyy년 MM월") +" / " + DateTime.Today.ToString("오늘은 yyyy-MM-dd");
		private Grid _grid;
		private GridViewItem _clickedItem;
		private Button _buttonDetail;
		private List<CalendarDay> _dayList;
		private DateTimeOffset _thisMonth = DateTimeOffset.Now;
		private CalendarDay _tempForDetailEvent_calendarDay;
		private CustomDay _tempForDetailEvent_customDay;
		private Grid _tempForDetailEvent_targetGrid;
		public CalendarMonth()
		{
			this.InitializeComponent();

			// 데이터 입력
			DataBinding(DateTime.Today);

			// GridView 초기값 삭제용
			//CalendarGridView.SelectedIndex = -1;
		}

		#region [| 데이터 입력 |]
		private async void DataBinding(DateTime date)
		{
			await Task.Run(() =>
			{
				_dayList = CalendarCore.GetMonth(date);
			});
			List<Item> items = new List<Item>();
			Item item = null;
			CustomDay customDay = null;
			int startDay = CalendarCore.FristWeekStart(date);
			int thisMonthDays = CalendarCore.DaysByMonth(date.Year)[date.Month - 1];
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

		#region [| 날짜 mouse over 효과 |]
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

		#region [| 메모 저장 버튼 클릭 |]
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
					// 파일 저장
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

					// 저장한 메모로 변경
					TextBlock textBlock = grid.Children.OfType<TextBlock>().LastOrDefault();
					CalendarDay calendarDay = _dayList.Find(item => item.Month.Equals(customDay.Month) && item.Day.Equals(customDay.Day));
					textBlock.Text = string.Empty;
					if (calendarDay.Timetable != null && calendarDay.Timetable.Count != 0)
					{
						foreach (CalendarTimetableItem calendarTimetableItem in calendarDay.Timetable)
						{
							textBlock.Text += calendarTimetableItem.Summary + "\r";
						}
					}
					textBlock.Text += localMemo.Text;

					// 포커스 변경
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
					Title = "오류",
					Content = "메모를 저장하다 오류 발생",
					CloseButtonText = "확인"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				errorDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await errorDataDialog.ShowAsync();
			}
		}
		#endregion

		#region [| 메모 방향키 제한 |]
		private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Left || e.Key == VirtualKey.Right || e.Key == VirtualKey.Up || e.Key == VirtualKey.Down)
			{
				e.Handled = true;
			}
		}
		#endregion

		#region [| 상세 보기 버튼 클릭 |]
		private void BtnOpenDetail_Click(object sender, RoutedEventArgs e)
		{
			DayDetailGrid.Visibility = Visibility.Visible;
			Button buttonOpenDetail = sender as Button;
			CustomDay customDay = buttonOpenDetail.Tag as CustomDay;
			CalendarDay calendarDay = _dayList.Find(item => item.Month.Equals(customDay.Month) && item.Day.Equals(customDay.Day));
			List<CustomTimetableItem> dataList = new List<CustomTimetableItem>();
			CustomTimetableItem data = null;
			string preview = string.Empty;
			foreach (CalendarTimetableItem item in calendarDay.Timetable)
			{
				data = new CustomTimetableItem();
				preview = string.Empty;
				data.Year = item.Year;
				data.Month = item.Month;
				data.Day = item.Day;
				data.StartTime = item.StartTime;
				data.EndTime = item.EndTime;
				preview += item.StartTime != null ? ((DateTime)item.StartTime).ToString("hh:mm") : "??:??";
				preview += " ~ ";
				preview += item.EndTime != null ? ((DateTime)item.EndTime).ToString("hh:mm") : "??:??";
				data.StarEndPreview = preview;
				data.Description = item.Description;
				data.Summary = item.Summary;
				dataList.Add(data);
			}
			ScheduleList.ItemsSource = dataList;
			MyLocalMemo.Text = calendarDay.LocalMemo;

			// 상세보기 작업용 임시 데이터
			_tempForDetailEvent_calendarDay = calendarDay;
			_tempForDetailEvent_targetGrid = buttonOpenDetail.Parent as Grid;
			_tempForDetailEvent_customDay = buttonOpenDetail.Tag as CustomDay;

			// 포커스 변경
			TextBox localMemo = _tempForDetailEvent_targetGrid.Children.OfType<TextBox>().LastOrDefault();
			Button buttonSave = _tempForDetailEvent_targetGrid.Children.OfType<Button>().LastOrDefault();
			localMemo.Visibility = Visibility.Collapsed;
			buttonSave.Visibility = Visibility.Collapsed;
			CalendarGridView.SelectedIndex = -1;
		}
		#endregion

		#region [| 이전달 버튼 클릭 |]
		private void BtnBeforeMonth_Click(object sender, RoutedEventArgs e)
		{
			DateTimeOffset date = CalendarDatePicker.Date != null ? (DateTimeOffset)CalendarDatePicker.Date : DateTimeOffset.Now;
			CalendarDatePicker.Date = date.AddMonths(-1);
		}
		#endregion

		#region [| 다음달 버튼 클릭 |]
		private void BtnNextMonth_Click(object sender, RoutedEventArgs e)
		{
			DateTimeOffset date = CalendarDatePicker.Date != null ? (DateTimeOffset)CalendarDatePicker.Date : DateTimeOffset.Now;
			CalendarDatePicker.Date = date.AddMonths(1);
		}
		#endregion

		#region [| CalendarDatePicker 날짜 변경 이벤트 |]
		private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
		{
			DateTimeOffset date = sender.Date != null ? (DateTimeOffset)CalendarDatePicker.Date : DateTimeOffset.Now;
			if (_thisMonth.Month != date.Month || _thisMonth.Year != date.Year)
			{
				_thisMonth = date;
				DateTime dateTime = new DateTime(date.Year, date.Month, date.Day);
				DataBinding(dateTime);
			}
		}
		#endregion

		#region [| 상세보기 여백 클릭시 |]
		private void DayDetailGrid_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (e.OriginalSource == sender)
			{
				DayDetailGrid.Visibility = Visibility.Collapsed;
			}
		}
		#endregion

		private void ScheduleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CustomTimetableItem item = ScheduleList.SelectedItem as CustomTimetableItem;
			if (item != null)
			{
				GoogleSheduleDelete.Visibility = Visibility.Visible;
				GoogleSheduleAddOrRetouch_Text.Text = "일정 수정";
				GoogleSheduleAddOrRetouch_Icon.Symbol = Symbol.Repair;
				GoogleSheduleSummary.Text = item.Summary;
				GoogleSheduleDescription.Text = item.Description;
				if (item.StartTime != null && item.EndTime != null)
				{
					DateTime startDt = (DateTime)item.StartTime;
					DateTime endDt = (DateTime)item.EndTime;
					GoogleSheduleStartTime.SelectedTime = new TimeSpan(startDt.Hour,startDt.Minute,0);
					GoogleSheduleEndTime.SelectedTime = new TimeSpan(endDt.Hour, endDt.Minute, 0);
				}
				else
				{
					GoogleSheduleStartTime.SelectedTime = new TimeSpan();
					GoogleSheduleEndTime.SelectedTime = new TimeSpan();
				}
			}
			else
			{
				GoogleSheduleDelete.Visibility = Visibility.Collapsed;
				GoogleSheduleAddOrRetouch_Text.Text = "일정 추가";
				GoogleSheduleAddOrRetouch_Icon.Symbol = Symbol.Add;
			}
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
		public string LocalMemo { get; set; }
		public Brush NumberColor { get; set; }
		public Brush FontColor { get; set; }
	}

	public class CustomTimetableItem
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public string StarEndPreview { get; set; }
		public string Description { get; set; }
		public string Summary { get; set; }
	}
}

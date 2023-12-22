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
using System.Collections;
using WinUIEx.Messaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NMS.Page
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CalendarMonth
	{
		private string TodayString { get; set; } = DateTime.Today.ToString("yyyy�� MM��") +" / " + DateTime.Today.ToString("������ yyyy-MM-dd");
		private Grid _grid;
		private GridViewItem _clickedItem;
		private Button _buttonDetail;
		private DateTimeOffset _thisMonth = DateTimeOffset.Now;
		private CustomDay _tempForDetailEvent_customDay;
		private Grid _tempForDetailEvent_targetGrid;
		private List<CustomTimetableItem> _tempListViewData;
		private List<Item> _calendarGridViewSource;
		public CalendarMonth()
		{
			this.InitializeComponent();

			// ������ �Է�
			DataBinding(DateTime.Today);

			// �ý��� �޼��� �ʱ� ����
			// ó������ fade out �����ϵ���
			// �����ϴ� ����� ��ã�Ƽ� �ӽ� ����
			MessageClose.Begin();
		}

		#region [| ������ �Է� |]
		private async void DataBinding(DateTime date)
		{
			try { 
				List<CalendarDay> dayList = new List<CalendarDay>();
				await Task.Run(() =>
				{
					dayList = CalendarCore.GetMonth(date);
				});
				List<Item> items = new List<Item>();
				Item item = null;
				CustomDay customDay = null;
				int startDay = CalendarCore.FristWeekStart(date);
				int thisMonthDays = CalendarCore.DaysByMonth(date.Year)[date.Month - 1];
				for (int i = 0; i < dayList.Count; i++)
				{
					item = new Item();
					customDay = new CustomDay();
					customDay.Year = dayList[i].Year;
					customDay.Month = dayList[i].Month;
					customDay.Day = dayList[i].Day;
					customDay.Memo = dayList[i].Memo;
					customDay.LocalMemo = dayList[i].LocalMemo;
					customDay.Timetable = ToCustomTimetableItem(dayList[i].Timetable);
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
				_calendarGridViewSource = items;

				// gridview �⺻���� ���ֱ�
				CalendarGridView.SelectedIndex = -1;
			}
			catch {
				ContentDialog errorDataDialog = new ContentDialog
				{
					Title = "����",
					Content = "�޷� ������ �������� ������ �߻��߽��ϴ�.",
					CloseButtonText = "Ȯ��"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				errorDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await errorDataDialog.ShowAsync();
			}
		}
		#endregion

		#region [| Grid View ������ ���� �̺�Ʈ |]
		private void OnGridViewSizeChanged(object sender, SizeChangedEventArgs e)
		{
			((ItemsWrapGrid)CalendarGridView.ItemsPanelRoot).ItemWidth = e.NewSize.Width / 7;
			((ItemsWrapGrid)CalendarGridView.ItemsPanelRoot).ItemHeight = (e.NewSize.Height - 31) / 6;
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
				Button buttonClose = beforeSelectedGrid.Children.OfType<Button>().ToList()[1];
				Button buttonSave = beforeSelectedGrid.Children.OfType<Button>().LastOrDefault();
				buttonClose.Visibility = Visibility.Collapsed;
				buttonSave.Visibility = Visibility.Collapsed;
			}
			if (_clickedItem != null)
			{
				Grid grid = _clickedItem.ContentTemplateRoot as Grid;
				TextBox textBox = grid.Children.OfType<TextBox>().LastOrDefault();
				Button buttonClose = _grid.Children.OfType<Button>().ToList()[1];
				Button buttonSave = _grid.Children.OfType<Button>().LastOrDefault();
				textBox.Visibility = Visibility.Visible;
				textBox.Focus(FocusState.Programmatic);
				buttonClose.Visibility = Visibility.Visible;
				buttonSave.Visibility = Visibility.Visible;
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

		#region [| �޸� �ݱ� ��ư |]
		private void Btn_MemoClose_Click(object sender, RoutedEventArgs e)
		{
			CalendarGridView.SelectedIndex = -1;
		}
		#endregion

		#region [| �޸� ���� ��ư Ŭ�� |]
		private async void Btn_MemoSave_Click(object sender, RoutedEventArgs e)
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

					// ������ �޸�� ������ ����
					string googleShedule = string.Empty;
					foreach (CustomTimetableItem calendarTimetableItem in customDay.Timetable)
					{
						googleShedule += calendarTimetableItem.Summary + "\r";
					}
					customDay.LocalMemo = localMemo.Text;
					customDay.Memo = googleShedule + localMemo.Text;
					_calendarGridViewSource[CalendarGridView.SelectedIndex].Connection.Add(customDay);
					_calendarGridViewSource[CalendarGridView.SelectedIndex].Connection.RemoveAt(0);
					MonthData.Source = _calendarGridViewSource;

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

		#region [| �� ���� ��ư Ŭ�� |]
		private void Btn_OpenDetail_Click(object sender, RoutedEventArgs e)
		{
			DayDetailGrid.Visibility = Visibility.Visible;
			Button buttonOpenDetail = sender as Button;
			CustomDay customDay = buttonOpenDetail.Tag as CustomDay;
			ScheduleList.ItemsSource = customDay.Timetable;
			Txtblk_MyLocalMemo.Text = customDay.LocalMemo;

			// �󼼺��� �۾��� �ӽ� ������
			_tempListViewData = customDay.Timetable;
			_tempForDetailEvent_targetGrid = buttonOpenDetail.Parent as Grid;
			_tempForDetailEvent_customDay = customDay;

			// ��Ŀ�� ����
			TextBox localMemo = _tempForDetailEvent_targetGrid.Children.OfType<TextBox>().LastOrDefault();
			Button buttonSave = _tempForDetailEvent_targetGrid.Children.OfType<Button>().LastOrDefault();
			localMemo.Visibility = Visibility.Collapsed;
			buttonSave.Visibility = Visibility.Collapsed;
			CalendarGridView.SelectedIndex = -1;
		}
		#endregion

		#region [| ������ ��ư Ŭ�� |]
		private void Btn_BeforeMonth_Click(object sender, RoutedEventArgs e)
		{
			DateTimeOffset date = CalendarDatePicker.Date != null ? (DateTimeOffset)CalendarDatePicker.Date : DateTimeOffset.Now;
			CalendarDatePicker.Date = date.AddMonths(-1);
		}
		#endregion

		#region [| ������ ��ư Ŭ�� |]
		private void Btn_NextMonth_Click(object sender, RoutedEventArgs e)
		{
			DateTimeOffset date = CalendarDatePicker.Date != null ? (DateTimeOffset)CalendarDatePicker.Date : DateTimeOffset.Now;
			CalendarDatePicker.Date = date.AddMonths(1);
		}
		#endregion

		#region [| CalendarDatePicker ��¥ ���� �̺�Ʈ |]
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



		#region [| ==�󼼺���== |]

		#region [| �󼼺��� ���� Ŭ���� |]
		private void DayDetailGrid_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (e.OriginalSource == sender)
			{
				DayDetailGrid.Visibility = Visibility.Collapsed;
			}
		}
		#endregion

		#region [| ���� ���� ListView ���� ���� �̺�Ʈ |]
		private void ScheduleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CustomTimetableItem item = ScheduleList.SelectedItem as CustomTimetableItem;
			if (item != null)
			{
				Btn_SheduleDelete.Visibility = Visibility.Visible;
				Btn_SheduleCancel.Visibility = Visibility.Visible;
				Txtblk_GoogleSheduleAddOrRetouch.Text = "����";
				SIcon_GoogleSheduleAddOrRetouch.Symbol = Symbol.Repair;
				Btn_SheduleAddOrRetouch.Tag = item;
				Btn_SheduleDelete.Tag = item;
				Txtbox_GoogleSheduleSummary.Text = item.Summary;
				Txtbox_GoogleSheduleDescription.Text = item.Description;
				if (item.StartTime != null && item.EndTime != null)
				{
					DateTime startDt = (DateTime)item.StartTime;
					DateTime endDt = (DateTime)item.EndTime;
					GoogleSheduleStartTime.SelectedTime = new TimeSpan(startDt.Hour, startDt.Minute, 0);
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
				Txtbox_GoogleSheduleSummary.Text = string.Empty;
				Txtbox_GoogleSheduleDescription.Text = string.Empty;
				GoogleSheduleStartTime.SelectedTime = null;
				GoogleSheduleEndTime.SelectedTime = null;
				Btn_SheduleDelete.Visibility = Visibility.Collapsed;
				Btn_SheduleCancel.Visibility = Visibility.Collapsed;
				Txtblk_GoogleSheduleAddOrRetouch.Text = "�߰�";
				SIcon_GoogleSheduleAddOrRetouch.Symbol = Symbol.Add;
				Btn_SheduleAddOrRetouch.Tag = null;
				Btn_SheduleDelete.Tag = null;
			}
		}
		#endregion

		#region [| �޸� ���� Ŭ�� |]
		private async void Btn_MyLocalMemoDelete_Click(object sender, RoutedEventArgs e)
		{
			CustomDay customDay = _tempForDetailEvent_targetGrid.Children.OfType<Button>().FirstOrDefault().Tag as CustomDay;
			int itemIndex = CalendarGridView.IndexFromContainer(CalendarGridView.ContainerFromItem(customDay));
			try
			{
				// dialog ����
				ContentDialog dialog = new ContentDialog
				{
					Title = "�˸�",
					Content = "�����Ͻðڽ��ϱ�?",
					PrimaryButtonText = "Ȯ��",
					CloseButtonText = "���"
				};

				dialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await dialog.ShowAsync();

				if (result == ContentDialogResult.Primary)
				{
					if (customDay != null)
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
							writer.Write(string.Empty);
						}

						// ������ �޸�� ������ ����
						string googleShedule = string.Empty;
						foreach (CustomTimetableItem calendarTimetableItem in customDay.Timetable)
						{
							googleShedule += calendarTimetableItem.Summary + "\r";
						}
						customDay.LocalMemo = string.Empty;
						customDay.Memo = googleShedule;
						_calendarGridViewSource[itemIndex].Connection.Add(customDay);
						_calendarGridViewSource[itemIndex].Connection.RemoveAt(0);
						MonthData.Source = _calendarGridViewSource;
						Txtblk_MyLocalMemo.Text = string.Empty;
					}
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

		#region [| �޸� ���� Ŭ�� |]
		private async void Btn_MyLocalMemoSave_Click(object sender, RoutedEventArgs e)
		{
			CustomDay customDay = _tempForDetailEvent_targetGrid.Children.OfType<Button>().FirstOrDefault().Tag as CustomDay;
			int itemIndex = CalendarGridView.IndexFromContainer(CalendarGridView.ContainerFromItem(customDay));
			try
			{
				if (customDay != null && !string.IsNullOrEmpty(Txtblk_MyLocalMemo.Text))
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
						writer.Write(Txtblk_MyLocalMemo.Text);
					}

					// ������ �޸�� ������ ����
					string googleShedule = string.Empty;
					foreach (CustomTimetableItem calendarTimetableItem in customDay.Timetable)
					{
						googleShedule += calendarTimetableItem.Summary + "\r";
					}
					customDay.LocalMemo = Txtblk_MyLocalMemo.Text;
					customDay.Memo = googleShedule + Txtblk_MyLocalMemo.Text;
					_calendarGridViewSource[itemIndex].Connection.Add(customDay);
					_calendarGridViewSource[itemIndex].Connection.RemoveAt(0);
					MonthData.Source = _calendarGridViewSource;

					SystemMessage("���� �Ϸ�", new SolidColorBrush(Common.GetColor("#80128b44")));
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

		#region [| ��� Ŭ�� |]
		private void Btn_SheduleCancel_Click(object sender, RoutedEventArgs e)
		{
			ScheduleList.ItemsSource = new List<CustomTimetableItem>(_tempListViewData);
		}
		#endregion

		#region [| ���� ���� ���� Ŭ�� |]
		private async void Btn_SheduleDelete_Click(object sender, RoutedEventArgs e)
		{
			// dialog ����
			ContentDialog dialog = new ContentDialog
			{
				Title = "�˸�",
				Content = "�����Ͻðڽ��ϱ�?",
				PrimaryButtonText = "Ȯ��",
				CloseButtonText = "���"
			};

			dialog.XamlRoot = this.MyPanel.XamlRoot;

			ContentDialogResult result = await dialog.ShowAsync();

			if (result == ContentDialogResult.Primary && Btn_SheduleDelete.Tag != null)
			{
				CustomTimetableItem targetItem = Btn_SheduleDelete.Tag as CustomTimetableItem;
				CalendarCore.DeleteGoogleCalendarData(targetItem.Id);
				DateTime date = new DateTime(targetItem.Year, targetItem.Month, targetItem.Day);

				// ���� ���� (ListView) ������ ������Ʈ
				List<CalendarTimetableItem> newGoogleCalendar = CalendarCore.GetGoogleCalendarData(date, date.AddDays(1));
				List<CustomTimetableItem> dataList = ToCustomTimetableItem(newGoogleCalendar);
				ScheduleList.ItemsSource = dataList;
				_tempListViewData = dataList;

				// �޷� ������ ����
				CustomDay customDay = _tempForDetailEvent_targetGrid.Children.OfType<Button>().FirstOrDefault().Tag as CustomDay;
				int itemIndex = CalendarGridView.IndexFromContainer(CalendarGridView.ContainerFromItem(customDay));
				string googleShedule = string.Empty;
				foreach (CustomTimetableItem calendarTimetableItem in dataList)
				{
					googleShedule += calendarTimetableItem.Summary + "\r";
				}
				customDay.Memo = googleShedule + customDay.LocalMemo;
				customDay.Timetable = dataList;
				_calendarGridViewSource[itemIndex].Connection.Add(customDay);
				_calendarGridViewSource[itemIndex].Connection.RemoveAt(0);
				MonthData.Source = _calendarGridViewSource;

				// �󼼺��� ����
				Txtbox_GoogleSheduleSummary.Text = string.Empty;
				Txtbox_GoogleSheduleDescription.Text = string.Empty;
				GoogleSheduleStartTime.SelectedTime = null;
				GoogleSheduleEndTime.SelectedTime = null;
				Btn_SheduleDelete.Visibility = Visibility.Collapsed;
				Btn_SheduleCancel.Visibility = Visibility.Collapsed;
				Txtblk_GoogleSheduleAddOrRetouch.Text = "�߰�";
				SIcon_GoogleSheduleAddOrRetouch.Symbol = Symbol.Add;
				Btn_SheduleAddOrRetouch.Tag = null;
				Btn_SheduleDelete.Tag = null;

				SystemMessage("���� ���� �Ϸ�", new SolidColorBrush(Common.GetColor("#80128b44")));
			}
		}
		#endregion

		#region [| ���� ���� ���� �� �߰� Ŭ�� |]
		private void Btn_SheduleAddOrRetouch_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(Txtbox_GoogleSheduleSummary.Text))
			{
				SystemMessage("������ �Է��ϼ���", new SolidColorBrush(Common.GetColor("#80c50500")));
				Txtbox_GoogleSheduleSummary.Focus(FocusState.Programmatic);
			} else if (GoogleSheduleStartTime.SelectedTime == null)
			{
				SystemMessage("���� �ð��� �����ϼ���", new SolidColorBrush(Common.GetColor("#80c50500")));
			} else if (GoogleSheduleEndTime.SelectedTime == null)
			{
				SystemMessage("���� �ð��� �����ϼ���", new SolidColorBrush(Common.GetColor("#80c50500")));
			} else if (_tempForDetailEvent_customDay != null)
			{
				DateTime date = new DateTime(_tempForDetailEvent_customDay.Year,
					_tempForDetailEvent_customDay.Month,
					_tempForDetailEvent_customDay.Day);
				DateTime start = new DateTime(date.Year,
					date.Month,
					date.Day,
					((TimeSpan)GoogleSheduleStartTime.SelectedTime).Hours,
					((TimeSpan)GoogleSheduleStartTime.SelectedTime).Minutes,
					((TimeSpan)GoogleSheduleStartTime.SelectedTime).Seconds
					);
				DateTime nextDate = TimeSpan.Compare((TimeSpan)GoogleSheduleEndTime.SelectedTime, (TimeSpan)GoogleSheduleStartTime.SelectedTime) == -1 ? date.AddDays(1) : date;
				DateTime end = new DateTime(nextDate.Year,
					nextDate.Month,
					nextDate.Day,
					((TimeSpan)GoogleSheduleEndTime.SelectedTime).Hours,
					((TimeSpan)GoogleSheduleEndTime.SelectedTime).Minutes,
					((TimeSpan)GoogleSheduleEndTime.SelectedTime).Seconds
					);

				string message = string.Empty;
				if (SIcon_GoogleSheduleAddOrRetouch.Symbol == Symbol.Repair && Btn_SheduleAddOrRetouch.Tag != null)
				{
					CustomTimetableItem targetItem = Btn_SheduleAddOrRetouch.Tag as CustomTimetableItem;
					CalendarCore.UpdateGoogleCalendarData(targetItem.Id, 
						Txtbox_GoogleSheduleSummary.Text,
						Txtbox_GoogleSheduleDescription.Text == null ? string.Empty : Txtbox_GoogleSheduleDescription.Text,
						start, end);
					message = "���� ���� �Ϸ�";
				} else
				{
					CalendarCore.CreateGoogleCalendarData(Txtbox_GoogleSheduleSummary.Text,
						Txtbox_GoogleSheduleDescription.Text == null ? string.Empty : Txtbox_GoogleSheduleDescription.Text,
						start, end);
					message = "���� �߰� �Ϸ�";
				}

				// ���� ���� (ListView) ������ ������Ʈ
				List<CalendarTimetableItem> newGoogleCalendar = CalendarCore.GetGoogleCalendarData(date, date.AddDays(1));
				List<CustomTimetableItem> dataList = ToCustomTimetableItem(newGoogleCalendar);
				ScheduleList.ItemsSource = dataList;
				_tempListViewData = dataList;

				// �޷� ������ ����
				CustomDay customDay = _tempForDetailEvent_targetGrid.Children.OfType<Button>().FirstOrDefault().Tag as CustomDay;
				int itemIndex = CalendarGridView.IndexFromContainer(CalendarGridView.ContainerFromItem(customDay));
				string googleShedule = string.Empty;
				foreach (CustomTimetableItem calendarTimetableItem in dataList)
				{
					googleShedule += calendarTimetableItem.Summary + "\r";
				}
				customDay.Memo = googleShedule + customDay.LocalMemo;
				customDay.Timetable = dataList;
				_calendarGridViewSource[itemIndex].Connection.Add(customDay);
				_calendarGridViewSource[itemIndex].Connection.RemoveAt(0);
				MonthData.Source = _calendarGridViewSource;

				// �󼼺��� ����
				Txtbox_GoogleSheduleSummary.Text = string.Empty;
				Txtbox_GoogleSheduleDescription.Text = string.Empty;
				GoogleSheduleStartTime.SelectedTime = null;
				GoogleSheduleEndTime.SelectedTime = null;
				Btn_SheduleDelete.Visibility = Visibility.Collapsed;
				Btn_SheduleCancel.Visibility = Visibility.Collapsed;
				Txtblk_GoogleSheduleAddOrRetouch.Text = "�߰�";
				SIcon_GoogleSheduleAddOrRetouch.Symbol = Symbol.Add;
				Btn_SheduleAddOrRetouch.Tag = null;
				Btn_SheduleDelete.Tag = null;

				SystemMessage(message, new SolidColorBrush(Common.GetColor("#80128b44")));
			}

		}
		#endregion

		#region [| �󼼺��� �ݱ� Ŭ�� |]
		private void Btn_DetailClose_Click(object sender, RoutedEventArgs e)
		{
			DayDetailGrid.Visibility = Visibility.Collapsed;
		}
		#endregion

		#endregion

		private List<CustomTimetableItem> ToCustomTimetableItem(List<CalendarTimetableItem> calendarTimetable)
		{
			List<CustomTimetableItem> dataList = new List<CustomTimetableItem>();
			CustomTimetableItem data = null;
			string preview = string.Empty;
			foreach (CalendarTimetableItem calendarTimetableItem in calendarTimetable)
			{
				data = new CustomTimetableItem();
				preview = string.Empty;
				data.Id = calendarTimetableItem.Id;
				data.Year = calendarTimetableItem.Year;
				data.Month = calendarTimetableItem.Month;
				data.Day = calendarTimetableItem.Day;
				data.StartTime = calendarTimetableItem.StartTime;
				data.EndTime = calendarTimetableItem.EndTime;
				preview += calendarTimetableItem.StartTime != null ? ((DateTime)calendarTimetableItem.StartTime).ToString("HH:mm") : "??:??";
				preview += " ~ ";
				preview += ((DateTime)calendarTimetableItem.StartTime).Day == ((DateTime)calendarTimetableItem.EndTime).Day ? string.Empty : ((DateTime)calendarTimetableItem.EndTime).ToString("dd�� ");
				preview += calendarTimetableItem.EndTime != null ? ((DateTime)calendarTimetableItem.EndTime).ToString("HH:mm") : "??:??";
				data.StarEndPreview = preview;
				data.Description = calendarTimetableItem.Description;
				data.Summary = calendarTimetableItem.Summary;
				dataList.Add(data);
			}
			return dataList;
		}

		private void SystemMessage(string message, SolidColorBrush background)
		{
			SystemMessageGrid.Background = background;
			Txtblk_SystemMessage.Text = message;
			SystemMessageGrid.Visibility = Visibility.Visible;
			MessageOpen.Begin();
			MessageOpen.Completed += (_, _) => MessageDelay.Begin();
			MessageDelay.Completed += (_, _) => MessageClose.Begin();
			MessageClose.Completed += (_, _) => SystemMessageGrid.Visibility = Visibility.Collapsed;
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
		public List<CustomTimetableItem> Timetable { get; set; }
		public Brush NumberColor { get; set; }
		public Brush FontColor { get; set; }
	}

	public class CustomTimetableItem
	{
		public string Id { get; set; }
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

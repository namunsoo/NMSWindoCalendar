using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NMS.Helpers
{
    class CalendarCore
	{
        public static List<CalendarDay> GetMonth(DateTime today)
        {
			int[] days = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
			if ((today.Year % 4 == 0) && ((today.Year % 100 != 0) || (today.Year % 400 == 0)))
			{
				days[1] = 29;//2월달의 날 수를 29로 설정 
			}

			CalendarDay calendarDay = new CalendarDay();
			List<CalendarDay> listDay = new List<CalendarDay>();
			for (int i = 1; i <= days[today.Month-1]; i++)
			{
				calendarDay = new CalendarDay();
				calendarDay.Year = today.Year;
				calendarDay.Month = today.Month;
				calendarDay.Day = i;
				calendarDay.Memo = "메모장메모장메모장\r메모장메모장메모장\r메모장메모장메모장\r메모장메모장메모장\r메모장메모장메모장\r메모장메모장메모장\r메모장메모장메모장\r";
				listDay.Add(calendarDay);
			}

			return listDay;
		}

		public static int FristWeekStart(DateTime today) {
			int[] days = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
			if ((today.Year % 4 == 0) && ((today.Year % 100 != 0) || (today.Year % 400 == 0)))
			{
				days[1] = 29;
			}
			int countDay = 0;
			for (int i = 0; i < today.Month - 1; i++)
			{
				countDay += days[i];
			}
			return (today.Year + (today.Year / 4 - today.Year / 100 + today.Year / 400) + countDay) % 7;
		}

		public static void GetGoogleCalendarData()
		{
			string[] Scopes = { CalendarService.Scope.Calendar };
			string ApplicationName = "MyCalendar";

			UserCredential credential;

			using (var stream =
				new FileStream("C:\\MyCalendarAssets\\client_secret.json", FileMode.Open, FileAccess.Read))
			{
				string credPath = "C:\\MyCalendarAssets\\token";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
			}

			// Create Google Calendar API service.
			var service = new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			// Example: List the user's upcoming events
			Events events = service.Events.List("primary").Execute();
			if (events.Items != null && events.Items.Count > 0)
			{
				foreach (var eventItem in events.Items)
				{
					string when = eventItem.Start.DateTime.ToString();
					if (string.IsNullOrEmpty(when))
					{
						when = eventItem.Start.Date;
					}
					Console.WriteLine($"{eventItem.Summary} ({when})");
				}
			}
		}
	}
}

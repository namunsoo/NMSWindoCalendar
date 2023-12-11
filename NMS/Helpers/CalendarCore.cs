using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				calendarDay.Memo = "메모장";
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

	}
}

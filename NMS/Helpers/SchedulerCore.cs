using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS.Helpers
{
    class SchedulerCore
    {
        public void GetMonth(DateTime today)
        {
			//// 년도
			//today.Year;

			//// 월
			//today.Month

			//// 일
			//today.Day
			int fristWeekStart = (today.Year + (today.Year / 4 - today.Year / 100 + today.Year / 400)) % 7;
			int[] days = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
			if ((today.Year % 4 == 0) && ((today.Year % 100 != 0) || (today.Year % 400 == 0)))//윤년 판단 
			{
				days[1] = 29;//2월달의 날 수를 29로 설정 
			}

		}

	}
}

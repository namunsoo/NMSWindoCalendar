using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS.Helpers
{
	public class SchedulerDay
    {
		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public string Memo {  get; set; }
    }

	public class SchedulerMonth
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public List<SchedulerDay> Days { get; set; }
	}

	public class SchedulerYear
	{
		public int Year { get; set; }
		public List<SchedulerMonth> Months { get; set; }
	}
}

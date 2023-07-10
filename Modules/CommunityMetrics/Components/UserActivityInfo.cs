using System;

namespace HCC.CommunityMetrics
{
    public class UserActivityInfo
	{

		// initialization
		public UserActivityInfo()
		{
		}
        public int ActivityId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public int Count { get; set; }

        public string Notes { get; set; }

    }
}
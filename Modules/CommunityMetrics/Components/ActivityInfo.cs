using System.Collections;
using DotNetNuke.Entities;

namespace HCC.CommunityMetrics
{
    public enum MetricTypeEnum : int
	{
		Undefined = -1,
		Daily = 0,
		Cumulative = 1,
        Once = 2
	}

	public class ActivityInfo : BaseEntityInfo
	{
		// initialization
		public ActivityInfo()
		{
		}

		// public properties
		public int ActivityId { get; set; }

		public string ActivityName { get; set; }

        public string Description { get; set; }

        public string TypeName { get; set; }

        public double Factor { get; set; }

        public bool IsActive { get; set; }

        public System.DateTime LastExecutionDate { get; set; }

        public MetricTypeEnum MetricType { get; set; }

        public string UserFilter { get; set;  }

        public Hashtable Settings { get; set; }

        public int MinDaily { get; set; }

        public int MaxDaily { get; set; }
    }

}